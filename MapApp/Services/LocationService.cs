using System;
using System.Threading.Tasks;

using MapApp.Helpers;

using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;

namespace MapApp.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class LocationService
    {
        private Geolocator _geolocator;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<Geoposition> PositionChanged;

        /// <summary>
        /// 
        /// </summary>
        public Geoposition CurrentPosition { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Task<bool> InitializeAsync()
        {
            return InitializeAsync(100);
        }

        /// <summary>
        /// 
        /// </summary>
        public Task<bool> InitializeAsync(uint desiredAccuracyInMeters)
        {
            return InitializeAsync(desiredAccuracyInMeters, (double)desiredAccuracyInMeters / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> InitializeAsync(uint desiredAccuracyInMeters, double movementThreshold)
        {
            // More about getting location at https://docs.microsoft.com/windows/uwp/maps-and-location/get-location
            if (_geolocator != null)
            {
                _geolocator.PositionChanged -= Geolocator_PositionChanged;
                _geolocator = null;
            }

            var access = await Geolocator.RequestAccessAsync();

            bool result;

            switch (access)
            {
                case GeolocationAccessStatus.Allowed:
                    _geolocator = new Geolocator
                    {
                        DesiredAccuracyInMeters = desiredAccuracyInMeters,
                        MovementThreshold = movementThreshold
                    };
                    result = true;
                    break;
                case GeolocationAccessStatus.Unspecified:
                case GeolocationAccessStatus.Denied:
                default:
                    result = false;
                    break;
            }

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public async Task StartListeningAsync()
        {
            if (_geolocator == null)
            {
                throw new InvalidOperationException("ExceptionLocationServiceStartListeningCanNotBeCalled".GetLocalized());
            }

            _geolocator.PositionChanged += Geolocator_PositionChanged;

            CurrentPosition = await _geolocator.GetGeopositionAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopListening()
        {
            if (_geolocator == null)
            {
                return;
            }

            _geolocator.PositionChanged -= Geolocator_PositionChanged;
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            CurrentPosition = args.Position;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PositionChanged?.Invoke(this, CurrentPosition);
            });
        }
    }
}
