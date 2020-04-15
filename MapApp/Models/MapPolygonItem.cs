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
    /// <summary>
    /// Extends <b>MapPolygon</b> with additional data needed in app.
    /// </summary>
    public class MapPolygonItem : MapElementItem
    {
        /// <summary>
        /// Gets or sets the border length of the polygon
        /// </summary>
        public double BorderLength { get; set; }

        /// <summary>
        /// Gets the stroke (border) color 
        /// </summary>
        public Color StrokeColor { get { return (Color)((Element as MapPolygon)?.StrokeColor); } }

        /// <summary>
        /// Gets the fill color
        /// </summary>
        public Color FillColor { get { return (Color)((Element as MapPolygon)?.FillColor); } }

        /// <summary>
        /// Gets the list of vertices of the polygon.
        /// </summary>
        public IReadOnlyList<BasicGeoposition> Path { get { return (Element as MapPolygon)?.Paths.First().Positions; } }

        /// <summary>
        /// Gets a single geoposition that represents the element.
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
