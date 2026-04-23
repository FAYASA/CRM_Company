using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.BLL.Services;
using AutoMapper;
using FluentValidation;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.BLL.Validators;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.BLL.Services.Service_Interfaces;
using Microsoft.AspNetCore.Identity;
using seashore_CRM.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Seashore_CRM.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register IHttpContextAccessor // This is needed if any service (like UserService) needs to access HttpContext for user info.
builder.Services.AddHttpContextAccessor();

// Add services to the container.
// Global authorization policy: require authenticated user by default
var requireAuthenticatedPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddRazorPages(options =>
{
    // Allow anonymous on the login and register pages
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
})
.AddMvcOptions(options =>
{
    options.Filters.Add(new AuthorizeFilter(requireAuthenticatedPolicy));
});

// Enable MVC controllers + views so controller-based views work
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter(requireAuthenticatedPolicy));
});

// Configure DbContext (placeholder connection string)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(conn);
});

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
});

// Repositories/UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register specialized repositories individually (optional)
builder.Services.AddScoped<ILeadRepository, LeadRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ILeadItemRepository, LeadItemRepository>();
builder.Services.AddScoped<ILeadStatusActivityRepository, LeadStatusActivityRepository>();

// BLL services
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ILeadStatusService, LeadStatusService>();
builder.Services.AddScoped<ILeadSourceService, LeadSourceService>();
builder.Services.AddScoped<IOpportunityService, OpportunityService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<ISaleItemService, SaleItemService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILeadItemService, LeadItemService>();
builder.Services.AddScoped<ILeadStatusActivityService, LeadStatusActivityService>();

// Register system log service implementation
builder.Services.AddScoped<ISystemLogService, SystemLogService>();

// Register user activity service
builder.Services.AddScoped<IUserActivityService, UserActivityService>();

// Register user app service
builder.Services.AddScoped<IUserAppService, UserAppService>();

// Register user lead rights service
builder.Services.AddScoped<IUserLeadRightsService, UserLeadRightsService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(seashore_CRM.BLL.Mapping.AutoMapperProfile));

// FluentValidation
// Register FluentValidation validators explicitly
builder.Services.AddTransient<IValidator<LeadDto>, LeadDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.BLL.DTOs.UserCreateDto>, seashore_CRM.BLL.Validators.UserCreateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.BLL.DTOs.UserUpdateDto>, seashore_CRM.BLL.Validators.UserUpdateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.Models.Entities.Comment>, seashore_CRM.BLL.Validators.CommentValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.Models.Entities.Sale>, seashore_CRM.BLL.Validators.SaleValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.Models.Entities.Invoice>, seashore_CRM.BLL.Validators.InvoiceValidator>();

// Additional validators for DTOs
builder.Services.AddTransient<IValidator<seashore_CRM.BLL.DTOs.CompanyCreateDto>, seashore_CRM.BLL.Validators.CompanyCreateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.BLL.DTOs.CompanyUpdateDto>, seashore_CRM.BLL.Validators.CompanyUpdateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.ApplicationLayer.DTOs.ProductCreateDto>, seashore_CRM.BLL.Validators.ProductCreateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.ApplicationLayer.DTOs.ProductUpdateDto>, seashore_CRM.BLL.Validators.ProductUpdateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.BLL.DTOs.ContactCreateDto>, seashore_CRM.BLL.Validators.ContactCreateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.BLL.DTOs.ContactUpdateDto>, seashore_CRM.BLL.Validators.ContactUpdateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.BLL.DTOs.CategoryCreateDto>, seashore_CRM.BLL.Validators.CategoryCreateDtoValidator>();
builder.Services.AddTransient<IValidator<seashore_CRM.BLL.DTOs.CategoryUpdateDto>, seashore_CRM.BLL.Validators.CategoryUpdateDtoValidator>();

var app = builder.Build();

// Insert exception logging middleware early in pipeline
app.UseMiddleware<ExceptionLoggingMiddleware>();

// Initialize DB and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    // apply migrations if any
    try
    {
        await db.Database.MigrateAsync();
    }
    catch
    {
        // ignore migration errors during development; ensure you run migrations manually
    }

    try
    {
        await DbInitializer.InitializeAsync(db);

        // Seed Identity roles and admin user
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var adminEmail = builder.Configuration["AdminUser:Email"] ?? "admin@example.com";
        var adminPassword = builder.Configuration["AdminUser:Password"] ?? "P@ssw0rd";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
    catch
    {
        // swallowing exceptions prevents startup crash; log in real app
    }

    // Optionally: write a startup info log
    var sysLog = scope.ServiceProvider.GetRequiredService<ISystemLogService>();
    await sysLog.LogMessageAsync("Info", "Application started", "Startup", null);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map controller routes (MVC) and Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Companies}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
