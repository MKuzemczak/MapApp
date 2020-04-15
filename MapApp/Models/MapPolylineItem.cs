using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

using MapApp.Helpers;
using Windows.UI;
using System.Collections.ObjectModel;

namespace MapApp.Models
{
    public class MapPolylineItem : MapElementItem
    {
        public double Length { get; set; }

        public double Width { get; set; }

        public Color StrokeColor { get { return (Color)((Element as MapPolygon)?.StrokeColor); } }

        public IReadOnlyList<BasicGeoposition> PolygonRepresentationPath { get { return (Element as MapPolygon)?.Paths.First().Positions; } }

        public IReadOnlyList<BasicGeoposition> Path { get; set; }

        public override BasicGeoposition GetPosition()
        {
            if (Element is MapPolygon)
            {
                return (Element as MapPolygon).Paths.FirstOrDefault().Positions.FirstOrDefault();
            }
            return base.GetPosition();
        }
    }


    [Serializable]
    public class MapElementTypeMismatchException : Exception
    {
        public MapElementTypeMismatchException() { }
        public MapElementTypeMismatchException(string message) : base(message) { }
        public MapElementTypeMismatchException(string message, Exception inner) : base(message, inner) { }
        protected MapElementTypeMismatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
