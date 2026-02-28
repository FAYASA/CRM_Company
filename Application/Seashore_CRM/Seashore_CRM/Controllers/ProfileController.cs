using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.DTOs;
using System.Security.Claims;

namespace Seashore_CRM.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId(); // from session or claims

            var profile = await _userService.GetProfileAsync(userId);
            return View(profile);
        }

        public async Task<IActionResult> Edit()
        {
            int userId = GetCurrentUserId();
            var profile = await _userService.GetProfileAsync(userId);

            var dto = new ProfileUpdateDto
            {
                Id = profile.Id,
                Email = profile.Email,
                FullName = profile.FullName,
                Contact = profile.Contact,
                Region = profile.Region
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _userService.UpdateProfileAsync(dto);
            return RedirectToAction("Index");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            dto.UserId = GetCurrentUserId();

            try
            {
                await _userService.ChangePasswordAsync(dto);
                TempData["SuccessMessage"] = "Password updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Display exception message as alert
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }


        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new Exception("User is not authenticated.");

            return int.Parse(userIdClaim.Value);
        }

        // ============================
        // Remote validation endpoints (used by RemoteAttribute)
        // ============================
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyEmail(string email, int? id)
        {
            var taken = await _userService.IsEmailTakenAsync(email, id);
            if (taken)
                return Json($"Email '{email}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyContactNumber(string contact, int? id)
        {
            var taken = await _userService.IsContactTakenAsync(contact, id);
            if (taken)
                return Json($"Contact Number '{contact}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyFullName(string FullName, int? id)
        {
            var taken = await _userService.IsFullNameTakenAsync(FullName, id);
            if (taken)
                return Json($"Full Name '{FullName}' is already in use.");
            return Json(true);
        }
    }
}