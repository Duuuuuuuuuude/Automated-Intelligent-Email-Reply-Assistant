using Serilog;
using System.Diagnostics;
using System.Reflection;



#if DEBUG
Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
#endif
Log.Logger = new LoggerConfiguration().WriteTo.Debug()
                                      .MinimumLevel.Debug()
                                      .CreateBootstrapLogger();
Log.Information("Starting up the AIERA API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!)
                         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                         .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

    // Add logging
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((hostContext, services, configuration) =>
    {
        configuration.ReadFrom.Configuration(hostContext.Configuration);
        configuration.Enrich.FromLogContext();
        //configuration.ReadFrom.Services(services);
    });


    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    await app.RunAsync().ConfigureAwait(false);

}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred.");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync().ConfigureAwait(false);
}