using Windows.ApplicationModel.DataTransfer;

namespace MapApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DragDropData
    {
        /// <summary>
        /// 
        /// </summary>
        public DataPackageOperation AcceptedOperation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DataPackageView DataView { get; set; }
    }
}
