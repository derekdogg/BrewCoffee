using Microsoft.AspNetCore.Mvc.Testing;
using BrewCoffeeApi;
using Microsoft.Extensions.Configuration;
 

namespace BrewCoffeeApiTests
{
    public class BrewCoffeeTests
    {
        [Fact]
        public async Task TestRootEndpoint()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetStringAsync("/");

            Assert.Equal("Brew Coffee API !", response); //would be better to put this string in a const.
        }

        [Fact]
        public void TestLocalUTCDateTime()
        {
            // Get the local date and time from the method
            var localDateTimeFromMethod = BrewCoffeeUtils.LocalUTCDateTime();

            // Get the local time zone
            var localTimeZone = TimeZoneInfo.Local;

            // Convert the current UTC time to local time manually for comparison
            var expectedLocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);

            // Allow a small delta for execution time differences
            var timeDifference = (localDateTimeFromMethod - expectedLocalDateTime).TotalSeconds;

            // Check that the time difference is within a reasonable range (e.g., less than 5 second)
            Assert.True(Math.Abs(timeDifference) < 5, $"The local time difference is not within the expected range: {timeDifference} seconds");
        }

        [Fact]
        public void TestAprilFoolDateTime()
        {             
            var aprilFoolsDay = new DateTime(2024, 4, 1);
 
            var result = BrewCoffeeUtils.IsAprilFoolsDay(aprilFoolsDay);
             
            Assert.True(result,"April fools day date should be true");
             
            var notAprilFoolsDay = new DateTime(2024, 4, 2);
             
            result = BrewCoffeeUtils.IsAprilFoolsDay(notAprilFoolsDay);
             
            Assert.False(result, "Not April Fools day date is true");

        }

        

        [Fact]
        public void TestGetApiKey()
        {
             
            var inMemorySettings = new Dictionary<string, string?>
            {
            {  BrewCoffeeUtils.WeatherApiKey, "my random key" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

           
            var apiKey = BrewCoffeeUtils.GetApiKey(configuration);

             
            Assert.Equal("my random key", apiKey);
        }


    }



}
