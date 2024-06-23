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
            var weatherResponse = await httpClient.GetStringAsync(weatherApiUrl);
            return weatherResponse;
        }


    }
}
