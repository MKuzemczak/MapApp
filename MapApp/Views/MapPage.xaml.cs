using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using MapApp.DatabaseAccess;
using MapApp.Helpers;
using MapApp.Models;
using MapApp.Services;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
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

        private ObservableCollection<MapLayerItem> MapLayers = new ObservableCollection<MapLayerItem>();
        private List<MapElementItem> MapElements = new List<MapElementItem>();
        private List<MapElement> TmpMapElements = new List<MapElement>();
        public MapElementItem SelectedElement { get; set; }
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
            MapLayers = new ObservableCollection<MapLayerItem>(await DatabaseAccessService.GetLayers());
            var elements = await DatabaseAccessService.GetMapElementItemsAsync();
            foreach (var item in elements)
            {
                AddMapElementItem(item);
            }

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
                // AddMapIcon(Center, "Map_YourLocation".GetLocalized());
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

        private async Task AddMapLayer(string name)
        {
            int newId = (MapLayers.Count == 0) ? 0 : MapLayers.Last().Id + 1;
            MapLayers.Add(new MapLayerItem() { Id = newId, Name = name });
            await DatabaseAccessService.InsertLayer(MapLayers.Last());
        }

        private void AddMapElementItem(MapElementItem item)
        {
            MapElements.Add(item);
            mapControl.MapElements.Add(item.Element);
        }

        private async Task CreateAndAddMapIconAsync(BasicGeoposition position, string title, MapLayerItem layer)
        {
            var newItem = MapElementItemFactoryService.GetMapIconItem(title, position, layer);
            AddMapElementItem(newItem);
            await DatabaseAccessService.InsertMapIconItem(newItem);
        }

        private async Task CreateAndAddMapPolylineAsync(IReadOnlyList<BasicGeoposition> path, Color strokeColor, string name, MapLayerItem layer, double width)
        {
            var newItem = MapElementItemFactoryService.GetMapPolylineItem(name, path, layer, strokeColor, width);
            AddMapElementItem(newItem);
            await DatabaseAccessService.InsertMapPolylineItem(newItem);
        }

        private async Task CreateAndAddMapPolygonAsync(IReadOnlyList<BasicGeoposition> path, Color fillColor, Color strokeColor, string name, MapLayerItem layer)
        {
            var newItem = MapElementItemFactoryService.GetMapPolygonItem(name, path, layer, strokeColor, fillColor);
            AddMapElementItem(newItem);
            await DatabaseAccessService.InsertMapPolygonItem(newItem);
        }

        private async Task DeleteMapElementItem(MapElementItem item)
        {
            mapControl.MapElements.Remove(item.Element);
            MapElements.Remove(item);
            await DatabaseAccessService.DeleteMapElement(item.Id);
        }

        public async Task DeleteSelectedMapElementItem()
        {
            await DeleteMapElementItem(SelectedElement);
        }

        public MapElementItem GetMapElementItemContaining(MapElement element)
        {
            foreach (var item in MapElements)
            {
                if (item.Element == element)
                    return item;
            }
            return null;
        }

        private void AddTmpMapPoint(Geopoint position)
        {
            if (TmpMapElements.Count == 1)
            {
                MapPolyline polyline = new MapPolyline
                {
                    Path = new Geopath(new List<BasicGeoposition>()
                    {
                        new BasicGeoposition()
                        {
                            Longitude = (TmpMapElements[0] as MapIcon).Location.Position.Longitude,
                            Latitude = (TmpMapElements[0] as MapIcon).Location.Position.Latitude
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

                TmpMapElements.Insert(0, polyline);
                mapControl.MapElements.Add(polyline);

            }
            else if (TmpMapElements.Count > 1)
            {
                List<BasicGeoposition> positions = new List<BasicGeoposition>((TmpMapElements[0] as MapPolyline).Path.Positions)
                {
                    new BasicGeoposition() { Latitude = position.Position.Latitude, Longitude = position.Position.Longitude }
                };
                
                (TmpMapElements[0] as MapPolyline).Path = new Geopath(positions);
            }

            MapIcon mapIcon = new MapIcon()
            {
                Location = position,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = "",
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/tmpMapPin.png")),
                ZIndex = 0
            };
            mapControl.MapElements.Add(mapIcon);
            TmpMapElements.Add(mapIcon);
        }

        private void RemoveTmpMapElements()
        {
            foreach (var item in TmpMapElements)
                mapControl.MapElements.Remove(item);
            TmpMapElements.Clear();
        }

        private void RemoveTmpPoint(MapIcon icon)
        {
            TmpMapElements.Remove(icon);
            mapControl.MapElements.Remove(icon);

            var iconPos = icon.Location.Position;

            if (TmpMapElements.Count > 0 && TmpMapElements.First() is MapPolyline)
            {
                if (TmpMapElements.Count == 2)
                {
                    mapControl.MapElements.Remove(TmpMapElements[0]);
                    TmpMapElements.Remove(TmpMapElements[0]);
                    return;
                }
                List<BasicGeoposition> path = new List<BasicGeoposition>((TmpMapElements.First() as MapPolyline).Path.Positions);
                BasicGeoposition itemToRemove = new BasicGeoposition();
                bool found = false;
                foreach (var item in path)
                {
                    if ((Math.Abs(item.Latitude - iconPos.Latitude) < 0.000001 &&
                        (Math.Abs(item.Longitude - iconPos.Longitude) < 0.000001)))
                    {
                        itemToRemove = item;
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    path.Remove(itemToRemove);
                    (TmpMapElements.First() as MapPolyline).Path = new Geopath(path);
                }

            }
        }

        public void UpdateAddButtonsVisibility()
        {
            addPinButton.Visibility = Visibility.Collapsed;
            addPolylineButton.Visibility = Visibility.Collapsed;
            addPolygonButton.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Collapsed;

            if (TmpMapElements.Count > 0)
            {
                cancelButton.Visibility = Visibility.Visible;
                if (TmpMapElements.Count == 1)
                {
                    addPinButton.Visibility = Visibility.Visible;
                }
                else if (TmpMapElements.Count > 1)
                {
                    addPolylineButton.Visibility = Visibility.Visible;

                    if (TmpMapElements.Count > 3)
                    {
                        addPolygonButton.Visibility = Visibility.Visible;
                    }
                }
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

        public event EventHandler<MapElementClickedEventArgs> MapElementClick;

        private void MapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            if (TmpMapElements.Contains(args.MapElements.First()))
            {
                RemoveTmpPoint(args.MapElements.First() as MapIcon);
            }
            else
            {
                SelectedElement = GetMapElementItemContaining(args.MapElements.First());

                MapElementClick?.Invoke(this, new MapElementClickedEventArgs(SelectedElement));
            }
            _mapClickWasElementClick = true;
        }

        //private void EditMapElementSaveFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    //if (_editedMapElement.GetType() == typeof(MapIcon))
        //    //{
        //    //    (_editedMapElement as MapIcon).Title = editMapElementFlyoutTextBox.Text;
        //    //}
        //}

        //private void EditMapElementDeleteFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    // TODO: delete from MapElements
        //    mapControl.MapElements.Remove(_editedMapElement);
        //    editMapElementFlyout.Hide();
        //}

        //private void EditMapElementFlyout_Closed(object sender, object e)
        //{
        //    editMapElementFlyoutTextBox.Text = "";
        //}

        //private void EditMapElementFlyoutTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        //{
        //    if (e.Key == Windows.System.VirtualKey.Enter)
        //    {
        //        EditMapElementSaveFlyoutButton_Click(this, new Windows.UI.Xaml.RoutedEventArgs());
        //    }
        //}

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            int a = 0;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveTmpMapElements();
            UpdateAddButtonsVisibility();
        }

        private async void AddPolylineButton_AddClicked(object sender, Controls.AddMapElementButtonFlyoutAddButtonClickedEventArgs e)
        {
            await CreateAndAddMapPolylineAsync((TmpMapElements.First() as MapPolyline).Path.Positions, e.BorderColor, e.Name, e.Layer, e.Width);
            RemoveTmpMapElements();
            UpdateAddButtonsVisibility();
        }

        private async void AddPolygonButton_AddClicked(object sender, Controls.AddMapElementButtonFlyoutAddButtonClickedEventArgs e)
        {
            await CreateAndAddMapPolygonAsync((TmpMapElements.First() as MapPolyline).Path.Positions, e.FillColor, e.BorderColor, e.Name, e.Layer);
            RemoveTmpMapElements();
            UpdateAddButtonsVisibility();
        }

        private async void AddPinButton_AddClicked(object sender, Controls.AddMapElementButtonFlyoutAddButtonClickedEventArgs e)
        {
            await CreateAndAddMapIconAsync((TmpMapElements.Last() as MapIcon).Location.Position, e.Name, e.Layer);
            RemoveTmpMapElements();
            UpdateAddButtonsVisibility();
        }

        private async void AddLayerFlyoutAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (addLayerTextBox.Text.Any())
            {
                await AddMapLayer(addLayerTextBox.Text);
                addLayerButton.Flyout.Hide();
                addLayerTextBox.Text = "";
                addLayerTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
            }
            else
            {
                addLayerTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
        }
    }

    public class MapElementClickedEventArgs : EventArgs
    {
        public MapElementItem Element { get; }

        public MapElementClickedEventArgs(MapElementItem element) => Element = element;
    }
}
