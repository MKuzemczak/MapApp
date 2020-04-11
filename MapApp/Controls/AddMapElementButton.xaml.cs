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
    public sealed partial class AddMapElementButton : UserControl
    {
        private Color _borderColor = Color.FromArgb(255, 255, 255, 255);
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
        public string NameTextBoxContent
        {
            get
            {
                return nameTextBox.Text;
            }
        }



        public ObservableCollection<MapLayerItem> Layers
        {
            get { return (ObservableCollection<MapLayerItem>)GetValue(LayersProperty); }
            set { SetValue(LayersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Layers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayersProperty =
            DependencyProperty.Register("Layers", typeof(ObservableCollection<MapLayerItem>), typeof(AddMapElementButton), new PropertyMetadata(0));



        public ImageSource ContentSource
        {
            get { return (ImageSource)GetValue(ContentSourceProperty); }
            set { SetValue(ContentSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentSourceProperty =
            DependencyProperty.Register("ContentSource", typeof(ImageSource), typeof(AddMapElementButton), new PropertyMetadata(0));


        public bool IsNameTextBoxVisible
        {
            get { return (bool)GetValue(IsNameTextBoxVisibleProperty); }
            set { SetValue(IsNameTextBoxVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsNameTextBoxVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNameTextBoxVisibleProperty =
            DependencyProperty.Register("IsNameTextBoxVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));


        public bool IsBorderColorPickVisible
        {
            get { return (bool)GetValue(IsBorderColorPickVisibleProperty); }
            set { SetValue(IsBorderColorPickVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBorderColorPickVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBorderColorPickVisibleProperty =
            DependencyProperty.Register("IsBorderColorPickVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));


        public bool IsFillColorPickVisible
        {
            get { return (bool)GetValue(IsFillColorPickVisibleProperty); }
            set { SetValue(IsFillColorPickVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFillColorPickVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFillColorPickVisibleProperty =
            DependencyProperty.Register("IsFillColorPickVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));



        public bool IsLayersComboBoxVisible
        {
            get { return (bool)GetValue(IsLayersComboBoxVisibleProperty); }
            set { SetValue(IsLayersComboBoxVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLayersComboBoxVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLayersComboBoxVisibleProperty =
            DependencyProperty.Register("IsLayersComboBoxVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));



        public bool IsWidthNumberBoxVisible
        {
            get { return (bool)GetValue(IsWidthNumberBoxVisibleProperty); }
            set { SetValue(IsWidthNumberBoxVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWidthNumberBoxVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWidthNumberBoxVisibleProperty =
            DependencyProperty.Register("IsWidthNumberBoxVisible", typeof(bool), typeof(AddMapElementButton), new PropertyMetadata(0));



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

        public event EventHandler<AddMapElementButtonFlyoutAddButtonClickedEventArgs> AddClicked;

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

            AddClicked?.Invoke(this,
                new AddMapElementButtonFlyoutAddButtonClickedEventArgs(NameTextBoxContent, BorderColor, FillColor, layersCombo.SelectedItem as MapLayerItem, widthNumberBox.Value));
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

    public class AddMapElementButtonFlyoutAddButtonClickedEventArgs : EventArgs
    {
        public readonly string Name;
        public readonly Color BorderColor;
        public readonly Color FillColor;
        public readonly MapLayerItem Layer;
        public readonly double Width;

        public AddMapElementButtonFlyoutAddButtonClickedEventArgs(string name, Color border, Color fill, MapLayerItem layer, double width)
        {
            Name = name;
            BorderColor = border;
            FillColor = fill;
            Layer = layer;
            Width = width;
        }
    }
}
