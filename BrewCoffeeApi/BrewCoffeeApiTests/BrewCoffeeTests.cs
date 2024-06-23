using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
 
using System.Net; 

using BrewCoffeeApi;

 
using Microsoft.AspNetCore.Http;
 



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

        [Fact]
        
        public async Task TestRequestCounterIsThreadSafe()
        {
            const int numberOfIncrements = 10000;
            const int numberOfRuns = 5;

            for (int run = 0; run < numberOfRuns; run++)
            {
               
                RequestCounter.Reset();
                Task[] tasks = new Task[numberOfIncrements];

                
                for (int i = 0; i < numberOfIncrements; i++)
                {
                    tasks[i] = Task.Run(() => RequestCounter.Increment());
                }

                await Task.WhenAll(tasks);

                
                Assert.Equal(numberOfIncrements, RequestCounter.GetCount());
            }
        }

        [Fact]
        public async Task TestServiceUnavailable()
        {
            
            // Set the request count to 4 so the next increment will be 5
            RequestCounter.Reset();
            
            for (int i = 0; i < 4; i++)
            {
                RequestCounter.Increment();
            }

            await using var application = new WebApplicationFactory<Program>();
            
            using var client = application.CreateClient();

            
            var response = await client.GetAsync("/brew-coffee");

            
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }


        [Fact]
        public async Task TestWeatherDataResponseWithValidAPIKey()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();

            using var serviceProvider = services.BuildServiceProvider();
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();

                // Define latitude and longitude for Point Cook, Melbourne
                string latitude = "-37.9142";
                string longitude = "144.7506";
                string validApiKey = "9e73141e19c73f9886e8f2ce567a72f2";
                var weatherResponse = await BrewCoffeeUtils.GetWeatherDataAsync(httpClient, latitude, longitude, validApiKey);

                Assert.NotNull(weatherResponse);
                Assert.Contains("main", weatherResponse);
                Assert.Contains("temp", weatherResponse);

            }
        }

        [Fact]
        public async Task TestWeatherDataResponseThrowExceptionWithInvalidAPIKey()
        {
            var services = new ServiceCollection(); 
            
            services.AddHttpClient();
             
            using var serviceProvider = services.BuildServiceProvider(); 
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();

                // Define latitude and longitude for Point Cook, Melbourne
                string latitude = "-37.9142";
                string longitude = "144.7506";
                string invalidApiKey = "Send me data";

                try
                {
                    await BrewCoffeeUtils.GetWeatherDataAsync(httpClient, latitude, longitude, invalidApiKey);
                    Assert.Fail("Expected an exception to be thrown due to invalid API key, but no exception was thrown.");
                }
                catch (Exception ex)
                {
                    Assert.True(ex.Message.Contains("Unauthorized") || ex.Message.Contains("Forbidden"),
                        $"Expected an Unauthorized or Forbidden error, but got: {ex.Message}");
                }



            }

        }

    }



}
