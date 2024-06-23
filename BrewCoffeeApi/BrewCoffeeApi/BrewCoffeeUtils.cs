using System.Net;

namespace BrewCoffeeApi
{
    public static class BrewCoffeeUtils
    {
        public static bool IsAprilFoolsDay(DateTime dateTime)
        {

            return dateTime.Month == 4 && dateTime.Day == 1;
        }

        public static double KelvinToCelsius(double kelvin)
        {
            return kelvin - 273.15;
        }

        public static string FormatUTCDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }


        public static DateTime LocalUTCDateTime()
        {
            var localTimeZone = TimeZoneInfo.Local;
             
            var LocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);
            return LocalDateTime;

        }
        public const string WeatherApiKey = "WeatherApi:ApiKey";

        public static string GetApiKey(IConfiguration configuration)
        {
            var apiKey = configuration[WeatherApiKey];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API key is not set.");
            }
            return apiKey;
        }
 


        public static async Task<string> GetWeatherDataAsync(HttpClient httpClient, string latitude, string longitude, string apiKey)
        {
            var weatherApiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}";

             
                HttpResponseMessage response = await httpClient.GetAsync(weatherApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new Exception("Bad Request: The request could not be understood or was missing required parameters.");
                        case HttpStatusCode.Unauthorized:
                            throw new Exception("Unauthorized: Access is denied due to invalid credentials.");
                        case HttpStatusCode.Forbidden:
                            throw new Exception("Forbidden: You do not have permission to access this resource.");
                        case HttpStatusCode.NotFound:
                            throw new Exception("Not Found: The requested resource could not be found.");
                        case HttpStatusCode.InternalServerError:
                            throw new Exception("Internal Server Error: The server encountered an error and could not complete your request.");
                        default:
                            throw new Exception($"Unexpected status code: {response.StatusCode}");
                    }
                }
            }
              
             
        }


     
}
