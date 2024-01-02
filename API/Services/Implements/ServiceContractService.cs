using API.DTOs.Requests.ServiceContracts;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ServiceContractService : IServiceContractService
{
    private readonly IRepositoryBase<ServiceContract> _repo;
    private readonly IRepositoryBase<Service> _serviceRepo;
    private readonly IRepositoryBase<Ticket> _ticketRepo;
    private readonly IRepositoryBase<Contract> _contractRepo;
    private readonly IRepositoryBase<Company> _companyRepo;
    private readonly IRepositoryBase<Category> _categoryRepo;
    private readonly IRepositoryBase<User> _userRepo;
    private readonly IMapper _mapper;

    public ServiceContractService(IRepositoryBase<ServiceContract> repo, IRepositoryBase<Service> serviceRepo, IRepositoryBase<Ticket> ticketRepo, IRepositoryBase<Contract> contractRepo, IRepositoryBase<Company> companyRepo, IRepositoryBase<Category> categoryRepo, IRepositoryBase<User> userRepo, IMapper mapper)
    {
        _repo = repo;
        _serviceRepo = serviceRepo;
        _ticketRepo = ticketRepo;
        _contractRepo = contractRepo;
        _companyRepo = companyRepo;
        _categoryRepo = categoryRepo;
        _userRepo = userRepo;
        _mapper = mapper;
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

    public async Task<List<Service>> GetServicesList(int contractId)
    {
        var currentServiceIds = (await _repo.WhereAsync(x => x.ContractId.Equals(contractId))).Select(x => x.ServiceId);
        var result = (await _serviceRepo.WhereAsync(x => !currentServiceIds.Contains(x.Id))).ToList();
        return result;
    }

    public async Task<List<ServiceContract>> Add(int contractId, List<int> serviceIds)
    {
        List<ServiceContract> result = new();
        foreach (var serviceId in serviceIds)
        {
            result.Add(new ServiceContract()
            {
                ContractId = contractId,
                ServiceId = serviceId
            });
        }
        await _repo.CreateAsync(result);
        return result;
    }

    public async Task<ServiceContract> AddPeriodicService(int contractId, AddPeriodicService model)
    {
        var entity = _mapper.Map(model, new ServiceContract());
        entity.ContractId = contractId;
        await _repo.CreateAsync(entity);
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _repo.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Service is not exist in the contract");
        await _repo.DeleteAsync(target);
    }

    public async Task<List<Ticket>> CreatePeriodicTickets(int id, int currentUserId)
    {
        List<Ticket> list = new();
        var serviceContract = await _repo.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Service is not exist in the contract");

        if (!serviceContract.StartDate.HasValue || !serviceContract.EndDate.HasValue || !serviceContract.Frequency.HasValue)
        {
            return list;
        }

        #region Data For Create Contract
        var service = await _serviceRepo.FirstOrDefaultAsync(x => x.Id.Equals(serviceContract.ServiceId));
        var contract = await _contractRepo.FirstOrDefaultAsync(x => x.Id.Equals(serviceContract.ContractId));
        var company = await _companyRepo.FirstOrDefaultAsync(x => x.Id.Equals(contract.CompanyId));
        #endregion

        TimeSpan? totalDate = serviceContract.EndDate - serviceContract.StartDate;
        int numOfTickets = (int)Math.Ceiling((decimal)(totalDate.Value.Days / serviceContract.Frequency));
        DateTime? startDate = serviceContract.StartDate;
        for (int i = 1; i <= numOfTickets; i++)
        {
            Ticket newTicket = new()
            {
                CreatedById = currentUserId,
                Title = $"Periodic Ticket #{i} - {service.Description}",
                Description = $"Periodic Service Support #{i} for {company.CompanyName} - {service.Description}",
                ServiceId = service.Id,
                CategoryId = (await _categoryRepo.FirstOrDefaultAsync(x => x.Id.Equals(service.CategoryId))).Id,
                TicketStatus = Domain.Constants.Enums.TicketStatus.Open,
                Priority = Domain.Constants.Enums.Priority.High,
                ScheduledStartTime = startDate,
                ScheduledEndTime = startDate.Value.AddDays((double)serviceContract.Frequency),
                DueTime = startDate.Value.AddDays((double)serviceContract.Frequency),
                Impact = Domain.Constants.Enums.Impact.Medium,
                Urgency = Domain.Constants.Enums.Urgency.Medium,
                IsPeriodic = true,
            };
            list.Add(newTicket);
            startDate = startDate.Value.AddDays((double)serviceContract.Frequency + 1);
        }
        await _ticketRepo.CreateAsync(list);
        return list;
    }
}
