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
    /// <summary>
    /// Parent class for map elements.
    /// </summary>
    public class MapElementItem
    {
        /// <summary>Database Id of the element.</summary>
        public int Id { get; set; }

        /// <summary>Element do be displayed on the map</summary>
        public MapElement Element { get; set; }

        /// <summary>The layers that the element is assigned to.</summary>
        public MapLayerItem ParentLayer { get; set; }

        private string _name;

        /// <summary>The name of the element.</summary>
        public string Name
        {
            get { return GetName(); }
            set { SetName(value); }
        }

        /// <summary>
        /// Gets the name of the element. Can be overridden in child classes.
        /// </summary>
        /// <returns>Name string.</returns>
        protected virtual string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Sets the name of the element. Can be overridden in child classes.
        /// </summary>
        /// <param name="newName">New name of the element.</param>
        protected virtual void SetName(string newName)
        {
            _name = newName;
        }

        /// <summary>
        /// Gets a single geoposition that represents the element.
        /// </summary>
        /// <returns>Geographical position.</returns>
        public virtual BasicGeoposition GetPosition()
        {
            return new BasicGeoposition();
        }
    }
}
