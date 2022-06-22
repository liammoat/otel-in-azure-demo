using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var resourceAttributes = new Dictionary<string, object> {
    { "service.name", "otel-native-api" },
    { "service.instance.id", Environment.MachineName }};
var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);

builder.Services.AddOpenTelemetryTracing(x => x
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Azure.*")
    .AddSource("OTelInAzure.*")
    .AddAspNetCoreInstrumentation()
    .AddJaegerExporter());

builder.Services.AddSingleton<ActivitySource>(new ActivitySource("OTelInAzure.Native.API"));

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
