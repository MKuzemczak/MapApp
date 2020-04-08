using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using MapApp.Helpers;
using MapApp.Services;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
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

        private MapElement _tmpMapElement
        {
            get;
            set;
        }
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
            UpdateAddButtonsVisibility();
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

        private void AddMapPolyline(MapPolyline polyline)
        {
            polyline.StrokeColor = Color.FromArgb(255, 255, 0, 0);
            mapControl.MapElements.Add(polyline);
        }

        private void AddMapPolygon(Geopath path)
        {
            MapPolygon polygon = new MapPolygon()
            {
                FillColor = Color.FromArgb(100, 0, 0, 255),
                StrokeColor = Color.FromArgb(200, 0, 0, 255),
                ZIndex = 0
            };
            polygon.Paths.Add(path);
            mapControl.MapElements.Add(polygon);
        }

        private void AddTmpMapPoint(Geopoint position)
        {
            if (_tmpMapElement is MapIcon)
            {
                MapPolyline polyline = new MapPolyline
                {
                    Path = new Geopath(new List<BasicGeoposition>()
                    {
                        new BasicGeoposition()
                        {
                            Longitude = (_tmpMapElement as MapIcon).Location.Position.Longitude,
                            Latitude = (_tmpMapElement as MapIcon).Location.Position.Latitude
                        },
                        new BasicGeoposition()
                        {
                            Longitude = position.Position.Longitude,
                            Latitude = position.Position.Latitude
                        }
                    }),
                    ZIndex = 0,
                    StrokeColor = Color.FromArgb(255, 100,100,100)
                };
                mapControl.MapElements.Remove(_tmpMapElement);
                _tmpMapElement = polyline;
                mapControl.MapElements.Add(_tmpMapElement);

            }
            else if (_tmpMapElement is MapPolyline)
            {
                List<BasicGeoposition> positions = new List<BasicGeoposition>((_tmpMapElement as MapPolyline).Path.Positions)
                {
                    new BasicGeoposition() { Latitude = position.Position.Latitude, Longitude = position.Position.Longitude }
                };
                
                (_tmpMapElement as MapPolyline).Path = new Geopath(positions);
            }
            else if (_tmpMapElement is null)
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
            }
        }

        private void RemoveTmpMapElement()
        {
            if (_tmpMapElement != null)
            {
                mapControl.MapElements.Remove(_tmpMapElement);
                _tmpMapElement = null;
            }
        }

        public void UpdateAddButtonsVisibility()
        {
            addPinButton.Visibility = Visibility.Collapsed;
            addPolylineButton.Visibility = Visibility.Collapsed;
            addPolygonButton.Visibility = Visibility.Collapsed;
            if (_tmpMapElement is MapIcon)
            {
                addPinButton.Visibility = Visibility.Visible;
            }
            else if (_tmpMapElement is MapPolyline)
            {
                addPolylineButton.Visibility = Visibility.Visible;
                addPolygonButton.Visibility = Visibility.Visible;
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
                AddTmpMapPoint(args.Location);
                UpdateAddButtonsVisibility();
            }
            _mapClickWasElementClick = false;
        }

        private void AddPinFlyout_Closing(object sender, object e)
        {
            addMapElementFlyoutTextBox.Text = "";
        }

        private void AddPinFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_tmpMapElement is MapIcon)
            {
                AddMapIcon((_tmpMapElement as MapIcon).Location, addMapElementFlyoutTextBox.Text);
                RemoveTmpMapElement();
                UpdateAddButtonsVisibility();
            }
            addMapElementFlyoutButton.Click -= AddPinFlyoutButton_Click;
            addMapElementFlyout.Hide();
        }

        private void AddPolylineFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_tmpMapElement is MapPolyline)
            {
                AddMapPolyline(_tmpMapElement as MapPolyline);
                RemoveTmpMapElement();
                UpdateAddButtonsVisibility();
            }
            addMapElementFlyoutButton.Click -= AddPolylineFlyoutButton_Click;
            addMapElementFlyout.Hide();
        }

        private void AddPolygonFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_tmpMapElement is MapPolyline)
            {
                AddMapPolygon((_tmpMapElement as MapPolyline).Path);
                RemoveTmpMapElement();
                UpdateAddButtonsVisibility();
            }
            addMapElementFlyoutButton.Click -= AddPolygonFlyoutButton_Click;
            addMapElementFlyout.Hide();
        }

        private void MapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            if (args.MapElements.Count > 0)
            {
                flyoutGrid.ContextFlyout = editMapElementFlyout;
                flyoutGrid.ContextFlyout.ShowAt(flyoutGrid);
                _editedMapElement = args.MapElements[0];

                if (_editedMapElement.GetType() == typeof(MapIcon))
                {
                    editMapElementFlyoutTextBox.Text = (_editedMapElement as MapIcon).Title;
                }

                _mapClickWasElementClick = true;
            }
        }

        private void EditMapElementSaveFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_editedMapElement.GetType() == typeof(MapIcon))
            {
                (_editedMapElement as MapIcon).Title = editMapElementFlyoutTextBox.Text;
            }
        }

        private void EditMapElementDeleteFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mapControl.MapElements.Remove(_editedMapElement);
            editMapElementFlyout.Hide();
        }

        private void EditMapElementFlyout_Closed(object sender, object e)
        {
            editMapElementFlyoutTextBox.Text = "";
        }

        private void EditMapElementFlyoutTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                EditMapElementSaveFlyoutButton_Click(this, new Windows.UI.Xaml.RoutedEventArgs());
            }
        }

        private void OpenAddPolylineFlyout(object sender, RoutedEventArgs e)
        {
            addFlyoutTitleTextBox.Text = "Add polyline";
            addMapElementFlyoutButton.Click += AddPolylineFlyoutButton_Click;
            flyoutGrid.ContextFlyout = addMapElementFlyout;
            addMapElementFlyout.ShowAt(flyoutGrid);
        }

        private void OpenAddPolygonFlyout(object sender, RoutedEventArgs e)
        {
            addFlyoutTitleTextBox.Text = "Add polygon";
            addMapElementFlyoutButton.Click += AddPolygonFlyoutButton_Click;
            flyoutGrid.ContextFlyout = addMapElementFlyout;
            addMapElementFlyout.ShowAt(flyoutGrid);
        }

        private void OpenAddPinFlyout(object sender, RoutedEventArgs e)
        {
            addFlyoutTitleTextBox.Text = "Add pin";
            addMapElementFlyoutButton.Click += AddPinFlyoutButton_Click;
            flyoutGrid.ContextFlyout = addMapElementFlyout;
            addMapElementFlyout.ShowAt(flyoutGrid);
        }
    }
}
