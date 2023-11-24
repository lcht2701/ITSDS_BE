using API.DTOs.Requests.ServiceContracts;
using API.Services.Interfaces;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ServiceContractService : IServiceContractService
{
    private readonly IRepositoryBase<ServiceContract> _repo;

    public ServiceContractService(IRepositoryBase<ServiceContract> repo)
    {
        _repo = repo;
    }

    public async Task<List<ServiceContract>> Get(int contractId)
    {
        return (await _repo.WhereAsync(x => x.ContractId.Equals(contractId), new string[] { "Contract", "Service" })).ToList();
    }

    public async Task<ServiceContract> GetById(int id)
    {
        var result = await _repo.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Contract", "Service" }) ?? throw new KeyNotFoundException("Service is not exist in the contract");
        return result;
    }

    public async Task<List<ServiceContract>> ModifyServices(ModifyServicesInContract model)
    {
        List<ServiceContract> result = new();
        var relatedServices = await _repo.GetAsync(x => x.ContractId.Equals(model.ContractId));
        if (relatedServices == null)
        {
            foreach (var serviceId in model.ServiceIds!)
            {
                result.Add(new ServiceContract()
                {
                    ContractId = model.ContractId,
                    ServiceId = serviceId
                });
            }
        }
        else
        {
            var existingServiceIds = relatedServices.Select(x => (int)x.ServiceId!);
            // Remove services that are no longer associated with the contract
            var servicesToRemove = relatedServices.Where(rs => !model.ServiceIds!.Contains((int)rs.ServiceId!)).ToList();
            foreach (var serviceToRemove in servicesToRemove)
            {
                await _repo.DeleteAsync(serviceToRemove);
            }

            // Add new services that are not in the existing list
            var servicesToAdd = model.ServiceIds!.Except(existingServiceIds).ToList();
            foreach (var serviceId in servicesToAdd)
            {
                result.Add(new ServiceContract()
                {
                    ContractId = model.ContractId,
                    ServiceId = serviceId
                });
            }
        }
        await _repo.CreateAsync(result);
        return result;
    }
}
