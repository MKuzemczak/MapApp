using System.Collections.Generic;

using Windows.ApplicationModel.DataTransfer;

namespace MapApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DragDropStartingData
    {
        /// <summary>
        /// 
        /// </summary>
        public DataPackage Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<object> Items { get; set; }
    }
}
