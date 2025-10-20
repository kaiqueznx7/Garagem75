using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// --- 2. Configuração da Cultura para pt-BR ---
var defaultCulture = "pt-BR";
var cultureInfo = new CultureInfo(defaultCulture);

// Define a cultura para formatação e parsing (números, datas, moedas)
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configura o Model Binding para usar a cultura pt-BR
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = new List<CultureInfo> { cultureInfo };
    options.SupportedUICultures = new List<CultureInfo> { cultureInfo };
});

builder.Services.AddDbContext<Garagem75DBContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>();



// Add services to the container.
builder.Services.AddControllersWithViews();

// DataProtection (chaves persistentes fora da pasta do projeto)
var dpBase = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "Garagem75Keys");
Directory.CreateDirectory(dpBase);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dpBase));

// Antiforgery (só exigido em POST/PUT/DELETE)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "Garagem75.AntiCsrf";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.HeaderName = "X-CSRF-TOKEN";
});

// Cookie Auth (ESQUEMA ÚNICO)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Garagem75.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        options.LoginPath = "/Usuario/Login";
        options.AccessDeniedPath = "/Usuario/AcessoNegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });



builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();
app.UseRequestLocalization();



app.UseSession();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
