using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var resourceAttributes = new Dictionary<string, object> {
    { "service.name", "otel-native-web" },
    { "service.instance.id", Environment.MachineName }};
var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);

builder.Services.AddOpenTelemetryTracing(x => x
    .SetResourceBuilder(resourceBuilder)
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation()
    .AddJaegerExporter());

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
