using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ServiceContractService : IServiceContractService
{
    private readonly IRepositoryBase<ServiceContract> _repo;
    private readonly IRepositoryBase<Service> _serviceRepo;
    private readonly IRepositoryBase<Contract> _contractRepo;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepo;
    private readonly IMapper _mapper;

    public ServiceContractService(IRepositoryBase<ServiceContract> repo, IRepositoryBase<Service> serviceRepo, IRepositoryBase<Contract> contractRepo, IRepositoryBase<CompanyMember> companyMemberRepo, IMapper mapper)
    {
        _repo = repo;
        _serviceRepo = serviceRepo;
        _contractRepo = contractRepo;
        _companyMemberRepo = companyMemberRepo;
        _mapper = mapper;
    }

    public async Task<List<ServiceContract>> Get(int contractId)
    {
        return (await _repo.WhereAsync(x => x.ContractId.Equals(contractId), new string[] { "Service" })).ToList();
    }

    public async Task<List<Service>> GetServicesList(int contractId)
    {
        var currentServiceIds = (await _repo.WhereAsync(x => x.ContractId.Equals(contractId))).Select(x => x.ServiceId);
        var result = (await _serviceRepo.WhereAsync(x => !currentServiceIds.Contains(x.Id))).ToList();
        return result;
    }

    public async Task<List<Service>> GetActiveServicesOfMemberCompany(int userId)
    {
        var companyMember = await _companyMemberRepo.FirstOrDefaultAsync(x => x.MemberId.Equals(userId)) ?? throw new KeyNotFoundException("Member is not found or not belong to any company");

        //Get Active Contracts Of Company
        var contractIds = (await _contractRepo
            .WhereAsync(x =>
                x.CompanyId.Equals(companyMember.CompanyId) &&
                x.Status == ContractStatus.Active))
            .Select(x => x.Id)
            .ToList();

        if (!contractIds.Any())
        {
            return new List<Service>();
        }

        //Get Services Of Company
        var serviceIds = (await _repo
            .WhereAsync(x => contractIds.Contains((int)x.ContractId!)))
            .Select(x => x.ServiceId)
            .ToList();

        if (!serviceIds.Any())
        {
            return new List<Service>();
        }

        var result = (await _serviceRepo
            .WhereAsync(x => serviceIds.Contains(x.Id)))
            .ToList();

        return result;
    }

    public async Task<ServiceContract> GetById(int id)
    {
        var result = await _repo.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Service" }) ?? throw new KeyNotFoundException("Service is not exist in the contract");
        return result;
    }

    public async Task<List<ServiceContract>> AddAndUpdate(int contractId, List<int> serviceIds)
    {
        List<ServiceContract> result = new();
        var existingServices = await _repo.WhereAsync(x => x.ContractId.Equals(contractId));
        var existingServiceIds = existingServices.Select(x => x.ServiceId);

        if (serviceIds.Any())
        {
            var servicesToRemove = existingServices.Where(rs => !serviceIds.Contains(rs.ServiceId)).ToList();
            foreach (var serviceToRemove in servicesToRemove)
            {
                await _repo.DeleteAsync(serviceToRemove);
            }

            //Add services
            var serviceIdsToAdd = serviceIds.Except(existingServiceIds).ToList();
            foreach (var serviceId in serviceIdsToAdd)
            {
                result.Add(new ServiceContract()
                {
                    ContractId = contractId,
                    ServiceId = serviceId
                });
            }
            await _repo.CreateAsync(result);
        }
        else
        {

            // Remove services that are no longer associated with the contract
            foreach (var serviceToRemove in existingServices)
            {
                await _repo.DeleteAsync(serviceToRemove);
            }
        }
        return result;
    }

    public async Task Remove(int contractId)
    {
        var target = await _repo.WhereAsync(x => x.ContractId.Equals(contractId)) ?? throw new KeyNotFoundException("Contract is not found");
        foreach (var entity in target)
        {
            await _repo.DeleteAsync(entity);
        }
    }
}
