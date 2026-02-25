using Microsoft.AspNetCore.Identity;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.DTOs;
using seashore_CRM.Common.Constants;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace seashore_CRM.BLL.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly UserManager<seashore_CRM.Models.Identity.ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserAppService(UserManager<seashore_CRM.Models.Identity.ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserListDto>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var list = new List<UserListDto>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var claims = await _userManager.GetClaimsAsync(u);
                var status = claims.FirstOrDefault(c => c.Type == "Status")?.Value;

                list.Add(new UserListDto
                {
                    Id = u.Id,
                    UserName = u.UserName ?? u.Email ?? "",
                    Email = u.Email ?? "",
                    Roles = roles.ToList(),
                    Status = int.TryParse(status, out var s) ? (UserStatus)s : UserStatus.Active,
                    Region = claims.FirstOrDefault(c => c.Type == "Region")?.Value,
                    IsActive = bool.TryParse(claims.FirstOrDefault(c => c.Type == "IsActive")?.Value, out var ia) ? ia : true,
                    ReportToUserId = claims.FirstOrDefault(c => c.Type == "ReportToUserId")?.Value
                });
            }
            return list;
        }

        public async Task<UserDetailDto?> GetByIdAsync(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return null;

            var claims = await _userManager.GetClaimsAsync(u);
            var roles = await _userManager.GetRolesAsync(u);

            return new UserDetailDto
            {
                Id = u.Id,
                UserName = u.UserName ?? u.Email ?? "",
                Email = u.Email ?? "",
                FullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value,
                Contact = claims.FirstOrDefault(c => c.Type == "Contact")?.Value,
                Region = claims.FirstOrDefault(c => c.Type == "Region")?.Value,
                Designation = claims.FirstOrDefault(c => c.Type == "Designation")?.Value,
                Roles = roles.ToList(),
                Status = int.TryParse(claims.FirstOrDefault(c => c.Type == "Status")?.Value, out var s) ? (UserStatus)s : UserStatus.Active,
                IsActive = bool.TryParse(claims.FirstOrDefault(c => c.Type == "IsActive")?.Value, out var ia) ? ia : true,
                ReportToUserId = claims.FirstOrDefault(c => c.Type == "ReportToUserId")?.Value
            };
        }

        public async Task<string> CreateAsync(UserCreateDto dto)
        {
            // validate dto
            var validator = new seashore_CRM.BLL.Validators.UserCreateDtoValidator();
            var vresult = await validator.ValidateAsync(dto);
            if (!vresult.IsValid) return string.Empty;

            var user = new seashore_CRM.Models.Identity.ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return string.Empty;

            if (!string.IsNullOrEmpty(dto.Role) && await _roleManager.RoleExistsAsync(dto.Role))
            {
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            // claims
            if (!string.IsNullOrEmpty(dto.FullName)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FullName", dto.FullName));
            if (!string.IsNullOrEmpty(dto.Contact)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Contact", dto.Contact));
            if (!string.IsNullOrEmpty(dto.Region)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Region", dto.Region));
            if (!string.IsNullOrEmpty(dto.Designation)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Designation", dto.Designation));
            //await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Status", ((int)dto.Status).ToString()));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("IsActive", dto.IsActive.ToString()));
            //if (!string.IsNullOrEmpty(dto.ReportToUserId)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("ReportToUserId", dto.ReportToUserId));

            return user.Id;
        }

        public async Task<bool> UpdateAsync(UserUpdateDto dto)
        {
            // validate dto
            var validator = new seashore_CRM.BLL.Validators.UserUpdateDtoValidator();
            var vresult = await validator.ValidateAsync(dto);
            if (!vresult.IsValid) return false;

            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null) return false;

            if (!string.Equals(user.Email, dto.Email, System.StringComparison.OrdinalIgnoreCase))
            {
                var setEmail = await _userManager.SetEmailAsync(user, dto.Email);
                var setUser = await _userManager.SetUserNameAsync(user, dto.Email);
                if (!setEmail.Succeeded || !setUser.Succeeded) return false;
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var toAdd = dto.Roles.Except(currentRoles).ToArray();
            var toRemove = currentRoles.Except(dto.Roles).ToArray();
            if (toAdd.Length > 0) await _userManager.AddToRolesAsync(user, toAdd);
            if (toRemove.Length > 0) await _userManager.RemoveFromRolesAsync(user, toRemove);

            // update claims
            var claims = await _userManager.GetClaimsAsync(user);
            var claimTypes = new[] { "FullName", "Contact", "Region", "Status", "IsActive", "ReportToUserId", "Designation" };
            foreach (var ct in claimTypes)
            {
                var existing = claims.Where(c => c.Type == ct).ToList();
                foreach (var ex in existing) await _userManager.RemoveClaimAsync(user, ex);
            }

            if (!string.IsNullOrEmpty(dto.FullName)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FullName", dto.FullName!));
            if (!string.IsNullOrEmpty(dto.Contact)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Contact", dto.Contact!));
            if (!string.IsNullOrEmpty(dto.Region)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Region", dto.Region!));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Status", ((int)dto.Status).ToString()));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("IsActive", dto.IsActive.ToString()));
            if (!string.IsNullOrEmpty(dto.ReportToUserId)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("ReportToUserId", dto.ReportToUserId));
            if (!string.IsNullOrEmpty(dto.Designation)) await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Designation", dto.Designation));

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
                if (!resetResult.Succeeded) return false;
            }

            return true;
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return;
            await _userManager.DeleteAsync(user);
        }
    }
}