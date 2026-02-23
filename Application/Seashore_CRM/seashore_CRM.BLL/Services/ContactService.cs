using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _uow;

        public ContactService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ===============================
        // GET ALL CONTACTS
        // ===============================
        public async Task<IEnumerable<ContactListDto>> GetAllAsync()
        {
            var contacts = await _uow.Contacts.GetAllAsync();
            return contacts.Select(c => new ContactListDto
            {
                Id = c.Id,
                ContactName = c.Contact_Name,
                Email = c.Email,
                Phone = c.Phone,
                Mobile = c.Mobile,
                Designation = c.Designation,
                CompanyName = c.Company?.CompanyName,
                IsActive = c.IsActive
            });
        }

        // ===============================
        // GET CONTACT BY ID
        // ===============================
        public async Task<ContactDetailDto?> GetByIdAsync(int id)
        {
            var c = await _uow.Contacts.GetByIdAsync(id);
            if (c == null) return null;

            return new ContactDetailDto
            {
                Id = c.Id,
                ContactName = c.Contact_Name,
                Email = c.Email,
                Phone = c.Phone,
                Mobile = c.Mobile,
                Designation = c.Designation,
                CompanyId = c.Company.Id,
                CompanyName = c.Company?.CompanyName,
                IsActive = c.IsActive,
                RowVersion = c.RowVersion
            };
        }

        // ===============================
        // CREATE CONTACT
        // ===============================
        public async Task<int> CreateAsync(ContactCreateDto dto)
        {
            var entity = new Contact
            {
                CompanyId = dto.CompanyId,
                Contact_Name = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                Mobile = dto.Mobile,
                Designation = dto.Designation,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _uow.Contacts.AddAsync(entity);
            await _uow.CommitAsync();

            return entity.Id;
        }

        // ===============================
        // UPDATE CONTACT
        // ===============================
        public async Task UpdateAsync(ContactUpdateDto dto)
        {
            var entity = await _uow.Contacts.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Contact not found");

            // Concurrency check
            if (!entity.RowVersion.SequenceEqual(dto.RowVersion))
                throw new InvalidOperationException("The contact has been modified by another user.");

            entity.CompanyId = dto.CompanyId;
            entity.Contact_Name = dto.ContactName;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Mobile = dto.Mobile;
            entity.Designation = dto.Designation;
            entity.IsActive = dto.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;

            _uow.Contacts.Update(entity);
            await _uow.CommitAsync();
        }

        // ===============================
        // DELETE CONTACT
        // ===============================
        public async Task DeleteAsync(int id)
        {
            var entity = await _uow.Contacts.GetByIdAsync(id);
            if (entity == null) return;

            _uow.Contacts.Remove(entity);
            await _uow.CommitAsync();
        }
    }
}