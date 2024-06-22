using BrewCoffeeApi;
 

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
var app = builder.Build();


string GetIso8601Timestamp()
{
    return DateTimeOffset.UtcNow.ToString("o");
}

static bool IsAprilFoolsDay()
{
    // Return true if the month is April and the day is 1
    return DateTime.Today.Month == 4 && DateTime.Today.Day == 1;
}

app.MapGet("/", () => "Brew Coffee API !");

app.MapGet("/brew-coffee", async (HttpContext context) =>
{
    var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();

    int requestCount = RequestCounter.Increment();

    if (requestCount % 5 == 0)
    {
        return Results.StatusCode(503);
    }
    else if (IsAprilFoolsDay()) 
    {
        return Results.StatusCode(418);
    }
    else
    {
        //Add here some exception handling because this can easily break.
        
        
        // Read API key from configuration
        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = configuration["WeatherApi:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            return Results.StatusCode(500); // Internal Server Error if API key is not set
        }

        var httpClient = httpClientFactory.CreateClient();

        // Define latitude and longitude for Point Cook, Melbourne
        string latitude = "-37.9142";
        string longitude = "144.7506";

        // Call OpenWeatherMap API
        var weatherResponse = await httpClient.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}");

        var response = new Response
        {
            Message = "Your piping hot coffee is ready",
            Prepared = GetIso8601Timestamp(),
            //WeatherInfo = weatherResponse
        };

        return Results.Json(response);
    }
});


app.Run();
