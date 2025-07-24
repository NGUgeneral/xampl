using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using xampl.Hubs;
using xampl.Models.Xampl;
using xampl.Services.ClaimsTransformer;
using xampl.Services.ConfigOptionsService;
using xampl.Services.EmailSenderService;
using xampl.Services.GeminiService;
using xampl.Services.RepositoryService;
using xampl.Utils;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    ConfigUtils.LoadAndReplaceEnvironmentVariables(builder.Configuration);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    ConfigUtils.Initialize(builder.Configuration);
    builder.Services.Configure<ConfigOptions>(options =>
        {
            builder.Configuration.GetSection(ConfigOptions.ConfigVariablesSectionKey).Bind(options);
            builder.Configuration.GetSection(ConfigOptions.ConfigSmtpSettingsSectionKey).Bind(options.SmtpSettings);
        });
    builder.Services.AddSingleton<EmailSender>();
    builder.Services.AddSingleton<GeminiService>();
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddControllersWithViews();
    builder.Services.AddSignalR();
    builder.Services.AddHttpClient();
    builder.Services.AddDbContextPool<XamplContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("supabase"), options =>
        {
            options.CommandTimeout(180);
        })
    );
    builder.Services.AddScoped<IRepository<XamplContext>, Repository<XamplContext>>();
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
    builder.Services.AddScoped<IClaimsTransformation, ExternalUserClaimsTransformer>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/About/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHub<ConsoleHub>("/consoleHub");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=About}/{action=Index}/{id?}");
    app.MapGet("/health", () => Results.Ok("Healthy"));

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