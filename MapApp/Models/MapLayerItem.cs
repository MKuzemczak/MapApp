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

        /// <summary>
        /// Compares this object to the object given as argument.
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is MapLayerItem item &&
                   Id == item.Id &&
                   Name == item.Name;
        }

        /// <summary>
        /// Gets hash code of this object.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            int hashCode = -1919740922;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns>True if objects are equal.</returns>
        public static bool operator ==(MapLayerItem l1, MapLayerItem l2)
        {
            if ((l1 is object && l2 is null) ||
                (l1 is null && l2 is object))
                return false;

            if (l1 is null && l2 is null)
                return true;

            return (l1.Name == l2.Name && l1.Id == l2.Id);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns>True if object aren't equal.</returns>
        public static bool operator !=(MapLayerItem l1, MapLayerItem l2)
        {
            if ((l1 is object && l2 is null) ||
                (l1 is null && l2 is object))
                return true;

            if (l1 is null && l2 is null)
                return false;

            return !(l1.Name == l2.Name && l1.Id == l2.Id);
        }


    }
}
