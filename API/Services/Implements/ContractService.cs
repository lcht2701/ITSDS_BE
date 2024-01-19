using API.DTOs.Requests.Contracts;
using API.DTOs.Responses.Contracts;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ContractService : IContractService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Contract> _contractRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IRepositoryBase<ServiceContract> _serviceContractRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;

    public ContractService(IRepositoryBase<User> userRepository, IRepositoryBase<Contract> contractRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<ServiceContract> serviceContractRepository, IAttachmentService attachmentService, IMapper mapper)
    {
        _userRepository = userRepository;
        _contractRepository = contractRepository;
        _companyMemberRepository = companyMemberRepository;
        _serviceContractRepository = serviceContractRepository;
        _attachmentService = attachmentService;
        _mapper = mapper;
    }

    public async Task<List<GetContractResponse>> Get()
    {
        var result = (await _contractRepository.GetAsync(navigationProperties: new string[] { "Company" })).ToList();
        List<GetContractResponse> response = await GetContractResponse(result);
        return response;
    }

    public async Task<List<GetContractResponse>> GetByCompanyAdmin(int userId)
    {
        var companyAdmin = await IsCompanyAdmin(userId);
        List<Contract> result = new();
        result = (await _contractRepository.WhereAsync(x => x.CompanyId.Equals(companyAdmin.CompanyId), new string[] { "Company" })).ToList();
        List<GetContractResponse> response = await GetContractResponse(result);
        return response;
    }

    public async Task<GetContractResponse> GetById(int id, int currentUserId)
    {
        var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(currentUserId));
        if (currentUser.Role == Role.Customer) await IsCompanyAdmin(currentUserId);
        Contract result = await _contractRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Company" }) ?? throw new KeyNotFoundException("Contract is not exist");
        var response = _mapper.Map(result, new GetContractResponse());
        response.AttachmentUrls = (await _attachmentService.Get(Tables.CONTRACT, response.Id)).Select(x => x.Url).ToList();
        return response;
    }

    public async Task<Contract> Create(CreateContractRequest model)
    {
        var entity = _mapper.Map(model, new Contract());
        entity.EndDate = entity.StartDate.AddMonths(model.Duration);
        CommonService.SetContractStatus(entity);
        var contract = await _contractRepository.CreateAsync(entity);
        await _attachmentService.Add(Tables.CONTRACT, contract.Id, model.AttachmentUrls);
        return contract;
    }

    public async Task<Contract> Update(int id, UpdateContractRequest model)
    {
        var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Contract is not exist"));
        if (target.Status == ContractStatus.Expired)
        {
            throw new BadRequestException("This contract cannot be edited when expired");
        }
        else if (target.Status != ContractStatus.Pending)
        {
            throw new BadRequestException("Contract can only be edited in pending stage");
        }
        Contract entity = _mapper.Map(model, target);
        entity.EndDate = entity.StartDate.AddMonths(model.Duration);
        CommonService.SetContractStatus(entity);
        var result = await _contractRepository.UpdateAsync(entity);
        await _attachmentService.Update(Tables.CONTRACT, result.Id, model.AttachmentUrls);
        var contract = await _contractRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Contract is not exist"));
        //if (target.Status != ContractStatus.Pending)
        //{
        //    throw new BadRequestException("Contract can only be removed in pending stage");
        //}
        await _contractRepository.SoftDeleteAsync(target);
        await _attachmentService.Delete(Tables.CONTRACT, target.Id);
        var relatedServices = await _serviceContractRepository.WhereAsync(x => x.ContractId == target.Id);
        foreach (var relatedService in relatedServices)
        {
            await _serviceContractRepository.DeleteAsync(relatedService);
        }
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