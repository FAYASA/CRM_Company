using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Common.Constants;
using seashore_CRM.BLL.DTOs;
using Seashore_CRM.ViewModels.User;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Seashore_CRM.Extensions;
using Seashore_CRM.Models;

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
            var dtos =  _userService.GetAllAsync();

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

        [HttpPost]
        public async Task<IActionResult> GetUsers()
        {
            var request = new Seashore_CRM.Models.DataTableRequest
            {
                Draw = Request.Form["draw"].FirstOrDefault(),
                Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0"),
                Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10"),
                SearchValue = Request.Form["search[value]"].FirstOrDefault(),
                SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault(),
                SortDirection = Request.Form["order[0][dir]"].FirstOrDefault()
            };

            // Base query (unfiltered) to compute total records
            var baseQuery = _userService.GetAllAsync(); // IQueryable<UserListDto>
            var totalRecords = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(baseQuery);

            // Apply filtering (search)
            var query = baseQuery;
            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                var sval = request.SearchValue.Trim();
                // case-insensitive search using Contains (translated to SQL by EF)
                query = query.Where(u => (u.FullName != null && u.FullName.Contains(sval)) || (u.Email != null && u.Email.Contains(sval)));
            }

            // Count after filtering
            var filteredRecords = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(query);

            // Sorting: map client column names to DTO property names
            if (!string.IsNullOrWhiteSpace(request.SortColumn))
            {
                var col = request.SortColumn;
                var mapped = col switch
                {
                    "userName" => "FullName",
                    "email" => "Email",
                    "role" => "Role",
                    "region" => "Region",
                    "isActive" => "IsActive",
                    _ => col
                };

                bool asc = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                query = query.OrderByDynamic(mapped, asc);
            }

            // Paging
            var paged = query.Skip(request.Start).Take(request.Length);

            var data = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(paged.Select(u => new
            {
                id = u.Id,
                userName = u.FullName,
                email = u.Email,
                role = u.Role,
                region = u.Region,
                isActive = u.IsActive
            }));

            return Json(new
            {
                draw = request.Draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = data
            });
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
            // Keep client-side remote/inline checks but primary validation is in service now
            try
            {
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
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException vex)
            {
                foreach (var err in vex.Errors)
                {
                    if (string.IsNullOrEmpty(err.PropertyName)) ModelState.AddModelError(string.Empty, err.ErrorMessage);
                    else ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
                }

                var vm = await PopulateCreateViewModel(model);
                return View(vm);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vm = await PopulateCreateViewModel(model);
                return View(vm);
            }
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

            var users = (_userService.GetAllAsync())
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
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException vex)
            {
                foreach (var err in vex.Errors)
                {
                    if (string.IsNullOrEmpty(err.PropertyName)) ModelState.AddModelError(string.Empty, err.ErrorMessage);
                    else ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
                }
                await PopulateEditViewModel(model);
                return View(model);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateEditViewModel(model);
                return View(model);
            }
        }

        private async Task PopulateEditViewModel(UserUpdateViewModel model)
        {
            model.Roles = (await _roleService.GetAllAsync())
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.RoleName }).ToList();

            model.Users = (_userService.GetAllAsync())
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

            var users = (_userService.GetAllAsync())
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