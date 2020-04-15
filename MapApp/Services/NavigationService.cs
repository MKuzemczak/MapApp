using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MapApp.Services
{

    /// <summary>
    /// 
    /// </summary>
    public static class NavigationService
    {
        /// <summary>
        /// 
        /// </summary>
        public static event NavigatedEventHandler Navigated;

        /// <summary>
        /// 
        /// </summary>
        public static event NavigationFailedEventHandler NavigationFailed;

        private static Frame _frame;
        private static object _lastParamUsed;

        /// <summary>
        /// 
        /// </summary>
        public static Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool CanGoBack => Frame.CanGoBack;

        /// <summary>
        /// 
        /// </summary>
        public static bool CanGoForward => Frame.CanGoForward;

        /// <summary>
        /// 
        /// </summary>
        public static bool GoBack()
        {
            if (CanGoBack)
            {
                Frame.GoBack();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void GoForward() => Frame.GoForward();

        /// <summary>
        /// 
        /// </summary>
        public static bool Navigate(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            // Don't open the same page multiple times
            if (Frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParamUsed)))
            {
                var navigationResult = Frame.Navigate(pageType, parameter, infoOverride);
                if (navigationResult)
                {
                    _lastParamUsed = parameter;
                }

                return navigationResult;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null)
            where T : Page
            => Navigate(typeof(T), parameter, infoOverride);

        private static void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated += Frame_Navigated;
                _frame.NavigationFailed += Frame_NavigationFailed;
            }
        }

        private static void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated -= Frame_Navigated;
                _frame.NavigationFailed -= Frame_NavigationFailed;
            }
        }

        private static void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) => NavigationFailed?.Invoke(sender, e);

        private static void Frame_Navigated(object sender, NavigationEventArgs e) => Navigated?.Invoke(sender, e);
    }
}
