using NLog;
using NLog.Web;
using Microsoft.EntityFrameworkCore;
using xampl.Models.Documents;
using xampl.Services.Repository;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
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