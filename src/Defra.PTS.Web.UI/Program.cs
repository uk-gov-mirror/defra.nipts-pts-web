using Defra.PTS.Web.Application.Extensions;
using Defra.PTS.Web.CertificateGenerator.Extensions;
using Defra.PTS.Web.Domain.Models;
using Defra.PTS.Web.Infrastructure.Extensions;
using Defra.PTS.Web.UI.Configuration;
using Defra.PTS.Web.UI.Configuration.Startup;
using Defra.PTS.Web.UI.Constants;
using Defra.Trade.Common.Api.Health;
using Defra.Trade.Common.Api.Infrastructure;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.FeatureManagement;
using System.Globalization;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);

// Configure PDFsharp font resolution (see PdfFontConfiguration for details).
PdfFontConfiguration.Configure();

builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
builder.Services.AddMvc()
        .AddViewLocalization(
            LanguageViewLocationExpanderFormat.Suffix,
            opts => { opts.ResourcesPath = "Resources"; })
        .AddDataAnnotationsLocalization();

builder.Configuration.ConfigureTradeAppConfiguration(settings =>
{
    settings.UseKeyVaultSecrets = true;
    settings.RefreshKeys.Add("RemosSignUpService:Sentinel");
});

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
#if DEBUG
builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
#else
           builder.Configuration.AddJsonFile("appsettings.json", true, true);
#endif

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddTradeAppConfiguration(builder.Configuration)
    .AddApimAuthentication(builder.Configuration.GetSection(ApimSettings.InternalApim));

// Add services to the container.
builder.Services.AddTradeApi(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<PtsSettings>(builder.Configuration.GetSection("PTS"));
builder.Services.Configure<GoogleTagManager>(builder.Configuration.GetSection("GoogleTagManager"));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationApis(builder.Configuration);
builder.Services.AddFeatureManagement();
builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<Program>(); });

builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession(options => 
{ 
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
});
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddCertificateServices(builder.Configuration);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    //set to true when GA is implemented
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddCors(o => o.AddPolicy("AllowOrigins", builder =>
{
    builder.WithOrigins("*")
           .AllowAnyMethod()
           .AllowAnyHeader();
}));


builder.Services.AddAntiforgery();

var useAuth = builder.Configuration.GetValue<bool>("AppSettings:UseAuth");
if (useAuth)
    builder.Services.AddAuthentications(builder.Configuration);

var app = builder.Build();
_ = app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cache-control", "no-cache, no-store, must-revalidate");
    context.Response.Headers.Append("Pragma", "no-cache");
    context.Response.Headers.Append(
        "Content-Security-Policy",
        $"default-src *; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline' https:; img-src 'self' www.googletagmanager.com data:;");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "same-origin");
    context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
    context.Response.Headers.Expires = "0";
    await next();
});
app.UseTradeHealthChecks();
_ = app.UseHttpsRedirection();
_ = app.UseCookiePolicy();
var supportedCultures = new[]
{
    new CultureInfo("en-GB"),
    new CultureInfo("cy")
};

app.UseRequestLocalization(options =>
{
    var questStringCultureProvider = options.RequestCultureProviders[0];
    options.RequestCultureProviders.RemoveAt(0);
    options.RequestCultureProviders.Insert(1, questStringCultureProvider);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.ApplyCurrentCultureToResponseHeaders = true;
});


_ = app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
});

_ = app.UseExceptionHandler("/Error/HandleError/500");
_ = app.UseStatusCodePagesWithReExecute("/Error/HandleError/{0}");
_ = app.UseRouting();
_ = app.UseCors("AllowOrigins");

if (useAuth)
{
    // UseAuthentication() must be before UseAuthorization()
    _ = app.UseAuthentication();
    _ = app.UseAuthorization();
}

_ = app.UseSession();
_ = app.UseMiddleware<SessionTimeoutMiddleware>();

_ = app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
