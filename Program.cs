using AssinaturaDigital.Data;
using AssinaturaDigital.Models;
using AssinaturaDigital.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Configurar PostgreSQL
// ----------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------
// Configurar Identity (.NET 9)
// ----------------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // exige confirmação de email
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders(); // necessário para reset de senha, email confirmation etc.

// ----------------------
// Configurar cookies
// ----------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// ----------------------
// Serviços customizados
// ----------------------
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<PdfService>();
builder.Services.AddTransient<IEmailSender, IdentityEmailSender>();

// ----------------------
// Controllers, Views e Razor Pages
// ----------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    // Rota alternativa para /Dashboard/Register
    options.Conventions.AddAreaPageRoute(
        areaName: "Identity",
        pageName: "/Account/Register",
        route: "/Dashboard/Register"
    );
});

var app = builder.Build();

// ----------------------
// Middleware
// ----------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Identity
app.UseAuthorization();

// ----------------------
// Rotas
// ----------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Necessário para páginas do Identity

app.Run();
