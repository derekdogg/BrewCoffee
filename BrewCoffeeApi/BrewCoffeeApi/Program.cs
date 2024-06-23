using BrewCoffeeApi;

using Microsoft.Extensions.Logging;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
var app = builder.Build();


 

 
 

app.MapGet("/", () => "Brew Coffee API !");


app.MapGet("/brew-coffee", async (HttpContext context) =>
{
    var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();

    int requestCount = RequestCounter.Increment();

    if (requestCount % 5 == 0) //every 5 calls reply with service unavailable.
    {
        return Results.StatusCode(503);
    }
    else if (BrewCoffeeUtils.IsAprilFoolsDay(DateTime.Now))
    {
        return Results.StatusCode(418); //I'm a teapot
    }
    else
    {    
        try
        {
            // Read API key from configuration

            string apiKey;
            try
            {
                var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
                apiKey = BrewCoffeeUtils.GetApiKey(configuration);
            }
            catch
            {                
                return Results.StatusCode(500); // Internal Server Error if API key is not set
            }


            var httpClient = httpClientFactory.CreateClient();

            // Define latitude and longitude for Point Cook, Melbourne
            string latitude = "-37.9142";
            string longitude = "144.7506";

            // Call OpenWeatherMap API
            var weatherResponse = await BrewCoffeeUtils.GetWeatherDataAsync(httpClient, latitude, longitude, apiKey);

            // Parse JSON to extract temperature
            using (JsonDocument document = JsonDocument.Parse(weatherResponse))
            {
                double temperature = document.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
                
                temperature = BrewCoffeeUtils.KelvinToCelsius(temperature);

                var msg = temperature > 30 ? "Your refreshing iced coffee is ready." : "Your piping hot coffee is ready.";

                var melbourneDateTime = BrewCoffeeUtils.LocalUTCDateTime();

                var response = new Response
                {
                    Message = $"{msg}",
                    Prepared = BrewCoffeeUtils.FormatUTCDateTime(melbourneDateTime),
               
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

public partial class Program { }
