using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _uow;

        public CompanyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<CompanyListDto>> GetAllAsync()
        {
            var companies = await _uow.Companies.GetAllAsync();

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
            }).ToList();
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
            var entity = new Company
            {
                CompanyName = dto.CompanyName,
                Email = dto.Email,
                //Country = dto.Country,
                City = dto.City,
                Address = dto.Address,
                AddressPost = dto.AddressPost,
                CreatedBy = "System",
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

            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _uow.Companies.GetByIdAsync(id);
            if (entity == null) return;

            _uow.Companies.SoftDelete(entity);
            await _uow.CommitAsync();
        }

        public async Task<List<CompanyListDto>> SearchAsync(string? query)
        {
            var companies = string.IsNullOrWhiteSpace(query)
                ? await _uow.Companies.GetAllAsync()
                : await _uow.Companies.SearchAsync(query);

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
            }).ToList();
        }

        // =========================
        // Validation helpers
        // =========================
        public async Task<bool> IsEmailTakenAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var all = await _uow.Companies.GetAllAsync();
            return all.Any(c => !string.IsNullOrWhiteSpace(c.Email)
                                && c.Email!.ToLower() == email.Trim().ToLower()
                                && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> IsCompanyNameTakenAsync(string companyName, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(companyName)) return false;
            var all = await _uow.Companies.GetAllAsync();
            return all.Any(c => c.CompanyName.ToLower() == companyName.Trim().ToLower()
                                && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> IsCompanyPhoneTakenAsync(string companyPhone, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(companyPhone)) return false;
            var all = await _uow.Companies.GetAllAsync();
            return all.Any(c => !string.IsNullOrWhiteSpace(c.Phone)
                                && c.Phone!.ToLower() == companyPhone.Trim().ToLower()
                                && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

    }
}