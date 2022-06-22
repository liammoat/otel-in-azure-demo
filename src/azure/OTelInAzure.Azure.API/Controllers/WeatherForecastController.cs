using System.Diagnostics;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using OTelInAzure.Core;

namespace OTelInAzure.Azure.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly Random _random = new Random();
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IConfiguration _configuration;
    private readonly ActivitySource _activitySource;

    public WeatherForecastController(IConfiguration configuration, ActivitySource activitySource)
    {
        _configuration = configuration;
        _activitySource = activitySource;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<WeatherForecast> GetAsync()
    {
        using var activity = _activitySource.StartActivity("GetWeatherForecast");

        await new BlobClient(connectionString: _configuration.GetConnectionString("AzureStorage"), blobContainerName: "content", blobName: "hello-world.txt").DownloadContentAsync();
        activity?.AddEvent(new ActivityEvent("Blob download complete"));

        var random = _random.Next(1, 5);
        if (random % 2 == 0) await Task.Delay(random * 300);
        activity?.AddEvent(new ActivityEvent("Random compute complete"));

        activity?.SetStatus(ActivityStatusCode.Ok);

        return new WeatherForecast
        {
            Date = DateTime.Now,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };
    }
}
