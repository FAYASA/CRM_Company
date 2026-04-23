using AutoMapper;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.BLL.Services.Service_Interfaces;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using seashore_CRM.DomainModelLayer.Entities;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace seashore_CRM.BLL.Services
{
    public partial class LeadService : ILeadService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILeadItemRepository _leadItemRepo;
        private readonly IValidator<LeadDto> _validator;
        private readonly IUserActivityService _activityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LeadService(IUnitOfWork uow, ILeadItemRepository leadItemRepo, IValidator<LeadDto> validator, IUserActivityService activityService, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _leadItemRepo = leadItemRepo;
            _validator = validator;
            _activityService = activityService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<LeadDto>> GetAllLeadsAsync()
        {
            var leads = (await _uow.Leads.GetAllAsync()).ToList();
            var result = new List<LeadDto>();

            foreach (var l in leads)
            {
                var dto = new LeadDto
                {
                    Id = l.Id,
                    LeadType = l.LeadType,
                    CompanyId = l.CompanyId,
                    ContactId = l.ContactId,
                    SourceId = l.SourceId,
                    StatusId = l.StatusId,
                    Priority = l.Priority,
                    AssignedUserId = l.AssignedUserId,
                    IsQualified = l.IsQualified,
                    QualificationNotes = l.QualificationNotes,
                    Budget = l.Budget,
                    DecisionDate = l.DecisionDate,
                    Probability = l.Probability,
                    UpdatedDate = l.UpdatedDate
                };

                // Customer name: prefer company, then contact, otherwise fallback to LeadType
                if (l.CompanyId.HasValue)
                {
                    var comp = await _uow.Companies.GetByIdAsync(l.CompanyId.Value);
                    dto.CustomerName = comp?.CompanyName ?? l.LeadType;
                    dto.CustomerLocation = comp?.City;
                }
                else if (l.ContactId.HasValue)
                {
                    var contact = await _uow.Contacts.GetByIdAsync(l.ContactId.Value);
                    dto.CustomerName = !string.IsNullOrWhiteSpace(contact?.ContactName) ? contact.ContactName : (contact?.Email ?? contact?.Mobile ?? l.LeadType);
                    dto.CustomerLocation = !string.IsNullOrWhiteSpace(contact?.Mobile) ? contact.Mobile : contact?.Phone;
                }
                else if (l.IndividualCustomerId.HasValue)
                {
                    var ind = await _uow.IndividualCustomers.GetByIdAsync(l.IndividualCustomerId.Value);
                    dto.CustomerName = ind?.Name ?? l.LeadType;
                    dto.CustomerLocation = ind?.Location;
                }
                else
                {
                    dto.CustomerName = l.LeadType;
                }

                if (l.StatusId.HasValue)
                {
                    var st = await _uow.LeadStatuses.GetByIdAsync(l.StatusId);
                    if (st != null)
                    {
                        dto.StatusName = st.StatusName;
                        // get activities for this status
                        var acts = (await _uow.LeadStatusActivities.FindAsync(a => a.LeadStatusId == st.Id)).Select(a => a.ActivityName).ToList();
                        dto.StatusActivities = acts.Any() ? acts : null;
                    }
                }

                if (l.AssignedUserId.HasValue)
                {
                    var u = await _uow.Users.GetByIdAsync(l.AssignedUserId.Value);
                    if (u != null) dto.AssignedUserName = u.FullName;
                }

                var items = (await _uow.LeadItems.FindAsync(li => li.LeadId == l.Id)).ToList();
                if (items.Any())
                {
                    dto.GrossTotal = items.Sum(i => i.LineTotal);
                    dto.Units = items.Sum(i => i.Quantity);
                    var pnames = new List<string>();
                    foreach (var it in items)
                    {
                        var p = await _uow.Products.GetByIdAsync(it.ProductId);
                        if (p != null) pnames.Add(p.ProductName);
                    }
                    dto.ProductNames = pnames.Distinct().ToList();
                }

                // Use LeadStatusActivities as the activity records for a lead (ordered descending by date)
                var acts2 = (await _uow.LeadStatusActivities.FindAsync(a => a.LeadId == l.Id)).OrderByDescending(a => a.ActivityDate).ToList();
                if (acts2.Any())
                {
                    dto.LatestActivity = acts2.First().ActivityName;
                    dto.UpdatedDate = acts2.First().ActivityDate;
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<(string result, int? leadId)> CreateLeadAsync(LeadDto leadDto)
        {
            // Validate DTO at service boundary
            if (_validator != null)
            {
                var v = await _validator.ValidateAsync(leadDto);
                if (!v.IsValid) throw new ValidationException(v.Errors);
            }

            try
            {
                var leadEntity = new Lead
                {
                    LeadType = leadDto.LeadType,
                    CompanyId = leadDto.CompanyId,
                    ContactId = leadDto.ContactId,
                    // store selected activity template id (if any)
                    ActivityId = leadDto.ActivityId,
                    IndividualCustomerId = leadDto.IndividualCustomerId ?? (leadDto.LeadType == "Individual" ? leadDto.ContactId : null),
                    SourceId = leadDto.SourceId,
                    StatusId = leadDto.StatusId,
                    Priority = leadDto.Priority,
                    AssignedUserId = leadDto.AssignedUserId,
                    ExpectedClosureDate = leadDto.ExpectedClosureDate,
                    FollowUpDate = leadDto.FollowUpDate,
                    FollowUpTime = leadDto.FollowUpTime,
                    ActivityType = leadDto.ActivityType,
                    AttachmentsJson = leadDto.AttachmentsJson,
                    //IsConverted = leadDto.IsConverted,
                    IsQualified = leadDto.IsQualified,
                    //QualifiedOn = leadDto.QualifiedOn,
                    //QualifiedById = leadDto.QualifiedById,
                    QualificationNotes = leadDto.QualificationNotes,
                    Budget = leadDto.Budget,
                    DecisionDate = leadDto.DecisionDate,
                    Probability = leadDto.Probability
                };

                // Use explicit transaction to make create atomic
                Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? tx = null;
                try
                {
                    tx = await _uow.BeginTransactionAsync();

                    // Add lead and save to obtain Id within transaction


                    /////////////////////  To save Leads ////////////////////////
                    ///
                    await _uow.Leads.AddAsync(leadEntity);
                    await _uow.CommitAsync();

                    //////////////////  For save leades Items  ////////////////////////
                    ///
                    /// Persist lead items now that leadEntity.Id is available
                    if (leadDto.ProductItems != null && leadDto.ProductItems.Any())
                    {
                        foreach (var pi in leadDto.ProductItems)
                        {
                            var li = new LeadItem
                            {
                                LeadId = leadEntity.Id,
                                ProductId = pi.ProductId ?? 0,
                                Quantity = pi.Quantity,
                                UnitPrice = pi.UnitPrice,
                                TaxPercentage = pi.TaxPercentage,
                                LineTotal = pi.Quantity * pi.UnitPrice * (1 + (pi.TaxPercentage / 100M))
                            };
                            await _uow.LeadItems.AddAsync(li);
                        }
                    }

                    ////////////////////// For user rights for the lead ////////////////////////
                    ///
                    if (leadDto.UserLeadRights != null && leadDto.UserLeadRights.Any())
                    {
                        foreach (var ur in leadDto.UserLeadRights)
                        {
                            // prevent duplicate rights for same user+lead within this create
                            var existing = (await _uow.UserLeadRights.FindAsync(x => x.UserId == ur.UserId && x.LeadId == leadEntity.Id)).FirstOrDefault();
                            if (existing != null)
                            {
                                existing.CanEdit = ur.CanEdit;
                                existing.CanView = ur.CanEdit || ur.CanView;
                                _uow.UserLeadRights.Update(existing);
                            }
                            else
                            {
                                var ulr = new UserLeadRights
                                {
                                    UserId = ur.UserId,
                                    LeadId = leadEntity.Id,
                                    CanEdit = ur.CanEdit,
                                    // enforce rule: edit implies view
                                    CanView = (ur.CanView || ur.CanEdit)
                                };
                                await _uow.UserLeadRights.AddAsync(ulr);
                            }
                        }
                    }

                    ///////////// Save items/rights ////////////////
                    await _uow.CommitAsync();

                    // Persist comments submitted with lead (if any)
                    try
                    {
                        if (leadDto.Comments != null && leadDto.Comments.Any())
                        {
                            var userIdStr = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            int.TryParse(userIdStr, out var currentUserId);
                            foreach (var c in leadDto.Comments)
                            {
                                if (string.IsNullOrWhiteSpace(c.Content)) continue;
                                var com = new seashore_CRM.Models.Entities.Comment
                                {
                                    LeadId = leadEntity.Id,
                                    CommentText = c.Content,
                                    CreatedById = c.UserId > 0 ? c.UserId : (currentUserId == 0 ? (int?)null : currentUserId)
                                };
                                await _uow.Comments.AddAsync(com);
                            }
                            await _uow.CommitAsync();
                        }
                    }
                    catch { }

                    // Record initial status activity so lead history includes the starting status
                    try
                    {
                        // add LeadStatusActivity record
                        if (leadEntity.StatusId.HasValue)
                        {
                            var status = await _uow.LeadStatuses.GetByIdAsync(leadEntity.StatusId.Value);
                            // If user selected an activity template, record that activity; otherwise record a generic "Status Set" activity
                            if (leadDto.ActivityId.HasValue)
                            {
                                var template = await _uow.LeadStatusActivities.GetByIdAsync(leadDto.ActivityId.Value);
                                if (template != null)
                                {
                                    var templAct = new LeadStatusActivity
                                    {
                                        LeadId = leadEntity.Id,
                                        ActivityName = template.ActivityName,
                                        NextFollowUpDate = null,
                                        ActivityDate = DateTime.UtcNow,
                                        CreatedById = null,
                                        LeadStatusId = template.LeadStatusId
                                    };
                                    await _uow.LeadStatusActivities.AddAsync(templAct);
                                    await _uow.CommitAsync();

                                    // persist chosen template id on lead record (ensure DB has reference)
                                    leadEntity.ActivityId = template.Id;
                                    _uow.Leads.Update(leadEntity);
                                    await _uow.CommitAsync();
                                }
                                else
                                {
                                    var initAct = new LeadStatusActivity
                                    {
                                        LeadId = leadEntity.Id,
                                        ActivityName = "Status Set",
                                        NextFollowUpDate = null,
                                        ActivityDate = DateTime.UtcNow,
                                        CreatedById = null,
                                        LeadStatusId = status?.Id ?? 0
                                    };
                                    await _uow.LeadStatusActivities.AddAsync(initAct);
                                    await _uow.CommitAsync();
                                    // ensure ActivityId cleared if template not found
                                    leadEntity.ActivityId = null;
                                    _uow.Leads.Update(leadEntity);
                                    await _uow.CommitAsync();
                                }
                            }
                            else
                            {
                                var initAct = new LeadStatusActivity
                                {
                                    LeadId = leadEntity.Id,
                                    ActivityName = "Status Set",
                                    NextFollowUpDate = null,
                                    ActivityDate = DateTime.UtcNow,
                                    CreatedById = null,
                                    LeadStatusId = status?.Id ?? 0
                                };
                                await _uow.LeadStatusActivities.AddAsync(initAct);
                                await _uow.CommitAsync();

                                // clear ActivityId when no template selected
                                leadEntity.ActivityId = null;
                                _uow.Leads.Update(leadEntity);
                                await _uow.CommitAsync();
                            }

                            // also add structured LeadHistory entry
                            var hist = new LeadHistory
                            {
                                LeadId = leadEntity.Id,
                                Type = LeadHistoryType.Created,
                                FieldName = "StatusId",
                                OldValue = null,
                                NewValue = leadEntity.StatusId?.ToString(),
                                OldStatusId = null,
                                OldStatusName = null,
                                NewStatusId = status?.Id,
                                NewStatusName = status?.StatusName,
                                ChangedAt = DateTime.UtcNow
                            };
                            await _uow.LeadHistories.AddAsync(hist);
                            await _uow.CommitAsync();
                        }
                    }
                    catch { }

                    // Commit transaction
                    await _uow.CommitTransactionAsync(tx);
                    await tx.DisposeAsync();

                    // Log user activity: Created Lead
                    var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                    var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? string.Empty;
                    await _activityService.LogEntityActionAsync(userId, userName, "Created", "Lead", leadEntity.Id.ToString(), JsonSerializer.Serialize(leadDto), _httpContextAccessor?.HttpContext?.TraceIdentifier);

                    return ("Success", leadEntity.Id);
                }
                catch
                {
                    if (tx != null)
                    {
                        try { await _uow.RollbackTransactionAsync(tx); } catch { }
                        try { await tx.DisposeAsync(); } catch { }
                    }
                    throw;
                }
            }
            catch (ValidationException vex)
            {
                var msg = string.Join("; ", vex.Errors.Select(e => e.ErrorMessage));
                return (msg, null);
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        public async Task DeleteLeadAsync(int id)
        {
            var lead = await _uow.Leads.GetByIdAsync(id);
            if (lead == null) return;
            _uow.Leads.Remove(lead);
            await _uow.CommitAsync();
        }

        public async Task<LeadDto?> GetLeadByIdAsync(int id)
        {
            var l = await _uow.Leads.GetByIdAsync(id);
            if (l == null) return null;

            var dto = new LeadDto
            {
                Id = l.Id,
                LeadType = l.LeadType,
                CompanyId = l.CompanyId,
                ContactId = l.ContactId,
                IndividualCustomerId = l.IndividualCustomerId,
                SourceId = l.SourceId,
                StatusId = l.StatusId,
                ActivityId = l.ActivityId,
                FollowUpTime = l.FollowUpTime,
                ActivityType = l.ActivityType,
                AttachmentsJson = l.AttachmentsJson,
                Priority = l.Priority,
                AssignedUserId = l.AssignedUserId,
                IsQualified = l.IsQualified,
                QualificationNotes = l.QualificationNotes,
                Budget = l.Budget,
                DecisionDate = l.DecisionDate,
                Probability = l.Probability,
                UpdatedDate = l.UpdatedDate,
                ExpectedClosureDate = l.ExpectedClosureDate
            };

            dto.FollowUpDate = l.FollowUpDate;
            dto.FollowUpTime = l.FollowUpTime;

            // Populate customer display fields (Company > Contact > Individual)
            if (l.CompanyId.HasValue && l.Company != null)
            {
                dto.CustomerName = l.Company.CompanyName;
                dto.CustomerLocation = l.Company.City;
            }
            else if (l.ContactId.HasValue && l.Contact != null)
            {
                dto.CustomerName = !string.IsNullOrWhiteSpace(l.Contact.ContactName) ? l.Contact.ContactName : (l.Contact.Email ?? l.Contact.Mobile ?? l.LeadType);
                dto.CustomerLocation = !string.IsNullOrWhiteSpace(l.Contact.Mobile) ? l.Contact.Mobile : l.Contact.Phone;
            }
            else if (l.IndividualCustomerId.HasValue && l.IndividualCustomer != null)
            {
                dto.CustomerName = l.IndividualCustomer.Name;
                dto.CustomerLocation = l.IndividualCustomer.Location;
            }
            else
            {
                dto.CustomerName = l.LeadType;
            }

            if (l.StatusId.HasValue && l.Status != null)
            {
                dto.StatusName = l.Status.StatusName;
            }

            if (l.AssignedUserId.HasValue && l.AssignedUser != null)
            {
                dto.AssignedUserName = l.AssignedUser.FullName;
            }

            // Map product items from included navigation to DTO so UI can pre-populate product rows
            var itemsNav = l.LeadItems?.ToList() ?? new List<LeadItem>();
            if (itemsNav.Any())
            {
                dto.ProductItems = new List<LeadProductDto>();
                foreach (var it in itemsNav)
                {
                    var p = it.Product; // product may be included by repository
                    var itemDto = new LeadProductDto
                    {
                        // LeadProductId should refer to LeadItem.Id so updates/deletes target the correct row
                        LeadProductId = it.Id,
                        ProductId = it.ProductId,
                        ProductName = p?.ProductName,
                        Quantity = it.Quantity,
                        UnitPrice = it.UnitPrice,
                        TaxPercentage = it.TaxPercentage,
                        CategoryId = p?.CategoryId,
                        CategoryName = p?.Category?.CategoryName,
                        ProductGroup = p?.ProductGroup?.GroupName,
                        SaleValue = it.UnitPrice * it.Quantity,
                        TaxValue = it.UnitPrice * it.Quantity * (it.TaxPercentage / 100M),
                        GrossTotal = it.LineTotal,
                        Cost = p?.Cost ?? 0,
                        GrossProfit = it.LineTotal - ((p?.Cost ?? 0) * it.Quantity)
                    };
                    dto.ProductItems.Add(itemDto);
                }
                // If lead-level CategoryId is needed by UI, pick the first product's category as a reasonable default
                dto.CategoryId = itemsNav.First().Product?.CategoryId;
            }
            else
            {
                dto.ProductItems = new List<LeadProductDto>();
            }

            var comments = (await _uow.Comments.FindAsync(c => c.LeadId == l.Id)).OrderByDescending(c => c.CreatedDate).ToList();
            if (comments.Any())
            {
                dto.Comments = comments.Select(c => new CommentDto
                {
                    LeadId = c.LeadId ?? 0,
                    UserId = c.CreatedById ?? 0,
                    Content = c.CommentText,
                    CreatedAt = c.CreatedDate
                }).ToList();
            }

            var activities = (await _uow.LeadStatusActivities.FindAsync(a => a.LeadId == l.Id)).OrderByDescending(a => a.ActivityDate).ToList();
            if (activities.Any()) dto.LatestActivity = activities.First().ActivityName;

            return dto;
        }

        public async Task UpdateLeadAsync(LeadDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (_validator != null)
            {
                var v = await _validator.ValidateAsync(dto);
                if (!v.IsValid) throw new ValidationException(v.Errors);
            }

            var lead = await _uow.Leads.GetByIdAsync(dto.Id);
            if (lead == null) throw new InvalidOperationException("Lead not found");

            lead.LeadType = dto.LeadType;
            lead.CompanyId = dto.CompanyId;
            lead.ContactId = dto.ContactId;
            lead.IndividualCustomerId = dto.IndividualCustomerId ?? (dto.LeadType == "Individual" ? dto.ContactId : null);
            lead.SourceId = dto.SourceId;
            lead.ActivityId = dto.ActivityId;
            var prevStatusId = lead.StatusId;
            lead.StatusId = dto.StatusId;
            lead.Priority = dto.Priority;
            lead.AssignedUserId = dto.AssignedUserId;
            lead.FollowUpDate = dto.FollowUpDate;
            lead.FollowUpTime = dto.FollowUpTime;
            lead.ActivityType = dto.ActivityType;
            lead.AttachmentsJson = dto.AttachmentsJson;
            lead.ExpectedClosureDate = dto.ExpectedClosureDate;

            lead.IsQualified = dto.IsQualified;
            lead.QualificationNotes = dto.QualificationNotes;

            lead.Budget = dto.Budget;
            lead.DecisionDate = dto.DecisionDate;
            lead.Probability = dto.Probability;

            // update lead core
            _uow.Leads.Update(lead);
            await _uow.CommitAsync();

            // If product items were submitted, replace existing LeadItems with the new set
            try
            {

                var existingItems = await _uow.LeadItems.FindAsync(x => x.LeadId == dto.Id);

                var existingDict = existingItems.ToDictionary(x => x.Id);

                // Loop Submitted Items

                foreach (var item in dto.ProductItems)
                {
                    if (item.ProductId.HasValue && existingDict.TryGetValue(item.ProductId.Value, out var existingItem))
                    {
                        // Update existing item
                        existingItem.ProductId = item.ProductId ?? existingItem.ProductId;
                        existingItem.Quantity = item.Quantity;
                        existingItem.UnitPrice = item.UnitPrice;
                        existingItem.TaxPercentage = item.TaxPercentage;
                        existingItem.LineTotal = item.Quantity * item.UnitPrice * (1 + (item.TaxPercentage / 100M));
                        _uow.LeadItems.Update(existingItem);
                    }
                    else
                    {
                        // Add new item
                        var newItem = new LeadItem
                        {
                            LeadId = dto.Id,
                            ProductId = item.ProductId ?? 0,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            TaxPercentage = item.TaxPercentage,
                            LineTotal = item.Quantity * item.UnitPrice * (1 + (item.TaxPercentage / 100M))
                        };
                        await _uow.LeadItems.AddAsync(newItem);
                    }
                }

                // Delete items that were not included in the submitted list (i.e. removed by user in UI)

                var submittedProductIds = dto.ProductItems.Where(i => i.ProductId.HasValue).Select(i => i.ProductId.Value).ToHashSet();
                foreach (var existing in existingItems)
                {
                    if (!submittedProductIds.Contains(existing.ProductId))
                    {
                        _uow.LeadItems.Remove(existing);
                    }
                }



                await _uow.CommitAsync();
            }

            catch(Exception ex)
            {
                //// Log error but continue with lead update - item update is best-effort
                //var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                //var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? string.Empty;
                //await _activityService.LogErrorAsync(userId, userName, "Error updating lead items: " + ex.Message, "Lead", lead.Id.ToString(), _httpContextAccessor?.HttpContext?.TraceIdentifier);
            }

            // If status changed, record a status-change activity for history
            try
            {
                if (prevStatusId != lead.StatusId)
                {
                    string prevName = null, newName = null;
                    if (prevStatusId.HasValue) {
                        var ps = await _uow.LeadStatuses.GetByIdAsync(prevStatusId.Value);
                        prevName = ps?.StatusName;
                    }
                    if (lead.StatusId.HasValue) {
                        var ns = await _uow.LeadStatuses.GetByIdAsync(lead.StatusId.Value);
                        newName = ns?.StatusName;
                    }

                    var statusChangeAct = new LeadStatusActivity
                    {
                        LeadId = lead.Id,
                        ActivityName = "Status Change",
                        NextFollowUpDate = null,
                        ActivityDate = DateTime.UtcNow,
                        CreatedById = null,
                        LeadStatusId = lead.StatusId ?? 0
                    };
                    await _uow.LeadStatusActivities.AddAsync(statusChangeAct);
                    await _uow.CommitAsync();

                    // write structured history
                    var hist = new LeadHistory
                    {
                        LeadId = lead.Id,
                        Type = LeadHistoryType.StatusChange,
                        FieldName = "StatusId",
                        OldValue = prevStatusId?.ToString(),
                        NewValue = lead.StatusId?.ToString(),
                        OldStatusId = prevStatusId,
                        OldStatusName = prevName,
                        NewStatusId = lead.StatusId,
                        NewStatusName = newName,
                        ChangedAt = DateTime.UtcNow
                    };
                    await _uow.LeadHistories.AddAsync(hist);
                    await _uow.CommitAsync();
                }
            }
            catch { }

            // persist comments submitted during update
            try
            {
                if (dto.Comments != null && dto.Comments.Any())
                {
                    var userIdStr = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    int.TryParse(userIdStr, out var currentUserId);

                    foreach (var c in dto.Comments)
                    {
                        if (string.IsNullOrWhiteSpace(c.Content)) continue;
                        var com = new seashore_CRM.Models.Entities.Comment
                        {
                            LeadId = lead.Id,
                            CommentText = c.Content,
                            CreatedById = c.UserId > 0 ? c.UserId : (currentUserId == 0 ? (int?)null : currentUserId)
                        };
                        await _uow.Comments.AddAsync(com);
                    }
                    await _uow.CommitAsync();
                }
            }
            catch { }

            if (dto.SelectedActivities != null && dto.SelectedActivities.Any())
            {
                foreach (var at in dto.SelectedActivities)
                {
                    if (string.IsNullOrWhiteSpace(at)) continue;
                    var a = new LeadStatusActivity
                    {
                        LeadId = dto.Id,
                        ActivityName = at,
                        ActivityDate = DateTime.UtcNow
                    };
                    await _uow.LeadStatusActivities.AddAsync(a);
                }
                await _uow.CommitAsync();
            }
        }

        // DTO-focused data builder for create/edit views
        public async Task<LeadCreateDataDto> BuildLeadCreateDataAsync(int? selectedCategoryId = null, int? selectedCompanyId = null, int? selectedStatusId = null)
        {
            var data = new LeadCreateDataDto();

            var companiesQueryable = _uow.Companies.GetAllExceptInactive();
            var companies = await companiesQueryable.ToListAsync();
            data.Companies = companies.Select(c => new OptionDto { Id = c.Id, Name = c.CompanyName }).ToList();

            var contactsEn = await _uow.Contacts.GetActiveByCompanyIdAsync(selectedCompanyId);
            data.Contacts = contactsEn.Select(c => 
            new OptionDto 
            { 
                Id = c.Id, 
                Name = !string.IsNullOrWhiteSpace(c.ContactName) ? c.ContactName : (c.Email ?? c.Mobile ?? "(no name)") 
            }).ToList();

            var indContactsQueryable = _uow.IndividualCustomers.GetAllExceptInactive();
            var indContacts = await indContactsQueryable.ToListAsync();
            data.ContactForIndv = indContacts.Select(c => new OptionDto { Id = c.Id, Name = c.Name }).ToList();

            var sources = await _uow.LeadSources.GetAllAsync();
            data.Sources = sources.Select(s => new OptionDto { Id = s.Id, Name = s.SourceName }).ToList();

            var statuses = await _uow.LeadStatuses.GetAllAsync();
            data.Statuses = statuses.Select(s => new OptionDto { Id = s.Id, Name = s.StatusName }).ToList();

            var usersQueryable = _uow.Users.GetAllAsync();
            var usersList = await usersQueryable.ToListAsync();
            // ensure distinct users by Id in case repository returns duplicates
            data.Users = usersList.GroupBy(u => u.Id).Select(g => g.First()).Select(u => new OptionDto { Id = u.Id, Name = u.FullName }).ToList();

            var productsQueryable = _uow.Products.GetAllExceptInactive();
            var productsList = await productsQueryable.ToListAsync();
            // ensure distinct products
            var products = productsList.GroupBy(p => p.Id).Select(g => g.First()).ToList();

            var categories = await _uow.Categories.GetAllAsync();
            var categoriesList = categories.ToList();
            var categoriesDict = categoriesList.ToDictionary(c => c.Id, c => c.CategoryName);
            var productGroups = (await _uow.ProductGroups.GetAllAsync()).ToList();
            var groupsDict = productGroups.ToDictionary(g => g.Id, g => g.GroupName);

            data.ProductList = products.Select(p => new ProductOptionDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Cost = p.Cost,
                TaxPercentage = p.TaxPercentage,
                CategoryId = p.CategoryId,
                ProductGroupId = p.ProductGroupId,
                ProductGroupName = p.ProductGroupId.HasValue && groupsDict.TryGetValue(p.ProductGroupId.Value, out var gp) ? gp : null,
                CategoryName = categoriesDict.TryGetValue(p.CategoryId, out var cn) ? cn : null
            }).ToList();

            data.Categories = categoriesList.Select(c => new OptionDto { Id = c.Id, Name = c.CategoryName }).ToList();

            data.ProductGroups = (await _uow.ProductGroups.GetByCategoryIdAsync(selectedCategoryId)).Select(g => new OptionDto { Id = g.Id, Name = g.GroupName }).ToList();

            // previously used product id as key which client JS expects product name as lookup key.
            // create products map keyed by product name so client-side productsMap[productName] works
            // Build combined maps to handle duplicate product names safely and support migration on client side.
            var productsById = data.ProductList.ToDictionary(
                p => p.Id.ToString(),
                p => new {
                    id = p.Id,
                    name = p.ProductName,
                    cost = p.Cost,
                    tax = p.TaxPercentage,
                    categoryId = p.CategoryId,
                    categoryName = p.CategoryName,
                    productGroupName = p.ProductGroupName
                });

            var groupsByName = data.ProductList
                .GroupBy(p => string.IsNullOrWhiteSpace(p.ProductName) ? p.Id.ToString() : p.ProductName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(p => new {
                        id = p.Id,
                        name = p.ProductName,
                        cost = p.Cost,
                        tax = p.TaxPercentage,
                        categoryId = p.CategoryId,
                        categoryName = p.CategoryName,
                        productGroupName = p.ProductGroupName
                    }).ToList()
                );

            // legacy single-item map (first item) to preserve existing client behavior while migrating
            var legacyMap = groupsByName.ToDictionary(kv => kv.Key, kv => (object)kv.Value.First());

            var combined = new {
                byId = productsById,
                byName = groupsByName,
                legacy = legacyMap
            };

            data.ProductsJson = JsonSerializer.Serialize(combined);

            data.CommentTemplates = new List<string> { "Need more information", "Sent quote", "Followed up", "Client requested sample", "Waiting for approval" };

            data.StatusActivitiesMapping = await GetStatusActivitiesAsync();

            return data;
        }

        public async Task<IEnumerable<OptionDto>> GetContactsByCompanyAsync(int companyId)
        {
            if (companyId <= 0) return Enumerable.Empty<OptionDto>();
            var contacts = await _uow.Contacts.GetByCompanyIdAsync(companyId);
            return contacts.Select(c => new OptionDto { Id = c.Id, Name = !string.IsNullOrWhiteSpace(c.ContactName) ? c.ContactName : (c.Email ?? c.Mobile ?? "(no name)") });
        }

        public async Task<IEnumerable<OptionDto>> GetProductGroupsByCategoryAsync(int categoryId)
        {
            if (categoryId <= 0) return Enumerable.Empty<OptionDto>();
            var groups = await _uow.ProductGroups.GetByCategoryIdAsync(categoryId);
            return groups.Select(g => new OptionDto { Id = g.Id, Name = g.GroupName });
        }

        public async Task<IEnumerable<OptionDto>> GetActivitiesByStatusAsync(int statusId)
        {
            if (statusId <= 0) return Enumerable.Empty<OptionDto>();
            var activities = await _uow.LeadStatusActivities.FindAsync(a => a.LeadStatusId == statusId);
            return activities.Select(a => new OptionDto { Id = a.Id, Name = a.ActivityName });
        }

        public async Task<Dictionary<string, string[]>> GetStatusActivitiesAsync()
        {
            var mapping = new Dictionary<string, string[]>();
            var statuses = (await _uow.LeadStatuses.GetAllAsync()).ToList();
            if (statuses.Any())
            {
                foreach (var st in statuses)
                {
                    var acts = (await _uow.LeadStatusActivities.FindAsync(a => a.LeadStatusId == st.Id)).Select(a => a.ActivityName).ToArray();
                    if (acts.Length > 0) mapping[st.StatusName] = acts;
                }
            }
            return mapping;
        }

        public async Task<IEnumerable<LeadStatusActivity>> GetActivitiesByLeadAsync(int leadId)
        {
            return (await _uow.LeadStatusActivities.FindAsync(a => a.LeadId == leadId)).OrderByDescending(a => a.ActivityDate).ToList();
        }

        public async Task<IEnumerable<LeadHistory>> GetHistoryByLeadAsync(int leadId)
        {
            var history = (await _uow.LeadHistories.FindAsync(h => h.LeadId == leadId)).OrderByDescending(h => h.ChangedAt).ToList();
            return history;
        }

        public async Task<IEnumerable<seashore_CRM.Models.Entities.Comment>> GetCommentsByLeadAsync(int leadId)
        {
            return (await _uow.Comments.FindAsync(c => c.LeadId == leadId)).OrderByDescending(c => c.CreatedDate).ToList();
        }

        public async Task<Dictionary<string, string[]>> AddStatusActivityAsync(string statusName, string activityName)
        {
            if (string.IsNullOrWhiteSpace(statusName) || string.IsNullOrWhiteSpace(activityName)) return await GetStatusActivitiesAsync();

            var status = (await _uow.LeadStatuses.FindAsync(s => s.StatusName == statusName)).FirstOrDefault();
            if (status == null)
            {
                status = new seashore_CRM.Models.Entities.LeadStatus { StatusName = statusName };
                await _uow.LeadStatuses.AddAsync(status);
                await _uow.CommitAsync();
            }

            var existing = (await _uow.LeadStatusActivities.FindAsync(a => a.LeadStatusId == status.Id && a.ActivityName == activityName)).FirstOrDefault();
            if (existing == null)
            {
                var act = new seashore_CRM.Models.Entities.LeadStatusActivity { LeadStatusId = status.Id, ActivityName = activityName };
                await _uow.LeadStatusActivities.AddAsync(act);
                await _uow.CommitAsync();
            }

            return await GetStatusActivitiesAsync();
        }

        public async Task AddActivitiesToLeadAsync(int leadId, IEnumerable<string> activities)
        {
            if (activities == null) return;
            foreach (var at in activities)
            {
                if (string.IsNullOrWhiteSpace(at)) continue;
                var a = new LeadStatusActivity
                {
                    LeadId = leadId,
                    ActivityName = at,
                    ActivityDate = DateTime.UtcNow
                };
                await _uow.LeadStatusActivities.AddAsync(a);
            }
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<seashore_CRM.BLL.DTOs.UserLeadRightDto>> GetUserLeadRightsAsync(int leadId)
        {
            var rightsEnumerable = await _uow.UserLeadRights.FindAsync(r => r.LeadId == leadId);
            var rights = rightsEnumerable.ToList();

            // _uow.Users.GetAllAsync() returns IQueryable<User>, use ToListAsync()
            var users = await _uow.Users.GetAllAsync().ToListAsync();
            var map = users.ToDictionary(u => u.Id, u => u.FullName);

            return rights.Select(r => new seashore_CRM.BLL.DTOs.UserLeadRightDto
            {
                Id = r.Id,
                UserId = r.UserId ?? 0,
                LeadId = r.LeadId ?? 0,
                UserName = (r.UserId.HasValue && map.ContainsKey(r.UserId.Value)) ? map[r.UserId.Value] : null,
                CanView = r.CanView,
                CanEdit = r.CanEdit
            }).ToList();
        }
    }
}
