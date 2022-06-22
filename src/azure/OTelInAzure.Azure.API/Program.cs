using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var resourceAttributes = new Dictionary<string, object> {
    { "service.name", "otel-azure-api" },
    { "service.instance.id", Environment.MachineName }};
var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);

builder.Services.AddOpenTelemetryTracing(x => x
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Azure.*")
    .AddSource("OTelInAzure.*")
    .AddAspNetCoreInstrumentation()
    .AddAzureMonitorTraceExporter(config =>
    {
        config.ConnectionString = "InstrumentationKey=67b15f61-70e1-4964-9ce9-1130fcaa3952;IngestionEndpoint=https://uksouth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://uksouth.livediagnostics.monitor.azure.com/";
    }));

builder.Services.AddSingleton<ActivitySource>(new ActivitySource("OTelInAzure.Azure.API"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
