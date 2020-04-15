using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

using MapApp.Helpers;
using Windows.UI;

namespace MapApp.Models
{
    public class MapPolygonItem : MapElementItem
    {
        public double BorderLength { get; set; }

        public Color StrokeColor { get { return (Color)((Element as MapPolygon)?.StrokeColor); } }

        public Color FillColor { get { return (Color)((Element as MapPolygon)?.FillColor); } }

        public IReadOnlyList<BasicGeoposition> Path { get { return (Element as MapPolygon)?.Paths.First().Positions; } }

        public override BasicGeoposition GetPosition()
        {
            if (Element is MapPolygon)
            {
                return (Element as MapPolygon).Paths.FirstOrDefault().Positions.FirstOrDefault();
            }
            return base.GetPosition();
        }
    }
}
