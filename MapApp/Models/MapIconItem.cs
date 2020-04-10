using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls.Maps;

namespace MapApp.Models
{
    public class MapIconItem : MapElementItem
    {
        protected override string GetName()
        {
            if (Element is MapIcon)
                return (Element as MapIcon).Title;

            return base.GetName();
        }

        protected override void SetName(string newName)
        {
            if (Element is MapIcon)
            {
                (Element as MapIcon).Title = newName;
                return;
            }
            base.SetName(newName);
        }
    }
}
