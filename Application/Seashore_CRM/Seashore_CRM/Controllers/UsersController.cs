using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Common.Constants;
using seashore_CRM.Models.DTOs;
using Seashore_CRM.ViewModels.User;
using System.Linq;
using System.Threading.Tasks;

namespace Seashore_CRM.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly PasswordHasher<seashore_CRM.Models.Entities.User> _passwordHasher
            = new PasswordHasher<seashore_CRM.Models.Entities.User>();

        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        // ====================
        // INDEX
        // ====================
        public async Task<IActionResult> Index()
        {
            var dtos = await _userService.GetAllAsync();

            var model = dtos.Select(d => new UserListViewModel
            {
                Id = d.Id,
                UserName = d.FullName,
                Email = d.Email,
                Region = d.Region,
                IsActive = d.IsActive,
                ReportToName = d.ReportToName,
                Roles = d.Role,
            }).ToList();

            return View(model);
        }

        // ====================
        // CREATE
        // ====================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await PopulateCreateViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            // uniqueness checks
            if (await _userService.IsEmailTakenAsync(model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");

            if (await _userService.IsFullNameTakenAsync(model.FullName))
                ModelState.AddModelError(nameof(model.FullName), "Full name already exists.");

            if (!string.IsNullOrWhiteSpace(model.Contact) && await _userService.IsContactTakenAsync(model.Contact))
                ModelState.AddModelError(nameof(model.Contact), "Contact already exists.");

            // Password and email required
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError(string.Empty, "Email and password are required.");

            // Password confirmation
            if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != model.ConfirmPassword)
                ModelState.AddModelError(string.Empty, "Password and Confirm Password do not match.");

            if (!ModelState.IsValid)
            {
                var vm = await PopulateCreateViewModel(model);
                return View(vm);
            }

            try
            {
                // Map ViewModel → DTO
                var dto = new UserCreateDto
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Contact = model.Contact,
                    Designation = model.Designation,
                    Region = model.Region,
                    ReportToUserId = string.IsNullOrEmpty(model.ReportToUserId) ? null : int.Parse(model.ReportToUserId),
                    RoleId = string.IsNullOrEmpty(model.RoleId) ? 0 : int.Parse(model.RoleId),
                    IsActive = model.IsActive,
                    Password = model.Password
                };

                await _userService.CreateAsync(dto);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vm = await PopulateCreateViewModel(model);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // ====================
        // EDIT
        // ====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _userService.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var roles = (await _roleService.GetAllAsync())
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.RoleName }).ToList();

            var users = (await _userService.GetAllAsync())
                .Where(u => u.Id != id)
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.FullName ?? u.Email }).ToList();

            var vm = new UserUpdateViewModel
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Contact = entity.Contact,
                Email = entity.Email,
                Designation = entity.Designation,
                Region = entity.Region,
                ReportToUserId = entity.ReportToUserId,
                RoleId = entity.RoleId,
                IsActive = entity.IsActive,
                Roles = roles,
                Users = users
            };

            ViewBag.Roles = roles;
            ViewBag.Users = users;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEditViewModel(model);
                return View(model);
            }

            if (await _userService.IsEmailTakenAsync(model.Email, model.Id))
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");

            if (await _userService.IsFullNameTakenAsync(model.FullName, model.Id))
                ModelState.AddModelError(nameof(model.FullName), "Full name already exists.");

            if (!string.IsNullOrWhiteSpace(model.Contact) && await _userService.IsContactTakenAsync(model.Contact, model.Id))
                ModelState.AddModelError(nameof(model.Contact), "Contact already exists.");

            if (!string.IsNullOrWhiteSpace(model.NewPassword) && model.NewPassword != model.ConfirmPassword)
                ModelState.AddModelError(string.Empty, "Password and Confirm Password do not match.");

            if (!ModelState.IsValid)
            {
                await PopulateEditViewModel(model);
                return View(model);
            }

            var existing = await _userService.GetByIdAsync(model.Id);
            if (existing == null) return NotFound();

            var updateDto = new UserUpdateDto
            {
                Id = model.Id,
                FullName = model.FullName,
                Email = model.Email,
                Contact = model.Contact,
                Designation = model.Designation,
                Region = model.Region,
                ReportToUserId = model.ReportToUserId,
                RoleId = model.RoleId,
                IsActive = model.IsActive,
                NewPassword = model.NewPassword
            };

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var tempUser = new seashore_CRM.Models.Entities.User { Id = model.Id };
                updateDto.NewPassword = _passwordHasher.HashPassword(tempUser, model.NewPassword);
            }

            try
            {
                await _userService.UpdateAsync(updateDto);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateEditViewModel(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateEditViewModel(UserUpdateViewModel model)
        {
            model.Roles = (await _roleService.GetAllAsync())
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.RoleName }).ToList();

            model.Users = (await _userService.GetAllAsync())
                .Where(u => u.Id != model.Id)
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.FullName ?? u.Email }).ToList();

            ViewBag.Roles = model.Roles;
            ViewBag.Users = model.Users;
        }

        // ====================
        // Toggle Status
        // ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _userService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ====================
        // Remote Validation
        // ====================
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyEmail(string email, int? id)
        {
            return Json(await _userService.IsEmailTakenAsync(email, id)
                ? $"Email '{email}' is already in use." : true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyFullName(string fullName, int? id)
        {
            return Json(await _userService.IsFullNameTakenAsync(fullName, id)
                ? $"Full name '{fullName}' is already in use." : true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyContact(string contact, int? id)
        {
            return Json(await _userService.IsContactTakenAsync(contact, id)
                ? $"Contact '{contact}' is already in use." : true);
        }

        // ====================
        // Populate Create ViewModel
        // ====================
        private async Task<UserCreateViewModel> PopulateCreateViewModel(UserCreateViewModel? model = null)
        {
            var roles = (await _roleService.GetAllAsync())
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.RoleName }).ToList();

            var users = (await _userService.GetAllAsync())
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.FullName ?? u.Email }).ToList();

            return new UserCreateViewModel
            {
                FullName = model?.FullName ?? string.Empty,
                Contact = model?.Contact,
                Email = model?.Email ?? string.Empty,
                Designation = model?.Designation,
                Region = model?.Region,
                ReportToUserId = model?.ReportToUserId,
                RoleId = model?.RoleId,
                Password = string.Empty,
                ConfirmPassword = string.Empty,
                IsActive = model?.IsActive ?? true,
                Roles = roles,
                Users = users
            };
        }
    }
}