using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

using MapApp.Helpers;

namespace MapApp.Models
{
    public class MapPolygonItem : MapElementItem
    {
        public double BorderLength { get; set; }
    }
}
