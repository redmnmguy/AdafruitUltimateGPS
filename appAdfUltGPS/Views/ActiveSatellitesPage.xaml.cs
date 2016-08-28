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
using appAdfUltGPS.ViewModels;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace appAdfUltGPS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActiveSatellitesPage : Page
    {
        // Instantiate the View Model
        public ActiveSatellitesPageViewModel ActiveSatellitesPageViewModel { get; set; } = new ActiveSatellitesPageViewModel();
        // Default Constructor
        public  ActiveSatellitesPage()
        {
            this.InitializeComponent();
            // Register our "CleanUpViewModel" with the page's Unloaded event so that
            // the view model can unregister its event handlers to the data model.
            this.Unloaded += CleanUpViewModel;
        }
        // Let the ViewModel know that it needs to should do any necessary clean-up before going out of scope
        private void CleanUpViewModel(object sender, RoutedEventArgs e)
        {
            ActiveSatellitesPageViewModel?.CleanUpViewModel();
        }
        // Handle a click event on the left app bar button
        private void appBarButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FixedDataPage));
        }
        // Handle a click event on the right app bar button
        private void appBarButtonRight_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SatellitesInViewPage));
        }
    }
}
