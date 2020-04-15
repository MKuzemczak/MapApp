using System;

using MapApp.Models;

using Windows.UI.Xaml;

namespace MapApp.Services.DragAndDrop
{
    /// <summary>
    /// 
    /// </summary>
    public class ListViewDropConfiguration : DropConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DragItemsStartingActionProperty =
            DependencyProperty.Register("DragItemsStartingAction", typeof(Action<DragDropStartingData>), typeof(DropConfiguration), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DragItemsCompletedActionProperty =
            DependencyProperty.Register("DragItemsCompletedAction", typeof(Action<DragDropCompletedData>), typeof(DropConfiguration), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Action<DragDropStartingData> DragItemsStartingAction
        {
            get { return (Action<DragDropStartingData>)GetValue(DragItemsStartingActionProperty); }
            set { SetValue(DragItemsStartingActionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Action<DragDropCompletedData> DragItemsCompletedAction
        {
            get { return (Action<DragDropCompletedData>)GetValue(DragItemsCompletedActionProperty); }
            set { SetValue(DragItemsCompletedActionProperty, value); }
        }
    }
}
