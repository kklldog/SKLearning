using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SKLearning
{
    public sealed class WeatherPlugin
    {
        [KernelFunction, Description("Gets the weather details of a given location")]
        [return: Description("Weather details")]        
        public static async Task<string> GetWeatherByLocation([Description("name of the location")]string location)
        {
            var key = "6fa6f54d2e5942fea80152410240510";
            var url = @$"http://api.weatherapi.com/v1/current.json?key={key}&q={location}";
            
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine(content);

            return content;
        }
    }
}