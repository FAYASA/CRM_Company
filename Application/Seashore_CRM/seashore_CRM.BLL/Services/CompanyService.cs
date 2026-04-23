using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using System;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;

namespace seashore_CRM.BLL.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CompanyCreateDto> _createValidator;
        private readonly IValidator<CompanyUpdateDto> _updateValidator;
        private readonly IUserActivityService _activityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyService(IUnitOfWork uow, IValidator<CompanyCreateDto> createValidator, IValidator<CompanyUpdateDto> updateValidator, IUserActivityService activityService, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _activityService = activityService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable <CompanyListDto> GetAllExceptInactive()
        {
            var companies = _uow.Companies.GetAllExceptInactive();
            return companies.Select(c => new CompanyListDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Email = c.Email!,
                City = c.City!,
                //Country = c.Country!,
                Industry = c.Industry,
                IsActive = c.IsActive,
                Address = c.Address,
                AddressPost = c.AddressPost
            });
        }

        public IQueryable <CompanyListDto> GetAllAsync()
        {
            var companies = _uow.Companies.GetAllAsync();

            return companies.Select(c => new CompanyListDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Email = c.Email!,
                City = c.City!,
                //Country = c.Country!,
                Industry = c.Industry,
                IsActive = c.IsActive,
                Address = c.Address,
                AddressPost = c.AddressPost
            });
        }

        public async Task<CompanyDetailDto?> GetByIdAsync(int id)
        {
            var c = await _uow.Companies.GetByIdAsync(id);
            if (c == null) return null;

            return new CompanyDetailDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Address = c.Address,
                City = c.City,
                //Country = c.Country,
                Phone = c.Phone,
                Email = c.Email,
                Website = c.Website,
                IsActive = c.IsActive,
                Industry = c.Industry,
                AddressPost = c.AddressPost,
                Pin = c.Pin,
            };
        }

        public async Task<int> CreateAsync(CompanyCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var vResult = await _createValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>();
            if (!vResult.IsValid) failures.AddRange(vResult.Errors);

            if (await IsCompanyNameTakenAsync(dto.CompanyName))
                failures.Add(new ValidationFailure(nameof(dto.CompanyName), "Company name already exists."));

            if (!string.IsNullOrWhiteSpace(dto.Email) && await IsEmailTakenAsync(dto.Email))
                failures.Add(new ValidationFailure(nameof(dto.Email), "Email already exists."));

            if (!string.IsNullOrWhiteSpace(dto.Phone) && await IsCompanyPhoneTakenAsync(dto.Phone))
                failures.Add(new ValidationFailure(nameof(dto.Phone), "Phone number already exists."));

            if (failures.Any()) throw new ValidationException(failures);

            var entity = new Company
            {
                CompanyName = dto.CompanyName,
                Email = dto.Email,
                //Country = dto.Country,
                City = dto.City,
                Address = dto.Address,
                AddressPost = dto.AddressPost,
                Phone = dto.Phone,
                Website = dto.Website,
                Industry = dto.Industry,
                Pin = dto.Pin

            };

            await _uow.Companies.AddAsync(entity);
            await _uow.CommitAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(CompanyUpdateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var vResult = await _updateValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>();
            if (!vResult.IsValid) failures.AddRange(vResult.Errors);

            if (await IsCompanyNameTakenAsync(dto.CompanyName, dto.Id))
                failures.Add(new ValidationFailure(nameof(dto.CompanyName), "Company name already exists."));

            if (!string.IsNullOrWhiteSpace(dto.Email) && await IsEmailTakenAsync(dto.Email, dto.Id))
                failures.Add(new ValidationFailure(nameof(dto.Email), "Email already exists."));

            if (!string.IsNullOrWhiteSpace(dto.Phone) && await IsCompanyPhoneTakenAsync(dto.Phone, dto.Id))
                failures.Add(new ValidationFailure(nameof(dto.Phone), "Phone number already exists."));

            if (failures.Any()) throw new ValidationException(failures);

            var entity = await _uow.Companies.GetByIdAsync(dto.Id);
            if (entity == null) return false;

            entity.CompanyName = dto.CompanyName;
            entity.Email = dto.Email;
            //entity.Country = dto.Country;
            entity.City = dto.City;
            entity.Address = dto.Address;
            entity.Phone = dto.Phone;
            entity.Website = dto.Website;
            entity.IsActive = dto.IsActive;
            entity.Industry = dto.Industry;
            entity.Pin = dto.Pin;
            entity.AddressPost = dto.AddressPost;

            _uow.Companies.Update(entity);
            await _uow.CommitAsync();

            // log user activity : Updated Company
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? string.Empty;
            await _activityService.LogEntityActionAsync(userId, userName, "Updated", "Company", entity.Id.ToString(), JsonSerializer.Serialize(dto), _httpContextAccessor?.HttpContext?.TraceIdentifier);

            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _uow.Companies.GetByIdAsync(id);
            if (entity == null) return;

            _uow.Companies.SoftDelete(entity);
            await _uow.CommitAsync();
        }

        public IQueryable <CompanyListDto> SearchAsync(string? query)
        {
            var companies = string.IsNullOrWhiteSpace(query)
                ? _uow.Companies.GetAllAsync()
                : _uow.Companies.SearchAsync(query);

            return companies.Select(c => new CompanyListDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Email = c.Email!,
                City = c.City!,
                //Country = c.Country!,
                Industry = c.Industry,
                IsActive = c.IsActive,
                Address = c.Address,
                AddressPost = c.AddressPost
            });
        }

        // =========================
        // Validation helpers
        // =========================
        public async Task<bool> IsEmailTakenAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var all =  _uow.Companies.GetAllAsync();
            return all.Any(c => !string.IsNullOrWhiteSpace(c.Email)
                                && c.Email!.ToLower() == email.Trim().ToLower()
                                && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> IsCompanyNameTakenAsync(string companyName, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(companyName)) return false;
            var all = _uow.Companies.GetAllAsync();
            return all.Any(c => c.CompanyName.ToLower() == companyName.Trim().ToLower()
                                && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> IsCompanyPhoneTakenAsync(string companyPhone, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(companyPhone)) return false;
            var all = _uow.Companies.GetAllAsync();
            return all.Any(c => !string.IsNullOrWhiteSpace(c.Phone)
                                && c.Phone!.ToLower() == companyPhone.Trim().ToLower()
                                && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

    }
}