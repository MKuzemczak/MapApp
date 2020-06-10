using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MapApp.Tests.MSTest.ServicesTests
{
    [TestClass]
    public class WeatherServiceTests
    {
        public struct Triple<T1, T2, T3>
        {
            public T1 Item1;
            public T2 Item2;
            public T3 Item3;
            public Triple(T1 item1, T2 item2, T3 item3)
            { Item1 = item1; Item2 = item2; Item3 = item3; }
        }

        public Dictionary<int, Triple<string, string, string>> WeatherConditions =
            new Dictionary<int, Triple<string, string, string>>()
            {
                {200, new Triple<string, string, string>("Thunderstorm", "thunderstorm with light rain", "11d")},
                {201, new Triple<string, string, string>("Thunderstorm", "thunderstorm with rain", "11d")},
                {202, new Triple<string, string, string>("Thunderstorm", "thunderstorm with heavy rain", "11d")},
                {210, new Triple<string, string, string>("Thunderstorm", "light thunderstorm", "11d")},
                {211, new Triple<string, string, string>("Thunderstorm", "thunderstorm", "11d")},
                {212, new Triple<string, string, string>("Thunderstorm", "heavy thunderstorm", "11d")},
                {221, new Triple<string, string, string>("Thunderstorm", "ragged thunderstorm", "11d")},
                {230, new Triple<string, string, string>("Thunderstorm", "thunderstorm with light drizzle", "11d")},
                {231, new Triple<string, string, string>("Thunderstorm", "thunderstorm with drizzle", "11d")},
                {232, new Triple<string, string, string>("Thunderstorm", "thunderstorm with heavy drizzle", "11d")},
                {300, new Triple<string, string, string>("Drizzle", "light intensity drizzle", "09d")},
                {301, new Triple<string, string, string>("Drizzle", "drizzle", "09d")},
                {302, new Triple<string, string, string>("Drizzle", "heavy intensity drizzle", "09d")},
                {310, new Triple<string, string, string>("Drizzle", "light intensity drizzle rain", "09d")},
                {311, new Triple<string, string, string>("Drizzle", "drizzle rain", "09d")},
                {312, new Triple<string, string, string>("Drizzle", "heavy intensity drizzle rain", "09d")},
                {313, new Triple<string, string, string>("Drizzle", "shower rain and drizzle", "09d")},
                {314, new Triple<string, string, string>("Drizzle", "heavy shower rain and drizzle", "09d")},
                {321, new Triple<string, string, string>("Drizzle", "shower drizzle", "09d")},
                {500, new Triple<string, string, string>("Rain", "light rain", "10d")},
                {501, new Triple<string, string, string>("Rain", "moderate rain", "10d")},
                {502, new Triple<string, string, string>("Rain", "heavy intensity rain", "10d")},
                {503, new Triple<string, string, string>("Rain", "very heavy rain", "10d")},
                {504, new Triple<string, string, string>("Rain", "extreme rain", "10d")},
                {511, new Triple<string, string, string>("Rain", "freezing rain", "13d")},
                {520, new Triple<string, string, string>("Rain", "light intensity shower rain", "09d")},
                {521, new Triple<string, string, string>("Rain", "shower rain", "09d")},
                {522, new Triple<string, string, string>("Rain", "heavy intensity shower rain", "09d")},
                {531, new Triple<string, string, string>("Rain", "ragged shower rain", "09d")},
                {600, new Triple<string, string, string>("Snow", "light snow", "13d")},
                {601, new Triple<string, string, string>("Snow", "Snow", "13d")},
                {602, new Triple<string, string, string>("Snow", "Heavy snow", "13d")},
                {611, new Triple<string, string, string>("Snow", "Sleet", "13d")},
                {612, new Triple<string, string, string>("Snow", "Light shower sleet", "13d")},
                {613, new Triple<string, string, string>("Snow", "Shower sleet", "13d")},
                {615, new Triple<string, string, string>("Snow", "Light rain and snow", "13d")},
                {616, new Triple<string, string, string>("Snow", "Rain and snow", "13d")},
                {620, new Triple<string, string, string>("Snow", "Light shower snow", "13d")},
                {621, new Triple<string, string, string>("Snow", "Shower snow", "13d")},
                {622, new Triple<string, string, string>("Snow", "Heavy shower snow", "13d")},
                {701, new Triple<string, string, string>("Mist", "mist", "50d")},
                {711, new Triple<string, string, string>("Smoke", "Smoke", "50d")},
                {721, new Triple<string, string, string>("Haze", "Haze", "50d")},
                {731, new Triple<string, string, string>("Dust", "sand/ dust whirls", "50d")},
                {741, new Triple<string, string, string>("Fog", "fog", "50d")},
                {751, new Triple<string, string, string>("Sand", "sand", "50d")},
                {761, new Triple<string, string, string>("Dust", "dust", "50d")},
                {762, new Triple<string, string, string>("Ash", "volcanic ash", "50d")},
                {771, new Triple<string, string, string>("Squall", "squalls", "50d")},
                {781, new Triple<string, string, string>("Tornado", "tornado", "50d")},
                {800, new Triple<string, string, string>("Clear", "clear sky", "01d")},
                {801, new Triple<string, string, string>("Clouds", "few clouds", "02d")},
                {802, new Triple<string, string, string>("Clouds", "scattered clouds", "03d")},
                {803, new Triple<string, string, string>("Clouds", "broken clouds", "04d")},
                {804, new Triple<string, string, string>("Clouds", "overcast clouds", "04d")}
            };


        [TestMethod]
        public async Task GetWeatherAsync_ReturnsCorrectWeatherItem()
        {
            double lat = 1, lon = 1;

            var result = await WeatherService.GetWeatherAsync(lat, lon);
            double expectTemp = -20;
            double expectTempDelta = 80;
            double expectPressure = 975;
            double expectPressureDelta = 125;

            Assert.AreEqual(expectTemp, double.Parse(result.Temp), expectTempDelta,
                "Temperature Celcius exceeds earth's surface min/max temperature.");
            Assert.IsTrue(WeatherConditions.ContainsKey(int.Parse(result.Id)));
            Assert.AreEqual(WeatherConditions[int.Parse(result.Id)].Item1, result.Main);
            Assert.AreEqual(WeatherConditions[int.Parse(result.Id)].Item2, result.Description);
            Assert.AreEqual(WeatherConditions[int.Parse(result.Id)].Item3.Take(2).ToString(), result.Icon.Take(2).ToString());
            Assert.AreEqual(expectPressure, double.Parse(result.Pressure), expectPressureDelta,
                "Pressure hPa exceeds earth's surface min/max temperature");
        }
    }
}
