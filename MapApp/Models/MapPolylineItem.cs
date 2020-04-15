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
    /// <summary>
    /// Extends <b>MapPolyline</b> with additional data needed in app.
    /// </summary>
    public class MapPolylineItem : MapElementItem
    {
        /// <summary>
        /// Gets or sets the length of the polyline.
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// Gets or sets the width of the polyline.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets the polyline color.
        /// </summary>
        public Color StrokeColor { get { return (Color)((Element as MapPolygon)?.StrokeColor); } }

        /// <summary>
        /// Gets the list of polygon vertices that represent the polyline on map
        /// </summary>
        public IReadOnlyList<BasicGeoposition> PolygonRepresentationPath { get { return (Element as MapPolygon)?.Paths.First().Positions; } }

        /// <summary>
        /// Gets the list of polyline point
        /// </summary>
        public IReadOnlyList<BasicGeoposition> Path { get; set; }

        /// <summary>
        /// Gets a single geoposition that represents the polyline.
        /// </summary>
        /// <returns>Geographical position.</returns>
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
