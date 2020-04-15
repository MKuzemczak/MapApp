using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace MapApp.Models
{
    /// <summary>
    /// Extends <b>MapIcon</b> with additional data needed in app.
    /// </summary>
    public class MapIconItem : MapElementItem
    {
        /// <summary>
        /// Sets the name of the icon, both in-app and displayed on map.
        /// </summary>
        /// <param name="newName">New name.</param>
        protected override void SetName(string newName)
        {
            if (Element is MapIcon)
            {
                (Element as MapIcon).Title = newName;
            }
            base.SetName(newName);
        }

        /// <summary>
        /// Gets a single geoposition that represents the element.
        /// </summary>
        /// <returns>Geographical position.</returns>
        public override BasicGeoposition GetPosition()
        {
            if (Element is MapIcon)
            {
                return (Element as MapIcon).Location.Position;
            }
            return base.GetPosition();
        }
    }
}
