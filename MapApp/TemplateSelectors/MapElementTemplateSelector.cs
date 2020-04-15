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
    /// <summary>
    /// XAML template selector for displaying map element info.
    /// </summary>
    public class MapElementTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The xaml template displaing <b>MapIconItem</b> info.
        /// </summary>
        public DataTemplate MapIconTemplate { get; set; }

        /// <summary>
        /// The xaml template displaing <b>mapPolylineItem</b> info.
        /// </summary>
        public DataTemplate MapPolylineTemplate { get; set; }

        /// <summary>
        /// The xaml template displaing <b>MapPolygonItem</b> info.
        /// </summary>
        public DataTemplate MapPolygonTemplate { get; set; }

        /// <summary>
        /// Selects appropriate template.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            return GetTemplate(item) ?? base.SelectTemplateCore(item);
        }

        /// <summary>
        /// Selects appropriate template.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
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
