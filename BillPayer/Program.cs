using BillPay.DataAccess.Data;
using BillPay.DataAccess.DbInitializers;
using BillPay.Models;
using BillPay.Models.ViewModels;
using BillPay.Utilities.BackgroundJobs.RecurringJobs;
using BillPay.Utilities.CommonMethods;
using BillPay.Utilities.ExceptionHandling;
using BillPay.Utilities.Middleware;
using BillPay.Utilities.Modules;
using Hangfire;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Ui.MsSqlServerProvider;
using Serilog.Ui.Web;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>();
var conStr = builder.Configuration.GetConnectionString("Myconnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conStr));
builder.Services.AddIdentity<ApplicationUser,ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true).AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.RegisterModule();
builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
builder.Host.UseSerilog(Log.Logger); //Adding this removed the excpetion caused by app.UseSerilogRequestLogging();
builder.Services.AddSerilogUi(options => options.UseSqlServer(conStr, "_ExceptionLogs"));
builder.Services.AddHangfireServer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<MailDetailsViewModel>(builder.Configuration.GetSection("mailDetails"));
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/AccountManager/Account/LogIn");
builder.Services.AddHealthChecks().AddCheck<SqlHealthCheck>(
        "Sql",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "Sql" }); ;
var app = builder.Build();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NAaF1cXmhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEFjW31XcHBRQGNYWUd0Xw==");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.ApplyMigrationAndAddControllersToMenu();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging(
    option=>
    {
        option.EnrichDiagnosticContext = Enricher.HttpRequestEnricher;
    }
    );
app.UseSerilogUi();
app.UseHangfireDashboard();
app.MapHangfireDashboard();
var env = app.Environment;
app.RunWithProgramStart(env);
app.StartHangfireSchedule();
app.UseMiddleware<AuthorizationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = ResponseDesign.WriteResponse
});
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Bills}/{controller=Home}/{action=Index}/{id?}");

app.Run();