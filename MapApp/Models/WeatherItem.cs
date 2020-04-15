using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApp.Models
{
    public class WeatherItem
    {
        public string Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public string Temp { get; set; }
        public string Pressure { get; set; }
        public string Wind_speed { get; set; }
    }
}
