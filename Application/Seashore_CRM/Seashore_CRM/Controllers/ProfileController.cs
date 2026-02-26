using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.DTOs;

namespace Seashore_CRM.Controllers
{
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

            await _userService.ChangePasswordAsync(dto);
            return RedirectToAction("Index");
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst("UserId").Value);
        }
    }
}