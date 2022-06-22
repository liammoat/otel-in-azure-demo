using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var resourceAttributes = new Dictionary<string, object> {
    { "service.name", "otel-azure-web" },
    { "service.instance.id", Environment.MachineName }};
var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);

builder.Services.AddOpenTelemetryTracing(x => x
    .SetResourceBuilder(resourceBuilder)
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation()
    .AddAzureMonitorTraceExporter(config =>
    {
        config.ConnectionString = "InstrumentationKey=67b15f61-70e1-4964-9ce9-1130fcaa3952;IngestionEndpoint=https://uksouth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://uksouth.livediagnostics.monitor.azure.com/";
    }));

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
