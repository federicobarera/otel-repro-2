using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.Channel.Implementation;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddOpenTelemetry()
//    .WithTracing(c => c
//        //.SetSampler(new AlwaysOnSampler())
//        .AddAspNetCoreInstrumentation()
//        .AddConsoleExporter());

builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
{
    // To customise adaptive sampling, you must set this flag to false. Please read more here: https://learn.microsoft.com/en-us/azure/azure-monitor/app/sampling?tabs=net-core-new#configure-sampling-settings
    EnableAdaptiveSampling = true,    
});

builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
