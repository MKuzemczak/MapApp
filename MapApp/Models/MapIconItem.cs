using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace MapApp.Models
{
    public class MapIconItem : MapElementItem
    {

        protected override void SetName(string newName)
        {
            if (Element is MapIcon)
            {
                (Element as MapIcon).Title = newName;
            }
            base.SetName(newName);
        }

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
