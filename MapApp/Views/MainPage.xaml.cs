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

using MapApp.DatabaseAccess;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapApp.Views
{
    /// <summary>
    /// Page that brings together MapPage and DetailsPage.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Creates a new instance of MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Called by the <b>NavigationService</b> when the page is navigated to
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await DatabaseAccessService.CreateDatabaseAsync();
            await mapPage.InitializeAsync();
        }

        private async void MapPage_MapElementClick(object sender, MapElementClickedEventArgs e)
        {
            splitView.IsPaneOpen = true;
            await detailsPage.SelectItemAsync(e.Element);
        }

        private async void DetailsPage_DeleteButtonClick(object sender, EventArgs e)
        {
            await mapPage.DeleteSelectedMapElementItem();
            splitView.IsPaneOpen = false;
        }
    }
}
