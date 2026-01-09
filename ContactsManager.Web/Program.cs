using ContactsManager.Web.Middlewares;
using ContactsManager.Web.Startup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders().AddConsole();
builder.Host.UseSerilog((HostBuilderContext hostBuilder, IServiceProvider services, LoggerConfiguration configureLogger) =>
{
    configureLogger.ReadFrom.Configuration(hostBuilder.Configuration);
    configureLogger.ReadFrom.Services(services);
});

builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    //app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandlingMiddleware();
}
app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseStaticFiles();
app.MapControllers();

app.Run();

