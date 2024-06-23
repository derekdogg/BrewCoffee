using BrewCoffeeApi;

var builder = WebApplication.CreateBuilder(args);
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

app.MapGet("/brew-coffee", (HttpContext context) =>
{

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
        var response = new Response
        {
            Message = "Your piping hot coffee is ready",
            Prepared = GetIso8601Timestamp()
        };

        
        return Results.Json(response); // Returns a 200 OK status with the custom response
    }
});



app.Run();

public partial class Program { }
