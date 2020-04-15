using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using MapApp.Models;
using MapApp.Services;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MapApp.Controls
{
    public sealed partial class WeatherControl : UserControl
    {
        private WeatherItem _weather;

        public WeatherItem Weather
        {
            get { return _weather; }
            set { SetWeatherItem(value); }
        }

        public WeatherControl()
        {
            this.InitializeComponent();
        }

        private void SetWeatherItem(WeatherItem item)
        {
            _weather = item;

            var path = "";
            if (ThemeSelectorService.Theme == ElementTheme.Light)
            {
                path = "ms-appx:///Assets/WeatherIconsBlack/";
            }
            else
            {
                path = "ms-appx:///Assets/WeatherIconsWhite/";
            }
            var iconFilename = _weather.Icon + ".svg";
            var iconFilePath = Path.Combine(path, iconFilename);
            SvgImageSource source = new SvgImageSource(new Uri(iconFilePath));
            weatherIconImage.Source = source;
            tempTextBlock.Text = _weather.Temp + " \u00B0C";
            pressureTextBlock.Text = _weather.Pressure + " hPa";
            windSpeedTextBlock.Text = _weather.Wind_speed + " m/s";
        }
    }
}
