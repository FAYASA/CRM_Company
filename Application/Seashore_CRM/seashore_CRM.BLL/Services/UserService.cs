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

        public async Task<IEnumerable<UserListDto>> GetAllAsync()
        {
            var users = await _uow.Users.GetAllAsync();
            return users.Select(u => new UserListDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Contact = u.Contact,
                Region = u.Region,
                IsActive = u.IsActive,
                ReportToUserId = u.ReportToUserId,
                ReportToName = u.ReportToUser != null ? u.ReportToUser.FullName : null,
                Role = u.Role != null ? new List<string> { u.Role.RoleName } : new List<string>(),
                // populate PasswordHash and RoleId for login and controller needs
                PasswordHash = u.PasswordHash,
                RoleId = u.RoleId
            }).ToList();
        }

        public async Task<UserDetailDto?> GetByIdAsync(int id)
        {
            
            var users = await _uow.Users.GetByIdAsync(id);

            return users == null ? null : new UserDetailDto
            {
                Id = users.Id,
                FullName = users.FullName,
                Email = users.Email,
                Contact = users.Contact,
                Region = users.Region,
                Designation = users.Designation,
                IsActive = users.IsActive,
                ReportToUserId = users.ReportToUserId,
                ReportToName = users.ReportToUser != null ? users.ReportToUser.FullName : null,
                Roles = users.Role != null ? new List<string> { users.Role.RoleName } : new List<string>(),
                // populate PasswordHash and RoleId for login and controller needs
                PasswordHash = users.PasswordHash,
                RoleId = users.RoleId
            };
        }

        public async Task UpdateAsync(UserUpdateDto dto)
        {
            var users = await _uow.Users.GetByIdAsync(dto.Id);
            if (users == null) return;
            users.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                users.PasswordHash = _passwordHasher.HashPassword(users, dto.NewPassword);
            }
            users.RoleId = dto.RoleId;
            users.ReportToUserId = dto.ReportToUserId;
            users.FullName = dto.FullName;
            users.Contact = dto.Contact;
            users.Region = dto.Region;
            users.Designation = dto.Designation;
            users.IsActive = dto.IsActive;
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

        public async Task<ProfileViewDto?> GetProfileAsync(int userId)
        {
            var user = await _uow.Users.GetByIdAsync(userId);

            if (user == null) return null;

            return new ProfileViewDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Contact = user.Contact,
                Region = user.Region,
                RoleName = user.Role?.RoleName ?? "",
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };
        }

        public async Task UpdateProfileAsync(ProfileUpdateDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(dto.Id);
            if (user == null)
                throw new Exception("User not found");

            // Check if email is already taken by another user
            var emailTaken = await IsEmailTakenAsync(dto.Email, dto.Id);
            if (emailTaken)
                throw new Exception($"Email '{dto.Email}' is already in use.");

            var fullNameTaken = await IsFullNameTakenAsync(dto.FullName, dto.Id);
            if (fullNameTaken)
                throw new Exception($"Full Name '{dto.FullName}' is already in use.");

            var contactTaken = !string.IsNullOrWhiteSpace(dto.Contact)
                && await IsContactTakenAsync(dto.Contact, dto.Id);
            if (contactTaken)
                throw new Exception($"Contact '{dto.Contact}' is already in use.");

            // Safe to update
            user.Email = dto.Email;
            user.FullName = dto.FullName;
            user.Contact = dto.Contact;
            user.Region = dto.Region;
            user.UpdatedDate = DateTime.UtcNow;

            _uow.Users.Update(user);
            await _uow.CommitAsync();
        }

        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception("User not found");

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.CurrentPassword
            );

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Current password is incorrect");

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            user.UpdatedDate = DateTime.UtcNow;

            _uow.Users.Update(user);
            await _uow.CommitAsync();
        }
    }
}
