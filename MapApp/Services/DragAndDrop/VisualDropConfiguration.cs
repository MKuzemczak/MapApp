using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MapApp.Services.DragAndDrop
{
    /// <summary>
    /// 
    /// </summary>
    public class VisualDropConfiguration : DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(VisualDropConfiguration), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsCaptionVisibleProperty =
            DependencyProperty.Register("IsCaptionVisible", typeof(bool), typeof(VisualDropConfiguration), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsContentVisibleProperty =
            DependencyProperty.Register("IsContentVisible", typeof(bool), typeof(VisualDropConfiguration), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsGlyphVisibleProperty =
            DependencyProperty.Register("IsGlyphVisible", typeof(bool), typeof(VisualDropConfiguration), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DragStartingImageProperty =
            DependencyProperty.Register("DragStartingImage", typeof(ImageSource), typeof(VisualDropConfiguration), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DropOverImageProperty =
            DependencyProperty.Register("DropOverImage", typeof(ImageSource), typeof(VisualDropConfiguration), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCaptionVisible
        {
            get { return (bool)GetValue(IsCaptionVisibleProperty); }
            set { SetValue(IsCaptionVisibleProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsContentVisible
        {
            get { return (bool)GetValue(IsContentVisibleProperty); }
            set { SetValue(IsContentVisibleProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsGlyphVisible
        {
            get { return (bool)GetValue(IsGlyphVisibleProperty); }
            set { SetValue(IsGlyphVisibleProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImageSource DragStartingImage
        {
            get { return (ImageSource)GetValue(DragStartingImageProperty); }
            set { SetValue(DragStartingImageProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImageSource DropOverImage
        {
            get { return (ImageSource)GetValue(DropOverImageProperty); }
            set { SetValue(DropOverImageProperty, value); }
        }
    }
}
