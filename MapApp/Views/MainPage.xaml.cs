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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await DatabaseAccessService.CreateDatabaseAsync();
            await mapPage.InitializeAsync();
        }

        private void MapPage_MapElementClick(object sender, MapElementClickedEventArgs e)
        {
            splitView.IsPaneOpen = true;
            detailsPage.SelectItem(e.Element);
        }

        private async void DetailsPage_DeleteButtonClick(object sender, EventArgs e)
        {
            await mapPage.DeleteSelectedMapElementItem();
            splitView.IsPaneOpen = false;
        }
    }
}
