using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using MapApp.Models;
using MapApp.Services;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapApp.Views
{
    /// <summary>
    /// Page that displays MapElementItem info.
    /// </summary>
    public sealed partial class DetailsPage : Page, INotifyPropertyChanged
    {
        private object _selectedItem;

        /// <summary>
        /// The currently displayed item.
        /// </summary>
        public object SelectedItem
        {
            get { return _selectedItem; }
            set { Set(ref _selectedItem, value); }
        }

        /// <summary>
        /// Creates a new instance of DetailsPage class.
        /// </summary>
        public DetailsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with data.
        /// </summary>
        /// <param name="item">Item containing data.</param>
        /// <returns></returns>
        public async Task SelectItemAsync(object item)
        {
            SelectedItem = item;
            var pos = (SelectedItem as MapElementItem).GetPosition();
            weatherControl.Weather = await WeatherService.GetWeatherAsync(pos.Latitude, pos.Longitude);
        }

        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Occurs when the delete button is clicked.
        /// </summary>
        public event EventHandler DeleteButtonClick;

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteButtonClick?.Invoke(this, new EventArgs());
        }
    }
}
