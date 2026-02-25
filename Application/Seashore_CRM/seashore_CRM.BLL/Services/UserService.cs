using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;


namespace seashore_CRM.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ToggleStatusAsync(int id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null) return;

            user.IsActive = !user.IsActive;
            await _uow.CommitAsync();
        }
        public async Task<int> CreateAsync(UserCreateDto dto)
        {
            var entity = new User
            {
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(null, dto.Password),
                RoleId = dto.RoleId,
                ReportToUserId = dto.ReportToUserId,
                FullName = dto.FullName,
                Contact = dto.Contact,
                Region = dto.Region,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.UtcNow
            };

            await _uow.Users.AddAsync(entity);
            await _uow.CommitAsync();

            return entity.Id;
        }


        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Users.GetByIdAsync(id);
            if (e == null) return;
            _uow.Users.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _uow.Users.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _uow.Users.GetByIdAsync(id);
        }

        public async Task UpdateAsync(User entity)
        {
            _uow.Users.Update(entity);
            await _uow.CommitAsync();
        }

        // Validation helpers
        public async Task<bool> IsEmailTakenAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var all = await _uow.Users.GetAllAsync();
            return all.Any(u => u.Email.ToLower().Trim() == email.ToLower().Trim() && (!excludeId.HasValue || u.Id != excludeId.Value));
        }

        public async Task<bool> IsFullNameTakenAsync(string fullName, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return false;
            var all = await _uow.Users.GetAllAsync();
            return all.Any(u => u.FullName.ToLower().Trim() == fullName.ToLower().Trim() && (!excludeId.HasValue || u.Id != excludeId.Value));
        }

        public async Task<bool> IsContactTakenAsync(string contact, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(contact)) return false;
            var all = await _uow.Users.GetAllAsync();
            return all.Any(u => !string.IsNullOrWhiteSpace(u.Contact) && u.Contact!.ToLower().Trim() == contact.ToLower().Trim() && (!excludeId.HasValue || u.Id != excludeId.Value));
        }
    }
}
