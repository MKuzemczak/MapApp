using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json.Linq;

using MapApp.Models;

namespace MapApp.Services
{
    public static class WeatherService
    {
        public static async Task<WeatherItem> GetAsync(double lat, double lon)
        {
            //Create an HTTP client object
            HttpClient httpClient = new HttpClient();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }
        

            Uri requestUri = new Uri(
                $"https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&units=metric&appid=d78cc4daa6e2f7b15a9ba394e3bc7b67");

            //Send the GET request asynchronously and retrieve the response as a string.
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            var jobject = Newtonsoft.Json.Linq.JObject.Parse(httpResponseBody);

            var current = jobject["current"];
            var currentWeatherItem = current.ToObject<WeatherItem>();

            var currentWeatherJobjects = jobject["current"]["weather"].Children().ToList();
            var weatherItems = new List<WeatherItem>();
            foreach (var item in currentWeatherJobjects)
            {
                weatherItems.Add(item.ToObject<WeatherItem>());
            }

            if (weatherItems.Count > 0)
            {
                currentWeatherItem.Id = weatherItems[0].Id;
                currentWeatherItem.Main = weatherItems[0].Main;
                currentWeatherItem.Description = weatherItems[0].Description;
                currentWeatherItem.Icon = weatherItems[0].Icon;
                return currentWeatherItem;
            }
            return null;
        }
    }
}
