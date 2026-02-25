using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Common.Constants;
using seashore_CRM.Models.DTOs;
using Seashore_CRM.ViewModels;
using Seashore_CRM.ViewModels.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Seashore_CRM.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly PasswordHasher<seashore_CRM.Models.Entities.User> _passwordHasher = new PasswordHasher<seashore_CRM.Models.Entities.User>();

        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _userService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }
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

                // ReportToUser navigation
                ReportToName = d.ReportToUser != null ? d.ReportToUser.FullName : null,

                // Single role mapped as a list with one string
                Roles = d.Role != null ? new List<string> { d.Role.RoleName } : new List<string>()
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleService.GetAllAsync();
            var users = await _userService.GetAllAsync();

            var vm = new UserCreateViewModel
            {
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RoleName
                }),
                Users = (await _userService.GetAllAsync()).Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FullName
                })
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDto userCreateDto)
        {
            // uniqueness checks similar to CompaniesController
            if (await _userService.IsEmailTakenAsync(userCreateDto.Email))
                ModelState.AddModelError(nameof(userCreateDto.Email), "Email already exists.");

            if (await _userService.IsFullNameTakenAsync(userCreateDto.FullName))
                ModelState.AddModelError(nameof(userCreateDto.FullName), "Full name already exists.");

            if (!string.IsNullOrWhiteSpace(userCreateDto.Contact) && await _userService.IsContactTakenAsync(userCreateDto.Contact))
                ModelState.AddModelError(nameof(userCreateDto.Contact), "Contact already exists.");

            if (!ModelState.IsValid)
            {
                var vm = await PopulateCreateViewModel(userCreateDto);
                return View(vm);
            }

            if (string.IsNullOrWhiteSpace(userCreateDto.Email) ||
                string.IsNullOrWhiteSpace(userCreateDto.Password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                var vm = await PopulateCreateViewModel(userCreateDto);
                return View(vm);
            }

            if (userCreateDto.Password != userCreateDto.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and Confirm Password do not match.");
                var vm = await PopulateCreateViewModel(userCreateDto);
                return View(vm);
            }

            try
            {
                var id = await _userService.CreateAsync(userCreateDto);

                if (string.IsNullOrEmpty(id.ToString()))
                {
                    ModelState.AddModelError(string.Empty, "Failed to create user.");
                    var vm = await PopulateCreateViewModel(userCreateDto);
                    return View(vm);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vm = await PopulateCreateViewModel(userCreateDto);
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
                        .Where(u => u.Id != id) // prevent reporting to self
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

            // Populate ViewBag for the Edit view
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

            // Uniqueness checks excluding current user
            if (await _userService.IsEmailTakenAsync(model.Email, model.Id))
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");

            if (await _userService.IsFullNameTakenAsync(model.FullName, model.Id))
                ModelState.AddModelError(nameof(model.FullName), "Full name already exists.");

            if (!string.IsNullOrWhiteSpace(model.Contact) && await _userService.IsContactTakenAsync(model.Contact, model.Id))
                ModelState.AddModelError(nameof(model.Contact), "Contact already exists.");

            // Password validation
            if (!string.IsNullOrWhiteSpace(model.NewPassword) && model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and Confirm Password do not match.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateEditViewModel(model);
                return View(model);
            }

            var entity = await _userService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.FullName = model.FullName;
            entity.Contact = model.Contact;
            entity.Email = model.Email;
            entity.Designation = model.Designation;
            entity.Region = model.Region;
            entity.ReportToUserId = model.ReportToUserId;
            entity.RoleId = model.RoleId;
            entity.IsActive = model.IsActive;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                entity.PasswordHash = _passwordHasher.HashPassword(entity, model.NewPassword);
            }

            try
            {
                await _userService.UpdateAsync(entity);
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

            // Also set ViewBag so the Edit view which uses ViewBag items is populated when re-displaying
            ViewBag.Roles = model.Roles;
            ViewBag.Users = model.Users;
        }


        // Remote validation endpoints
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyEmail(string email, int? id)
        {
            var taken = await _userService.IsEmailTakenAsync(email, id);
            if (taken)
                return Json($"Email '{email}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyFullName(string fullName, int? id)
        {
            var taken = await _userService.IsFullNameTakenAsync(fullName, id);
            if (taken)
                return Json($"Full name '{fullName}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyContact(string contact, int? id)
        {
            var taken = await _userService.IsContactTakenAsync(contact, id);
            if (taken)
                return Json($"Contact '{contact}' is already in use.");
            return Json(true);
        }

        private async Task<UserCreateViewModel> PopulateCreateViewModel(UserCreateDto? dto = null)
        {
            var roles = (await _roleService.GetAllAsync()).Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.RoleName
            }).ToList();

            var users = (await _userService.GetAllAsync()).Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FullName ?? u.Email
            }).ToList();

            var vm = new UserCreateViewModel
            {
                FullName = dto?.FullName ?? string.Empty,
                Contact = dto?.Contact,
                Email = dto?.Email ?? string.Empty,
                Designation = dto?.Designation,
                Region = dto?.Region,
                ReportToUserId = dto?.ReportToUserId.HasValue == true ? dto.ReportToUserId.Value.ToString() : dto?.ReportToUserId?.ToString(),
                RoleId = dto != null ? dto.RoleId.ToString() : null,
                // Do not populate password fields for security reasons
                Password = string.Empty,
                ConfirmPassword = string.Empty,
                IsActive = dto?.IsActive ?? true,
                Roles = roles,
                Users = users
            };

            return vm;
        }

        //private async Task PopulateEditViewBags(string id)
        //{
        //    var dto = await _user_service.GetByIdAsync(id);

        //    var roles = (await _role_service.GetAllAsync())
        //                .Select(r => new SelectListItem
        //                {
        //                    Value = r.Id.ToString(),
        //                    Text = r.RoleName
        //                }).ToList();

        //    var users = (await _user_service.GetAllAsync())
        //                .Select(u => new SelectListItem
        //                {
        //                    Value = u.Id,
        //                    Text = u.UserName ?? u.Email
        //                }).ToList();

        //    ViewBag.AllRoles = roles;
        //    ViewBag.UserRoles = dto?.Roles ?? new List<string>();
        //    ViewBag.Users = users;
        //    ViewBag.User = dto;
        //}
    }
}