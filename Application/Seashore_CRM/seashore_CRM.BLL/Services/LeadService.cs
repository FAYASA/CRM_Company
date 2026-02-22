using AutoMapper;
using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.BLL.Services.Service_Interfaces;

namespace seashore_CRM.BLL.Services
{
    public class LeadService : ILeadService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILeadItemRepository _leadItemRepo;

        public LeadService(IUnitOfWork uow, IMapper mapper, ILeadItemRepository leadItemRepo)
        {
            _uow = uow;
            _mapper = mapper;
            _leadItemRepo = leadItemRepo;
        }

        public async Task<int> AddLeadAsync(LeadDto leadDto)
        {
            var entity = _mapper.Map<Lead>(leadDto);
            await _uow.Leads.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteLeadAsync(int id)
        {
            var lead = await _uow.Leads.GetByIdAsync(id);
            if (lead == null) return;
            _uow.Leads.Remove(lead);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<LeadDto>> GetAllLeadsAsync()
        {
            var leads = (await _uow.Leads.GetAllAsync()).ToList();

            var dtos = new List<LeadDto>();
            foreach (var l in leads)
            {
                var dto = _mapper.Map<LeadDto>(l);
                // populate names via repositories
                if (l.StatusId.HasValue)
                {
                    var status = await _uow.LeadStatuses.GetByIdAsync(l.StatusId.Value);
                    dto.StatusName = status?.StatusName;
                }
                if (l.AssignedUserId.HasValue)
                {
                    var user = await _uow.Users.GetByIdAsync(l.AssignedUserId.Value);
                    dto.AssignedUserName = user?.FullName;
                }

                // populate lead items totals (GrossTotal, Units) and product names
                var items = (await _uow.LeadItems.FindAsync(x => x.LeadId == l.Id)).ToList();
                if (items.Any())
                {
                    dto.Units = items.Sum(i => i.Quantity);
                    dto.GrossTotal = items.Sum(i => i.LineTotal);
                    dto.ProductNames = new List<string>();
                    foreach (var it in items)
                    {
                        var p = await _uow.Products.GetByIdAsync(it.ProductId);
                        if (p != null) dto.ProductNames.Add(p.ProductName);
                    }
                }

                // latest activity and updated date
                var acts = (await _uow.Activities.FindAsync(a => a.LeadId == l.Id)).OrderByDescending(a => a.ActivityDate).ToList();
                if (acts.Any())
                {
                    dto.LatestActivity = acts.First().ActivityType;
                    dto.UpdatedDate = acts.First().ActivityDate;
                }

                // closure date (use DecisionDate or ExpectedClosureDate)
                dto.ClosureDate = l.DecisionDate ?? l.ExpectedClosureDate;

                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<LeadDto?> GetLeadByIdAsync(int id)
        {
            var lead = await _uow.Leads.GetByIdAsync(id);
            if (lead == null) return null;
            var dto = _mapper.Map<LeadDto>(lead);
            if (lead.StatusId.HasValue)
            {
                var status = await _uow.LeadStatuses.GetByIdAsync(lead.StatusId.Value);
                dto.StatusName = status?.StatusName;
            }
            if (lead.AssignedUserId.HasValue)
            {
                var user = await _uow.Users.GetByIdAsync(lead.AssignedUserId.Value);
                dto.AssignedUserName = user?.FullName;
            }

            // populate items
            var items = (await _uow.LeadItems.FindAsync(x => x.LeadId == lead.Id)).ToList();
            if (items.Any())
            {
                dto.Units = items.Sum(i => i.Quantity);
                dto.GrossTotal = items.Sum(i => i.LineTotal);
                dto.ProductNames = new List<string>();
                foreach (var it in items)
                {
                    var p = await _uow.Products.GetByIdAsync(it.ProductId);
                    if (p != null) dto.ProductNames.Add(p.ProductName);
                }
            }

            var acts = (await _uow.Activities.FindAsync(a => a.LeadId == lead.Id)).OrderByDescending(a => a.ActivityDate).ToList();
            if (acts.Any())
            {
                dto.LatestActivity = acts.First().ActivityType;
                dto.UpdatedDate = acts.First().ActivityDate;
            }

            dto.ClosureDate = lead.DecisionDate ?? lead.ExpectedClosureDate;

            return dto;
        }

        public async Task UpdateLeadAsync(LeadDto leadDto)
        {
            var entity = _mapper.Map<Lead>(leadDto);
            _uow.Leads.Update(entity);
            await _uow.CommitAsync();
        }

        public async Task QualifyLeadAsync(LeadDto dto)
        {
            var lead = await _uow.Leads.GetByIdAsync(dto.Id);
            if (lead == null) return;

            lead.IsQualified = dto.IsQualified;
            lead.QualifiedOn = dto.QualifiedOn ?? DateTime.UtcNow;
            lead.QualifiedById = dto.QualifiedById;
            lead.QualificationNotes = dto.QualificationNotes;

            lead.Budget = dto.Budget;
            lead.DecisionDate = dto.DecisionDate;
            lead.Probability = dto.Probability;

            _uow.Leads.Update(lead);
            await _uow.CommitAsync();
        }

        public async Task<int?> ConvertToOpportunityAsync(int leadId)
        {
            var lead = await _uow.Leads.GetByIdAsync(leadId);
            if (lead == null) return null;

            // mark converted
            lead.IsConverted = true;
            _uow.Leads.Update(lead);

            var opp = _mapper.Map<Opportunity>(lead);
            opp.LeadId = lead.Id; // ensure relation
            await _uow.Opportunities.AddAsync(opp);
            await _uow.CommitAsync();

            return opp.Id;
        }
    }
}
