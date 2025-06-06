using dotenv.net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using xampl.Models.Documents;
using xampl.Services.ConfigOptionsService;
using xampl.Services.EmailSenderService;
using xampl.Services.RepositoryService;
using xampl.Utils;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigUtils.LoadAndReplaceEnvironmentVariables(builder.Configuration);

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    ConfigUtils.Initialize(builder.Configuration);
    builder.Services.Configure<ConfigOptions>(options =>
        {
            builder.Configuration.GetSection(ConfigOptions.ConfigVariablesSectionKey);
            builder.Configuration.GetSection(ConfigOptions.ConfigSmtpSettingsSectionKey);
        });
    builder.Services.AddSingleton<EmailSender>();
    builder.Services.AddAutoMapper(typeof(Program));
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    // Add dbContext
    builder.Services.AddDbContextPool<DocumentsContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("supabase"), options =>
        {
            options.CommandTimeout(180);
        })
    );
    builder.Services.AddScoped<IRepository<DocumentsContext>, Repository<DocumentsContext>>();

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
    .AddCookie()
    .AddGoogle(options =>
        {
            options.ClientId = builder?.Configuration["Variables:GoogleAuthClientId"] ?? string.Empty;
            options.ClientSecret = builder?.Configuration["Variables:GoogleAuthClientSecret"] ?? string.Empty;
            options.Events.OnCreatingTicket = context =>
            {
                Task.Run(async () => await AccountUtils.MaybeRegisterExternalUser(context.Principal));
                return Task.CompletedTask;
            };
        });

    builder.Services.AddAuthorization();


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/About/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=About}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}