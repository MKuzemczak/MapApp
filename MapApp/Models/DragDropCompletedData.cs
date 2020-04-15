using System.Collections.Generic;

using Windows.ApplicationModel.DataTransfer;

namespace MapApp.Models
{
    /// <summary>
    /// Represents the data moved with a completed drag and drop.
    /// </summary>
    public class DragDropCompletedData
    {
        /// <summary>
        /// 
        /// </summary>
        public DataPackageOperation DropResult { get; set; }

        /// <summary>
        /// Items moved with drag and drop
        /// </summary>
        public IReadOnlyList<object> Items { get; set; }
    }
}
