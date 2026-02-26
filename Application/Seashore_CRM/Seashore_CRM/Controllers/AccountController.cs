using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using seashore_CRM.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.Services.Service_Interfaces;
using System.Linq;
using seashore_CRM.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Seashore_CRM.ViewModels.Login;

namespace Seashore_CRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService; // application users table service
        private readonly IRoleService _roleService; // service to read role names

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserService userService, IRoleService roleService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Authenticate against the application Users table (custom users)
            var users = await _userService.GetAllAsync();
            var appUser = users.FirstOrDefault(u => string.Equals(u.Email?.Trim(), model.Email?.Trim(), System.StringComparison.OrdinalIgnoreCase));

            if (appUser == null || !appUser.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var passwordHasher = new PasswordHasher<User>();
            var verification = passwordHasher.VerifyHashedPassword(null, appUser.PasswordHash, model.Password);
            if (verification == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // build claims and sign in using Identity cookie scheme so the rest of the app (Authorize) works
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(appUser.FullName) ? appUser.Email : appUser.FullName),
                new Claim(ClaimTypes.Email, appUser.Email ?? string.Empty),
                new Claim("IsActive", appUser.IsActive.ToString()),
                
            };

            // include role claim from Roles table if available
            if (appUser.RoleId > 0)
            {
                var roleEntity = await _roleService.GetByIdAsync(appUser.RoleId);
                if (roleEntity != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleEntity.RoleName));
                }
            }

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties { IsPersistent = model.RememberMe };
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Companies");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Login", "Account");
        }

        // Administrator-only: register a new user and assign role
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Register()
        {
            var roles = _roleManager.Roles.Select(r => new SelectListItem { Text = r.Name!, Value = r.Name! }).ToList();
            var vm = new RegisterViewModel { Roles = roles };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                model.Roles = _roleManager.Roles.Select(r => new SelectListItem { Text = r.Name!, Value = r.Name! }).ToList();
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return RedirectToAction("Index", "Users");
        }
    }
}