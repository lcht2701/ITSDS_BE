using API.DTOs.Requests.Contracts;
using API.DTOs.Responses.Contracts;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ContractService : IContractService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Contract> _contractRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IRepositoryBase<Renewal> _renewalRepository;
    private readonly IRepositoryBase<ServiceContract> _serviceContractRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;

    public ContractService(IRepositoryBase<User> userRepository, IRepositoryBase<Contract> contractRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<Renewal> renewalRepository, IRepositoryBase<ServiceContract> serviceContractRepository, IAttachmentService attachmentService, IMapper mapper)
    {
        _userRepository = userRepository;
        _contractRepository = contractRepository;
        _companyMemberRepository = companyMemberRepository;
        _renewalRepository = renewalRepository;
        _serviceContractRepository = serviceContractRepository;
        _attachmentService = attachmentService;
        _mapper = mapper;
    }

    public async Task<List<GetContractResponse>> Get()
    {
        var result = (await _contractRepository.GetAsync(navigationProperties: new string[] { "Accountant", "Company" })).ToList();
        List<GetContractResponse> response = await GetContractResponse(result);
        return response;
    }

    public async Task<List<GetContractResponse>> GetParentContracts()
    {
        var result = (await _contractRepository.WhereAsync(x => x.ParentContractId == null, new string[] { "Accountant", "Company" })).ToList();
        List<GetContractResponse> response = await GetContractResponse(result);
        return response;
    }

    public async Task<List<GetContractResponse>> GetChildContracts(int contractId)
    {
        var result = (await _contractRepository.WhereAsync(x => x.ParentContractId.Equals(contractId), new string[] { "Accountant", "Company" })).ToList();
        List<GetContractResponse> response = await GetContractResponse(result);
        return response;
    }

    public async Task<List<GetContractResponse>> GetByAccountant(int userId)
    {
        var result = (await _contractRepository.WhereAsync(x => x.AccountantId.Equals(userId), new string[] { "Accountant", "Company" })).ToList();
        List<GetContractResponse> response = await GetContractResponse(result);
        return response;
    }

    public async Task<List<GetContractResponse>> GetByCompanyAdmin(int userId)
    {
        var companyAdmin = await IsCompanyAdmin(userId);
        List<Contract> result = new();
        result = (await _contractRepository.WhereAsync(x => x.CompanyId.Equals(companyAdmin.CompanyId), new string[] { "Accountant", "Company" })).ToList();
        List<GetContractResponse> response = await GetContractResponse(result);
        return response;
    }

    public async Task<GetContractResponse> GetById(int id, int currentUserId)
    {
        var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(currentUserId));
        if (currentUser.Role == Role.Customer) await IsCompanyAdmin(currentUserId);
        Contract result = await _contractRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Accountant", "Company" }) ?? throw new KeyNotFoundException("Contract is not exist");
        var response = _mapper.Map(result, new GetContractResponse());
        response.AttachmentUrls = (await _attachmentService.Get(Tables.CONTRACT, response.Id)).Select(x => x.Url).ToList();
        return response;
    }

    public async Task<Contract> Create(CreateContractRequest model)
    {
        var entity = _mapper.Map(model, new Contract());
        SetContractStatus(entity);
        entity.IsRenewed = false;
        if (model.ParentContractId.HasValue)
        {
            var parent = await _contractRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.ParentContractId));
            if (parent.ParentContractId.HasValue)
            {
                throw new BadRequestException("Child Contract cannot have more child contracts");
            }
            bool isValid = ValidateChildContract(entity, parent);
            if (!isValid) throw new BadRequestException($"Date must be within parent contract: From [{parent.StartDate}] to [{parent.EndDate}]");
        }
        var contract = await _contractRepository.CreateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKET, contract.Id, model.AttachmentUrls);
        }
        return entity;
    }

    public async Task<Contract> Update(int id, UpdateContractRequest model)
    {
        var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Contract is not exist"));
        Contract entity = _mapper.Map(model, target);
        SetContractStatus(entity);
        if (model.ParentContractId.HasValue)
        {
            var parent = await _contractRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.ParentContractId));
            bool isValid = ValidateChildContract(entity, parent);
            if (isValid == false) throw new BadRequestException($"Date must be within parent contract: From [{parent.StartDate}] to [{parent.EndDate}]");
        }
        await _contractRepository.UpdateAsync(entity);
        var contract = await _contractRepository.UpdateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Update(Tables.TICKET, contract.Id, model.AttachmentUrls);
        }
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Contract is not exist"));
        await _contractRepository.SoftDeleteAsync(target);
        await _attachmentService.Delete(Tables.TICKET, target.Id);
        var relatedServices = await _serviceContractRepository.WhereAsync(x => x.ContractId == target.Id);
        foreach (var relatedService in relatedServices)
        {
            await _serviceContractRepository.DeleteAsync(relatedService);
        }
    }

    public async Task<List<Renewal>> GetContractRenewals(int contractId)
    {
        var result = (await _renewalRepository.WhereAsync(x => x.ContractId.Equals(contractId), new string[] { "RenewedBy", "Contract" })).ToList();
        return result;
    }

    public async Task<Renewal> RenewContract(int contractId, RenewContractRequest model, int userId)
    {
        var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(contractId), new KeyNotFoundException("Contract is not exist"));
        var renewals = await _renewalRepository.WhereAsync(x => x.ContractId == contractId);
        if (!renewals.Any())
        {
            Renewal firstRenewal = new()
            {
                ContractId = target.Id,
                Description = target.Description,
                FromDate = target.StartDate,
                ToDate = target.EndDate,
                Value = target.Value,
                RenewedDate = DateTime.Now,
                RenewedById = userId
            };
            await _renewalRepository.CreateAsync(firstRenewal);
        }
        Contract entity = _mapper.Map(model, target);
        SetContractStatus(entity);
        entity.IsRenewed = entity.IsRenewed == false ? true : entity.IsRenewed;
        await _contractRepository.UpdateAsync(entity);
        Renewal newRenewal = new()
        {
            ContractId = entity.Id,
            Description = entity.Description,
            FromDate = entity.StartDate,
            ToDate = entity.EndDate,
            Value = entity.Value,
            RenewedDate = DateTime.Now,
            RenewedById = userId
        };
        await _renewalRepository.CreateAsync(newRenewal);
        return newRenewal;
    }

    private async Task<List<GetContractResponse>> GetContractResponse(List<Contract> result)
    {
        var response = _mapper.Map<List<GetContractResponse>>(result);
        foreach (var item in response)
        {
            item.AttachmentUrls = (await _attachmentService.Get(Tables.CONTRACT, item.Id)).Select(x => x.Url).ToList();
        }

        return response;
    }
    private static void SetContractStatus(Contract entity)
    {
        if (entity.StartDate < DateTime.Now)
        {
            entity.Status = ContractStatus.Pending;
        }
        else if (entity.EndDate >= DateTime.Now)
        {
            entity.Status = ContractStatus.Active;
        }
        else
        {
            entity.Status = ContractStatus.Expired;
        }
    }
    private static bool ValidateChildContract(Contract child, Contract parent)
    {
        bool isValid = true;
        if (child.StartDate < parent.StartDate || child.EndDate > parent.EndDate)
        {
            isValid = false;
        }
        return isValid;
    }
    private async Task<CompanyMember> IsCompanyAdmin(int currentUserId)
    {
        var currentUserMember = await _companyMemberRepository
                                            .FirstOrDefaultAsync(x =>
                                                            x.MemberId.Equals(currentUserId) &&
                                                            x.IsCompanyAdmin == true);
        if (currentUserMember == null)
        {
            throw new UnauthorizedAccessException("User is not authorize for this action");
        }

        return currentUserMember;
    }

}