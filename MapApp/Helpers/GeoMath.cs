using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace MapApp.Helpers
{
    public static class GeoMath
    {
        public static double PolygonBorderLength(IReadOnlyList<BasicGeoposition> path)
        {
            if (path.Count < 2)
                return 0;

            double sum = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                sum += CoordinatesToDistanceInMeters(path[i], path[i + 1]);
            }

            sum += CoordinatesToDistanceInMeters(path.First(), path.Last());
            return sum;
        }

        public static double PolylineLength(IReadOnlyList<BasicGeoposition> path)
        {
            if (path.Count < 2)
                return 0;

            double sum = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                sum += CoordinatesToDistanceInMeters(path[i], path[i + 1]);
            }
            return sum;
        }

        public static double CoordinatesToDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6378.137; // Radius of earth in KM
            var dLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            var dLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;
            return d * 1000; // meters
        }

        public static double CoordinatesToDistanceInMeters(BasicGeoposition pos1, BasicGeoposition pos2)
        {
            return CoordinatesToDistanceInMeters(pos1.Latitude, pos1.Longitude, pos2.Latitude, pos2.Longitude);
        }

        public static BasicGeoposition CrossProduct(BasicGeoposition pos1, BasicGeoposition pos2)
        {
            BasicGeoposition result = new BasicGeoposition()
            {
                Longitude = pos1.Latitude * pos2.Altitude - pos1.Altitude * pos2.Latitude,
                Latitude = pos1.Altitude * pos2.Longitude - pos1.Longitude * pos2.Altitude,
                Altitude = pos1.Longitude * pos2.Latitude - pos1.Latitude * pos2.Longitude
            };
            return result;
        }

        public static BasicGeoposition TimesScalar(BasicGeoposition pos, double scalar)
        {
            BasicGeoposition result = new BasicGeoposition()
            {
                Longitude = pos.Longitude * scalar,
                Latitude = pos.Latitude * scalar,
                Altitude = pos.Latitude * scalar
            };
            return result;
        }

        public static double Magnitude(BasicGeoposition pos)
        {
            return Math.Sqrt(Math.Pow(pos.Longitude, 2) + Math.Pow(pos.Latitude, 2) + Math.Pow(pos.Altitude, 2));
        }

        public static BasicGeoposition Normalize(BasicGeoposition pos)
        {
            double magnitude = Magnitude(pos);
            BasicGeoposition result = new BasicGeoposition()
            {
                Longitude = pos.Longitude / magnitude,
                Latitude = pos.Latitude / magnitude,
                Altitude = pos.Altitude / magnitude
            };
            return result;
        }

        public static BasicGeoposition Difference(BasicGeoposition pos1, BasicGeoposition pos2)
        {
            BasicGeoposition result = new BasicGeoposition()
            {
                Longitude = pos1.Longitude - pos2.Longitude,
                Latitude = pos1.Latitude - pos2.Latitude,
                Altitude = pos1.Altitude - pos2.Altitude
            };
            return result;
        }

        public static BasicGeoposition Sum(BasicGeoposition pos1, BasicGeoposition pos2)
        {
            BasicGeoposition result = new BasicGeoposition()
            {
                Longitude = pos1.Longitude + pos2.Longitude,
                Latitude = pos1.Latitude + pos2.Latitude,
                Altitude = pos1.Altitude + pos2.Altitude
            };
            return result;
        }
    }
}
