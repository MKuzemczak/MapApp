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
    public class MapPolylineItem : MapElementItem
    {
        public double Length { get; private set; }

        public static MapPolylineItem FromMapPolyline(MapPolyline polyline, double width = 0.000001, string name = "", string parentLayerName = "")
        {
            MapPolylineItem result = new MapPolylineItem()
            {
                ParentLayerName = parentLayerName,
                Name = name,
                Length = GeoMath.PolylineLength(polyline.Path.Positions)
            };

            List<BasicGeoposition> left = new List<BasicGeoposition>();
            List<BasicGeoposition> right = new List<BasicGeoposition>();

            var list = polyline.Path.Positions;

            for (int i = 0; i < list.Count - 1; i++)
            {
                BasicGeoposition z = new BasicGeoposition() { Altitude = 1 };
                BasicGeoposition vec = GeoMath.Difference(list[i + 1], list[i]);
                BasicGeoposition prod = GeoMath.CrossProduct(vec, z);
                BasicGeoposition norm = GeoMath.Normalize(prod);
                BasicGeoposition tim = GeoMath.TimesScalar(norm, width);
                right.Add(GeoMath.Sum(list[i], tim));
                right.Add(GeoMath.Sum(list[i + 1], tim));
                BasicGeoposition neg = GeoMath.TimesScalar(tim, -1);
                left.Insert(0, GeoMath.Sum(list[i], neg));
                left.Insert(0, GeoMath.Sum(list[i + 1], neg));
            }

            left.InsertRange(left.Count, right);
            MapPolygon element = new MapPolygon()
            {
                FillColor = polyline.StrokeColor,
                StrokeColor = polyline.StrokeColor,
                ZIndex = polyline.ZIndex,
                Tag = polyline.Tag
            };
            element.Paths.Add(new Geopath(left));
            result.Element = element;

            return result;
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
