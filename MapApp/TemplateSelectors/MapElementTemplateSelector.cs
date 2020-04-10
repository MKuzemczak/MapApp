using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

using MapApp.Models;

namespace MapApp.TemplateSelectors
{
    public class MapElementTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MapIconTemplate { get; set; }
        public DataTemplate MapPolylineTemplate { get; set; }
        public DataTemplate MapPolygonTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return GetTemplate(item) ?? base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return GetTemplate(item) ?? base.SelectTemplateCore(item, container);
        }

        private DataTemplate GetTemplate(object item)
        {
            switch (item)
            {
                case MapIconItem icon:
                    return MapIconTemplate;
                case MapPolygonItem polygon:
                    return MapPolygonTemplate;
                case MapPolylineItem polyline:
                    return MapPolylineTemplate;
            }

            return null;
        }
    }
}
