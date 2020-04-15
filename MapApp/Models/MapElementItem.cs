using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace MapApp.Models
{
    public class MapElementItem
    {
        public int Id { get; set; }

        public MapElement Element { get; set; }

        public MapLayerItem ParentLayer { get; set; }

        private string _name;

        // get and set methods can be overridden in child classes
        public string Name
        {
            get { return GetName(); }
            set { SetName(value); }
        }

        protected virtual string GetName()
        {
            return _name;
        }

        protected virtual void SetName(string newName)
        {
            _name = newName;
        }

        public virtual BasicGeoposition GetPosition()
        {
            return new BasicGeoposition();
        }
    }
}
