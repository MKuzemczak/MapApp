using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using MapApp.Helpers;
using MapApp.Services;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace MapApp.Views
{
    public sealed partial class MapPage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set your preferred default zoom level
        private const double DefaultZoomLevel = 17;

        private readonly LocationService _locationService;

        // TODO WTS: Set your preferred default location if a geolock can't be found.
        private readonly BasicGeoposition _defaultPosition = new BasicGeoposition()
        {
            Latitude = 47.609425,
            Longitude = -122.3417
        };

        private double _zoomLevel;

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { Set(ref _zoomLevel, value); }
        }

        private Geopoint _center;

        public Geopoint Center
        {
            get { return _center; }
            set { Set(ref _center, value); }
        }

        private Geopoint _addPinGeoposition = null;
        private MapElement _tmpMapElement = null;
        private MapElement _editedMapElement = null;
        private bool _mapClickWasElementClick = false;

        public MapPage()
        {
            _locationService = new LocationService();
            Center = new Geopoint(_defaultPosition);
            ZoomLevel = DefaultZoomLevel;
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Cleanup();
        }

        public async Task InitializeAsync()
        {
            if (_locationService != null)
            {
                _locationService.PositionChanged += LocationService_PositionChanged;

                var initializationSuccessful = await _locationService.InitializeAsync();

                if (initializationSuccessful)
                {
                    await _locationService.StartListeningAsync();
                }

                if (initializationSuccessful && _locationService.CurrentPosition != null)
                {
                    Center = _locationService.CurrentPosition.Coordinate.Point;
                }
                else
                {
                    Center = new Geopoint(_defaultPosition);
                }
            }

            if (mapControl != null)
            {
                // TODO WTS: Set your map service token. If you don't have one, request from https://www.bingmapsportal.com/
                // mapControl.MapServiceToken = string.Empty;
                AddMapIcon(Center, "Map_YourLocation".GetLocalized());
            }
        }

        public void Cleanup()
        {
            if (_locationService != null)
            {
                _locationService.PositionChanged -= LocationService_PositionChanged;
                _locationService.StopListening();
            }
        }

        private void LocationService_PositionChanged(object sender, Geoposition geoposition)
        {
            if (geoposition != null)
            {
                Center = geoposition.Coordinate.Point;
            }
        }

        private void AddMapIcon(Geopoint position, string title)
        {
            MapIcon mapIcon = new MapIcon()
            {
                Location = position,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = title,
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/map.png")),
                ZIndex = 0
            };
            mapControl.MapElements.Add(mapIcon);
        }

        private MapIcon AddTmpMapIcon(Geopoint position)
        {
            MapIcon mapIcon = new MapIcon()
            {
                Location = position,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = "",
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/tmpMapPin.png")),
                ZIndex = 0
            };
            mapControl.MapElements.Add(mapIcon);

            _tmpMapElement = mapIcon;

            return mapIcon;
        }

        private void RemoveTmpMapIcon()
        {
            if (_tmpMapElement != null)
            {
                mapControl.MapElements.Remove(_tmpMapElement);
                _tmpMapElement = null;
            }
        }

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

        private void MapControl_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            if (!_mapClickWasElementClick)
            {
                //addPinFlyout.ShowAt(rect);
                flyoutGrid.ContextFlyout = this.Resources["AddPinFlyout"] as Flyout;
                flyoutGrid.ContextFlyout.ShowAt(flyoutGrid);
                _addPinGeoposition = args.Location;
                AddTmpMapIcon(_addPinGeoposition);
            }

            _mapClickWasElementClick = false;
        }

        private void AddPinFlyout_Closed(object sender, object e)
        {
            addPinFlyoutTextBox.Text = "";
            RemoveTmpMapIcon();
        }

        private void AddPinButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_addPinGeoposition != null)
            {
                AddMapIcon(_addPinGeoposition, addPinFlyoutTextBox.Text);
                _addPinGeoposition = null;
            }

            flyoutGrid.ContextFlyout.Hide();
        }

        private void MapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            if (args.MapElements.Count > 0)
            {
                flyoutGrid.ContextFlyout = editPinFlyout;
                flyoutGrid.ContextFlyout.ShowAt(flyoutGrid);
                _editedMapElement = args.MapElements[0];

                if (_editedMapElement.GetType() == typeof(MapIcon))
                {
                    editPinFlyoutTextBox.Text = (_editedMapElement as MapIcon).Title;
                }

                _mapClickWasElementClick = true;
            }
        }

        private void EditPinSaveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_editedMapElement.GetType() == typeof(MapIcon))
            {
                (_editedMapElement as MapIcon).Title = editPinFlyoutTextBox.Text;
            }
        }

        private void EditPinDeleteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mapControl.MapElements.Remove(_editedMapElement);
            editPinFlyout.Hide();
        }

        private void EditPinFlyout_Closed(object sender, object e)
        {
            editPinFlyoutTextBox.Text = "";
        }

        private void EditPinFlyoutTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                EditPinSaveButton_Click(this, new Windows.UI.Xaml.RoutedEventArgs());
            }
        }
    }
}
