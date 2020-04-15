using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using MapApp.Models;
using System.Collections.ObjectModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MapApp.Controls
{
    /// <summary>
    /// A custom AddMapElementButton button control.
    /// </summary>
    /// <remarks>
    /// Contains a definition of a flyout that allows user to type in added map element's properties.
    /// Contains properties for storing map element's properties and controlling which items are displayed in the flyout.
    /// </remarks>
    public sealed partial class AddMapElementButton : UserControl
    {
        private Color _borderColor = Color.FromArgb(255, 255, 255, 255);

        /// <summary> Gets or sets the border (stroke) color value of added map element. </summary>
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
                BorderColorBrush.Color = value;
            }
        }

        private Color _fillColor = Color.FromArgb(255, 255, 255, 255);

        /// <summary> Gets or sets the fill color value of added map element. </summary>
        public Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
                FillColorBrush.Color = value;
            }
        }

        private SolidColorBrush BorderColorBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private SolidColorBrush FillColorBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        /// <summary> Gets the name of added map element entered in the flyout's "Name" text box. </summary>
        public string NameTextBoxContent
        {
            get
            {
                return nameTextBox.Text;
            }
        }

        /// <summary> Gets or sets the list of layers diplayed in the flyout's "Layer" combo box. </summary>
        public ObservableCollection<MapLayerItem> Layers
        {
            get { return (ObservableCollection<MapLayerItem>)GetValue(LayersProperty); }
            set { SetValue(LayersProperty, value); }
        }

        /// <summary> Identifies the Layers dependency property. </summary>
        public static readonly DependencyProperty LayersProperty =
            DependencyProperty.Register("Layers", typeof(ObservableCollection<MapLayerItem>), typeof(AddMapElementButton), new PropertyMetadata(0));


        /// <summary> Gets or sets the image (icon) that's displayed as the button's content. </summary>
        public ImageSource ContentSource
        {
            get { return (ImageSource)GetValue(ContentSourceProperty); }
            set { SetValue(ContentSourceProperty, value); }
        }

        /// <summary> Identifies the ContentSource dependency property. </summary>
        public static readonly DependencyProperty ContentSourceProperty =
            DependencyProperty.Register("ContentSource", typeof(ImageSource), typeof(AddMapElementButton), new PropertyMetadata(0));

        /// <summary> Gets or sets the boolean variable that determines the flyout's "Name" text box visibility. </summary>
        public bool IsNameTextBoxVisible
        {
            get { return (bool)GetValue(IsNameTextBoxVisibleProperty); }
            set { SetValue(IsNameTextBoxVisibleProperty, value); }
        }

        /// <summary> Identifies the <code>IsNameTextBoxVisible</code> dependency property. </summary>
        public static readonly DependencyProperty IsNameTextBoxVisibleProperty =
            DependencyProperty.Register("IsNameTextBoxVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));

        /// <summary> Gets or sets the boolean variable that determines the flyout's "Border color" color picker button visibility. </summary>
        public bool IsBorderColorPickVisible
        {
            get { return (bool)GetValue(IsBorderColorPickVisibleProperty); }
            set { SetValue(IsBorderColorPickVisibleProperty, value); }
        }

        /// <summary> Identifies the <code>IsBorderColorPickVisible</code> dependency property. </summary>
        public static readonly DependencyProperty IsBorderColorPickVisibleProperty =
            DependencyProperty.Register("IsBorderColorPickVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));

        /// <summary> Gets or sets the boolean variable that determines the flyout's "Fill color" color picker button visibility. </summary>
        public bool IsFillColorPickVisible
        {
            get { return (bool)GetValue(IsFillColorPickVisibleProperty); }
            set { SetValue(IsFillColorPickVisibleProperty, value); }
        }

        /// <summary> Identifies the <code>IsFillColorPickVisible</code> dependency property. </summary>
        public static readonly DependencyProperty IsFillColorPickVisibleProperty =
            DependencyProperty.Register("IsFillColorPickVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));


        /// <summary> Gets or sets the boolean variable that determines the flyout's "layer" combo box visibility. </summary>
        public bool IsLayersComboBoxVisible
        {
            get { return (bool)GetValue(IsLayersComboBoxVisibleProperty); }
            set { SetValue(IsLayersComboBoxVisibleProperty, value); }
        }

        /// <summary> Identifies the <code>IsLayerComboBoxVisible</code> dependency property. </summary>
        public static readonly DependencyProperty IsLayersComboBoxVisibleProperty =
            DependencyProperty.Register("IsLayersComboBoxVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));


        /// <summary> Gets or sets the boolean variable that determines the flyout's "Width" number box visibility. </summary>
        public bool IsWidthNumberBoxVisible
        {
            get { return (bool)GetValue(IsWidthNumberBoxVisibleProperty); }
            set { SetValue(IsWidthNumberBoxVisibleProperty, value); }
        }

        /// <summary> Identifies the <code>IsWidthNumberBoxVisible</code> dependency property. </summary>
        public static readonly DependencyProperty IsWidthNumberBoxVisibleProperty =
            DependencyProperty.Register("IsWidthNumberBoxVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));


        /// <summary>
        /// Initializes a new instance of the AddMapElementButton class.
        /// </summary>
        public AddMapElementButton()
        {
            IsNameTextBoxVisible = false;
            IsBorderColorPickVisible = false;
            IsFillColorPickVisible = false;
            IsLayersComboBoxVisible = false;
            IsWidthNumberBoxVisible = false;
            this.InitializeComponent();
            
        }

        private void CollapseAll()
        {
            nameTextBlock.Visibility = Visibility.Collapsed;
            nameTextBox.Visibility = Visibility.Collapsed;
            borderColorTextBlock.Visibility = Visibility.Collapsed;
            borderColorPickButton.Visibility = Visibility.Collapsed;
            fillColorTextBlock.Visibility = Visibility.Collapsed;
            fillColorPickButton.Visibility = Visibility.Collapsed;
            layerTextBlock.Visibility = Visibility.Collapsed;
            layersCombo.Visibility = Visibility.Collapsed;
            widthTextBlock.Visibility = Visibility.Collapsed;
            widthNumberBox.Visibility = Visibility.Collapsed;
        }

        private void BorderColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            BorderColor = args.NewColor;
        }

        private void FillColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            FillColor = args.NewColor;
        }

        private void AddMapElementFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {

        }

        /// <summary>
        /// Occurs when the flyout's "Add" button was clicked
        /// </summary>
        public event EventHandler<AddButtonClickedEventArgs> AddButtonClicked;

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsNameTextBoxVisible)
            {
                if (NameTextBoxContent == "")
                {
                    nameTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    return;
                }
                else
                {
                    nameTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
                }
            }
            if (IsLayersComboBoxVisible)
            {
                if (layersCombo.SelectedItem is null)
                {
                    layersCombo.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    return;
                }
                else
                {
                    layersCombo.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
                }
            }
            if (IsWidthNumberBoxVisible)
            {
                if (widthNumberBox.Text == "" || widthNumberBox.Value < 0.000000001)
                {
                    widthNumberBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    return;
                }
                else
                {
                    widthNumberBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
                }
            }

            AddButtonClicked?.Invoke(this,
                new AddButtonClickedEventArgs(NameTextBoxContent, BorderColor, FillColor, layersCombo.SelectedItem as MapLayerItem, widthNumberBox.Value));
            button.Flyout.Hide();
            nameTextBox.Text = "";
        }

        private void AddMapElementFlyout_Opening(object sender, object e)
        {
            CollapseAll();
            if (IsNameTextBoxVisible)
            {
                nameTextBlock.Visibility = Visibility.Visible;
                nameTextBox.Visibility = Visibility.Visible;
            }
            if (IsBorderColorPickVisible)
            {
                borderColorTextBlock.Visibility = Visibility.Visible;
                borderColorPickButton.Visibility = Visibility.Visible;
            }
            if (IsFillColorPickVisible)
            {
                fillColorTextBlock.Visibility = Visibility.Visible;
                fillColorPickButton.Visibility = Visibility.Visible;
            }
            if (IsLayersComboBoxVisible)
            {
                layerTextBlock.Visibility = Visibility.Visible;
                layersCombo.Visibility = Visibility.Visible;
            }
            if (IsWidthNumberBoxVisible)
            {
                widthTextBlock.Visibility = Visibility.Visible;
                widthNumberBox.Visibility = Visibility.Visible;
            }
        }
    }

    /// <summary>
    /// Provides event data for the AddButtonClicked event from AddMapElementButton.
    /// </summary>
    public class AddButtonClickedEventArgs : EventArgs
    {
        /// <summary> Gets the name of the added map element. </summary>
        public readonly string Name;

        /// <summary> Gets the border (stroke) color of the added map element. </summary>
        public readonly Color BorderColor;

        /// <summary> Gets the fill color of the added map element. </summary>
        public readonly Color FillColor;

        /// <summary> Gets the layer of the added map element. </summary>
        public readonly MapLayerItem Layer;

        /// <summary> Gets the width of the added map element. </summary>
        public readonly double Width;

        /// <summary>
        /// Initializes a new instance of the AddButtonClickedEventArgs class.
        /// </summary>
        /// <param name="name">Name of the added map element.</param>
        /// <param name="border">Border (stroke) color of the added map element.</param>
        /// <param name="fill">Fill color of the added map element.</param>
        /// <param name="layer">Layer of the added map element.</param>
        /// <param name="width">Width of the added map element.</param>
        public AddButtonClickedEventArgs(string name, Color border, Color fill, MapLayerItem layer, double width)
        {
            Name = name;
            BorderColor = border;
            FillColor = fill;
            Layer = layer;
            Width = width;
        }
    }
}
