using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

using MapApp.Models;
using MapApp.Helpers;
using Windows.Foundation;

namespace MapApp.Services
{
    public static class MapElementItemFactoryService
    {
        public static MapIconItem GetMapIconItem(string name, BasicGeoposition position, MapLayerItem layer)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be empty", nameof(name));
            }

            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            return new MapIconItem()
            {
                Element = new MapIcon()
                {
                    NormalizedAnchorPoint = new Point(0.5, 1.0),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/map.png")),
                    Location = new Geopoint(position),
                    Title = name,
                    ZIndex = layer.Id
                },
                ParentLayer = layer
            };
        }

        public static MapPolylineItem GetMapPolylineItem(string name, IReadOnlyList<BasicGeoposition> path, MapLayerItem layer, Color strokeColor, double width)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be empty", nameof(name));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            MapPolylineItem result = new MapPolylineItem()
            {
                ParentLayer = layer,
                Name = name,
                Length = GeoMath.PolylineLength(path)
            };

            List<BasicGeoposition> left = new List<BasicGeoposition>();
            List<BasicGeoposition> right = new List<BasicGeoposition>();

            for (int i = 0; i < path.Count - 1; i++)
            {
                BasicGeoposition z = new BasicGeoposition() { Altitude = 1 };
                BasicGeoposition vec = GeoMath.Difference(path[i + 1], path[i]);
                BasicGeoposition prod = GeoMath.CrossProduct(vec, z);
                BasicGeoposition norm = GeoMath.Normalize(prod);
                BasicGeoposition tim = GeoMath.TimesScalar(norm, width);
                right.Add(GeoMath.Sum(path[i], tim));
                right.Add(GeoMath.Sum(path[i + 1], tim));
                BasicGeoposition neg = GeoMath.TimesScalar(tim, -1);
                left.Insert(0, GeoMath.Sum(path[i], neg));
                left.Insert(0, GeoMath.Sum(path[i + 1], neg));
            }

            left.InsertRange(left.Count, right);
            MapPolygon element = new MapPolygon()
            {
                FillColor = strokeColor,
                StrokeColor = strokeColor,
                ZIndex = layer.Id
            };
            element.Paths.Add(new Geopath(left));
            result.Element = element;

            return result;
        }

        public static MapPolygonItem GetMapPolygonItem(string name, IReadOnlyList<BasicGeoposition> path, MapLayerItem layer, Color strokeColor, Color fillColor)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be empty", nameof(name));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            var polygon = new MapPolygon()
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                ZIndex = layer.Id
            };
            polygon.Paths.Add(new Geopath(path));

            return new MapPolygonItem()
            {
                Name = name,
                ParentLayer = layer,
                Element = polygon,
                BorderLength = GeoMath.PolygonBorderLength(path)
            };
        }

    }
}
