using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seashore_CRM.ViewModels;
using seashore_CRM.Common.Constants;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Seashore_CRM.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    public class UsersController : Controller
    {
        private readonly IUserAppService _userAppService;
        private readonly IRoleService _roleService;

        public UsersController(IUserAppService userAppService, IRoleService roleService)
        {
            _userAppService = userAppService;
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _userAppService.GetAllAsync();
            var model = dtos.Select(d => new UserListViewModel
            {
                Id = d.Id,
                UserName = d.UserName,
                Email = d.Email,
                Roles = d.Roles ?? new List<string>(),
                Status = d.Status,
                Region = d.Region,
                IsActive = d.IsActive,
                ReportToName = d.ReportToUserId
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var roles = (await _roleService.GetAllAsync()).Select(r => r.RoleName).ToList();
            ViewBag.Roles = roles;

            var users = await _userAppService.GetAllAsync();
            ViewBag.Users = users.Select(u => new { Id = u.Id, UserName = u.UserName, Email = u.Email }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string email,
            string password,
            string? role,
            string? reportToUserId,
            string? fullName,
            string? contact,
            string? region,
            string? status,
            bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                await PopulateCreateViewBags();
                return View();
            }

            var dto = new UserCreateDto
            {
                Email = email,
                Password = password,
                Role = role,
                ReportToUserId = reportToUserId,
                FullName = fullName,
                Contact = contact,
                Region = region,
                Status = int.TryParse(status, out var s) ? (UserStatus)s : UserStatus.Active,
                IsActive = isActive
            };

            var id = await _userAppService.CreateAsync(dto);
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError(string.Empty, "Failed to create user. See logs for details.");
                await PopulateCreateViewBags();
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var dto = await _userAppService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var roles = (await _roleService.GetAllAsync()).Select(r => r.RoleName).ToList();
            ViewBag.AllRoles = roles;
            ViewBag.UserRoles = dto.Roles ?? new List<string>();

            var users = await _userAppService.GetAllAsync();
            ViewBag.Users = users.Select(u => new { Id = u.Id, UserName = u.UserName, Email = u.Email }).ToList();

            ViewBag.User = new { Id = dto.Id, UserName = dto.UserName, Email = dto.Email };
            ViewBag.FullName = dto.FullName;
            ViewBag.Contact = dto.Contact;
            ViewBag.Region = dto.Region;
            ViewBag.Status = ((int)dto.Status).ToString();
            ViewBag.IsActive = dto.IsActive.ToString();
            ViewBag.ReportToUserId = dto.ReportToUserId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            string? role,
            string? reportToUserId,
            string? fullName,
            string? contact,
            string? email,
            string? region,
            string? status,
            bool isActive = true,
            string? newPassword = null)
        {
            var dto = new UserUpdateDto
            {
                Id = id,
                Email = email ?? string.Empty,
                FullName = fullName,
                Contact = contact,
                Region = region,
                Roles = new List<string>(),
                ReportToUserId = reportToUserId,
                Status = int.TryParse(status, out var s) ? (UserStatus)s : UserStatus.Active,
                IsActive = isActive,
                NewPassword = string.IsNullOrWhiteSpace(newPassword) ? null : newPassword
            };

            if (!string.IsNullOrEmpty(role)) dto.Roles.Add(role);

            var ok = await _userAppService.UpdateAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Failed to update user.");
                await PopulateEditViewBags(id);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _userAppService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCreateViewBags()
        {
            var roles = (await _roleService.GetAllAsync()).Select(r => r.RoleName).ToList();
            ViewBag.Roles = roles;
            var users = await _userAppService.GetAllAsync();
            ViewBag.Users = users.Select(u => new { Id = u.Id, UserName = u.UserName, Email = u.Email }).ToList();
        }

        private async Task PopulateEditViewBags(string id)
        {
            var dto = await _userAppService.GetByIdAsync(id);
            var roles = (await _roleService.GetAllAsync()).Select(r => r.RoleName).ToList();
            ViewBag.AllRoles = roles;
            ViewBag.UserRoles = dto?.Roles ?? new List<string>();
            var users = await _userAppService.GetAllAsync();
            ViewBag.Users = users.Select(u => new { Id = u.Id, UserName = u.UserName, Email = u.Email }).ToList();

            ViewBag.User = new { Id = dto?.Id, UserName = dto?.UserName, Email = dto?.Email };
            ViewBag.FullName = dto?.FullName;
            ViewBag.Contact = dto?.Contact;
            ViewBag.Region = dto?.Region;
            ViewBag.Status = dto != null ? ((int)dto.Status).ToString() : "0";
            ViewBag.IsActive = dto?.IsActive.ToString() ?? "true";
            ViewBag.ReportToUserId = dto?.ReportToUserId;
        }
    }
}