using Garagem75.Client.Services;
using Garagem75.Data;
using Garagem75.Interfaces;
using Garagem75.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

//builder.Services.AddDbContext<Garagem75DBContext>(options => 
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<UsuarioApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7244/");
});
builder.Services.AddHttpClient<ClienteApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7244/");
});
builder.Services.AddHttpClient<PecaApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7244/");
});
builder.Services.AddHttpClient<OrdemServicoApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7244/");
});
builder.Services.AddHttpClient<VeiculoApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7244/");
});
builder.Services.AddHttpClient<DashboardApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7244/");
});


//Repositórios
//builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
//builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>();


builder.Services.AddControllersWithViews(options =>
{
    // Força a mensagem de erro em português para números (resolve o erro em inglês)
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        (_) => "O campo deve ser um número válido, utilize vírgula (,) como separador decimal."
    );
})
// Adicione estas duas linhas se quiser traduzir outras mensagens de erro:
.AddViewLocalization()
.AddDataAnnotationsLocalization();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
