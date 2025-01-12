using EShop.Web.Data;
using EShop.Web.Models.Domain;
using EShop.Web.Models.Email;
using EShop.Web.Models.Enums;
using EShop.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EMailSettings"));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<EShopUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add Razor Pages support
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();


builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Seed roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>(); // Get RoleManager service
    var userManager = services.GetRequiredService<UserManager<EShopUser>>(); // Get UserManager service
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    await SeedRolesAndUsersAsync(roleManager, userManager); // Call the seeding method
    await SeedSupplierAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // This is needed for Razor Pages routes

app.Run();

// Seed Roles and Users
async Task SeedRolesAndUsersAsync(RoleManager<IdentityRole> roleManager, UserManager<EShopUser> userManager)
{
    // Define roles
    var roles = new[] { UserType.Admin, UserType.Manufacturer, UserType.Buyer };

    // Ensure roles are created
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role.ToString()))
        {
            await roleManager.CreateAsync(new IdentityRole(role.ToString()));
        }
    }

    // Ensure Admin user
    var adminName = "Admin";
    var adminSurname = "Admin";
    var adminEmail = "admin@example.com";
    var adminPassword = "Admin@123"; // Use a secure password in production
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new EShopUser { FirstName = adminName, LastName = adminSurname, UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, UserType.Admin.ToString());
        }
    }

    // Ensure Manufacturer user
    var manufacturerEmail = "manufacturer@example.com";
    var manufacturerPassword = "Manufacturer@123"; // Use a secure password in production
    var manufacturerAddress = "Street, City, Area, Number";
    if (await userManager.FindByEmailAsync(manufacturerEmail) == null)
    {
        var manufacturerUser = new EShopUser { UserName = manufacturerEmail, Email = manufacturerEmail, EmailConfirmed = true, Address = manufacturerAddress };
        var result = await userManager.CreateAsync(manufacturerUser, manufacturerPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(manufacturerUser, UserType.Manufacturer.ToString());
        }
    }
}

async Task SeedSupplierAsync(ApplicationDbContext dbContext)
{
    // Check if any suppliers already exist
    if (!dbContext.Suppliers.Any())
    {
        // Create a new supplier
        var supplier = new Supplier
        {
            Name = "Best Supplies Inc.",
            Email = "karajanova289@gmail.com",
            Address = "123 Supplier Street, Supplier City, 12345",
            ContactPhone = 1234567890,
            PricePerKm = 6.5f,
            FreeDeliveryKm = 5.0f,
            Orders = new List<Order>() // Empty list for now, or initialize with sample orders if needed
        };

        // Add the supplier to the database
        dbContext.Suppliers.Add(supplier);

        // Save changes to persist the supplier
        await dbContext.SaveChangesAsync();
    }
}
