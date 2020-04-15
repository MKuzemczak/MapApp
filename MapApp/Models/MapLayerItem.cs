using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApp.Models
{
    /// <summary>
    /// Represents a map layer.
    /// </summary>
    public class MapLayerItem
    {
        /// <summary>
        /// Database Id of the layer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the layer.
        /// </summary>
        public string Name { get; set; }
    }
}
