using API.DTOs.Requests.Contracts;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ContractService : IContractService
{
    private readonly IRepositoryBase<Contract> _contractRepository;
    private readonly IRepositoryBase<Renewal> _renewalRepository;
    private readonly IMapper _mapper;

    public ContractService(IRepositoryBase<Contract> contractRepository, IRepositoryBase<Renewal> renewalRepository, IMapper mapper)
    {
        _contractRepository = contractRepository;
        _renewalRepository = renewalRepository;
        _mapper = mapper;
    }

    public async Task<List<Contract>> Get()
    {
        var result = (await _contractRepository.GetAsync(navigationProperties: new string[] { "Accountant", "Company" })).ToList();
        return result;
    }

    public async Task<List<Contract>> GetChildContracts(int contractId)
    {
        var result = (await _contractRepository.WhereAsync(x => x.ParentContractId.Equals(contractId), new string[] { "Accountant", "Company" })).ToList();
        return result;
    }

    public async Task<List<Contract>> GetByAccountant(int userId)
    {
        var result = (await _contractRepository.WhereAsync(x => x.AccountantId.Equals(userId), new string[] { "Accountant", "Company" })).ToList();
        return result;
    }

    public async Task<List<Contract>> GetByCompany(int companyId)
    {
        var result = (await _contractRepository.WhereAsync(x => x.CompanyId.Equals(companyId), new string[] { "Accountant", "Company" })).ToList();
        return result;
    }

    public async Task<Contract> GetById(int id)
    {
        Contract result = (Contract)await _contractRepository.WhereAsync(x => x.Id.Equals(id), new string[] { "Accountant", "Company" });
        return result;
    }

    public async Task<Contract> Create(CreateContractRequest model)
    {
        var entity = _mapper.Map(model, new Contract());
        SetContractStatus(entity);
        entity.IsRenewed = false;
        await _contractRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Contract> Update(int id, UpdateContractRequest model)
    {
        var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Contract is not exist"));
        Contract entity = _mapper.Map(model, target);
        SetContractStatus(entity);
        await _contractRepository.UpdateAsync(entity);
        return entity;
    }
    public async Task Remove(int id)
    {
        var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("Contract is not exist"));
        await _contractRepository.SoftDeleteAsync(target);
    }

    public async Task<List<Renewal>> GetContractRenewals(int contractId)
    {
        var result = (await _renewalRepository.WhereAsync(x => x.ContractId.Equals(contractId), new string[] { "RenewedBy", "Contract" })).ToList();
        return result;
    }

    public async Task<Renewal> RenewContract(int contractId, RenewContractRequest model, int userId)
    {
        var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(contractId), new NotFoundException("Contract is not exist"));
        Contract entity = _mapper.Map(model, target);
        SetContractStatus(entity);
        entity.IsRenewed = entity.IsRenewed == false ? true : entity.IsRenewed;
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
}