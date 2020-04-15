using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApp.Models
{
    /// <summary>
    /// Stores the data queried from weather api.
    /// </summary>
    public class WeatherItem
    {
        /// <summary>
        /// Weather state id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Main weather state.
        /// </summary>
        public string Main { get; set; }

        /// <summary>
        /// A bit longer weather state.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Icon name of the weather.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Temperature in degrees Celcius.
        /// </summary>
        public string Temp { get; set; }

        /// <summary>
        /// Pressure in hPa.
        /// </summary>
        public string Pressure { get; set; }

        /// <summary>
        /// Wind speed in m/s.
        /// </summary>
        public string Wind_speed { get; set; }
    }
}
