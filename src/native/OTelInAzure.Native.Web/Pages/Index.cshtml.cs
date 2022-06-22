using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OTelInAzure.Core;

namespace OTelInAzure.Native.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public string WeatherSummary { get; set; }

    public IndexModel(IHttpClientFactory httpClientFactory,
        IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task OnGet()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_configuration["APIBaseAddress"]);
            var result = await httpClient.GetFromJsonAsync<WeatherForecast>("weatherforecast");
            WeatherSummary = result.Summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retreive weather forecast.");
        }
    }
}
