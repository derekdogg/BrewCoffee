using BrewCoffeeApi;
using Microsoft.Extensions.Logging;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
var app = builder.Build();


string MelbourneUTCDateTime()
{
   DateTimeOffset utcNow = DateTimeOffset.UtcNow;
   TimeZoneInfo melbourneTimeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");

   DateTimeOffset melbourneTime = TimeZoneInfo.ConvertTime(utcNow, melbourneTimeZone);

   return melbourneTime.ToString("yyyy-MM-ddTHH:mm:sszzz");     
}

bool IsAprilFoolsDay()
{
    var today = DateTime.Today;
    return today.Month == 4 && today.Day == 1;
}

double KelvinToCelsius(double kelvin)
{
    return kelvin - 273.15;
}

app.MapGet("/", () => "Brew Coffee API !");

 


app.MapGet("/brew-coffee", async (HttpContext context) =>
{
    var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();

    int requestCount = RequestCounter.Increment();

    if (requestCount % 5 == 0) //every 5 calls reply with service unavailable.
    {
        return Results.StatusCode(503);
    }
    else if (IsAprilFoolsDay())
    {
        return Results.StatusCode(418); //I'm a teapot
    }
    else
    {
    
        try
        {

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

            // Parse JSON to extract temperature
            using (JsonDocument document = JsonDocument.Parse(weatherResponse))
            {
                double temperature = document.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
                
                temperature = KelvinToCelsius(temperature);

                var msg = temperature > 30 ? "Your refreshing iced coffee is ready." : "Your piping hot coffee is ready.";

                var response = new Response
                {
                    Message = $"{msg}",
                    Prepared = MelbourneUTCDateTime(),
               
                };

                return Results.Json(response);
            }
        }         
        catch
        {            
            return Results.StatusCode(500); 
        }
    }
});

 

app.Run();
