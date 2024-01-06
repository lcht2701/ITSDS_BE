﻿using API.DTOs.Requests.ServiceContracts;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;
using System.Linq;

namespace API.Services.Implements;

public class ServiceContractService : IServiceContractService
{
    private readonly IRepositoryBase<ServiceContract> _repo;
    private readonly IRepositoryBase<Service> _serviceRepo;
    private readonly IRepositoryBase<Ticket> _ticketRepo;
    private readonly IRepositoryBase<Contract> _contractRepo;
    private readonly IRepositoryBase<Company> _companyRepo;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepo;
    private readonly IRepositoryBase<Category> _categoryRepo;
    private readonly IRepositoryBase<User> _userRepo;
    private readonly IMapper _mapper;

    public ServiceContractService(IRepositoryBase<ServiceContract> repo, IRepositoryBase<Service> serviceRepo, IRepositoryBase<Ticket> ticketRepo, IRepositoryBase<Contract> contractRepo, IRepositoryBase<Company> companyRepo, IRepositoryBase<CompanyMember> companyMemberRepo, IRepositoryBase<Category> categoryRepo, IRepositoryBase<User> userRepo, IMapper mapper)
    {
        _repo = repo;
        _serviceRepo = serviceRepo;
        _ticketRepo = ticketRepo;
        _contractRepo = contractRepo;
        _companyRepo = companyRepo;
        _companyMemberRepo = companyMemberRepo;
        _categoryRepo = categoryRepo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<List<ServiceContract>> Get(int contractId)
    {
        return (await _repo.WhereAsync(x => x.ContractId.Equals(contractId), new string[] { "Contract", "Service" })).ToList();
    }

    public async Task<List<Service>> GetActiveServicesOfMemberCompany(int userId)
    {
        var companyMember = await _companyMemberRepo.FirstOrDefaultAsync(x => x.MemberId.Equals(userId)) ?? throw new KeyNotFoundException("Member is not found or not belong to any company");

        var company = await _companyRepo.FirstOrDefaultAsync(x => x.Id.Equals(companyMember.CompanyId)) ?? throw new KeyNotFoundException("Company is not found");

        //Get Active Contracts Of Company
        var contractIds = (await _contractRepo
            .WhereAsync(x => 
                x.CompanyId.Equals(companyMember.CompanyId) && 
                x.Status == ContractStatus.Active))
            .Select(x => x.Id);
        if (contractIds == null || !contractIds.Any())
        {
            return new List<Service>();
        }

        //Get Services Of Company
        var serviceIds = (await _repo
            .WhereAsync(x => contractIds.Contains((int)x.ContractId!)))
            .Select(x => x.ServiceId);
        if (serviceIds == null || !serviceIds.Any())
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
}
