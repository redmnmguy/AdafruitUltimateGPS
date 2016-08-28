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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace appAdfUltGPS.Views
{
    /// <summary>
    /// Page that displays the data received from "$GPVTG" (Course and Speed Data) messages sent from the AdaFruit GPS module.
    /// </summary>
    public sealed partial class CourseAndSpeedPage : Page
    {   // Instantiate the View Model
        public CourseAndSpeedPageViewModel CourseAndSpeedPageViewModel { get; set; } = new CourseAndSpeedPageViewModel();
        // Default Constructor
        public CourseAndSpeedPage()
        {
            this.InitializeComponent();
            // Register our "CleanUpViewModel" with the page's Unloaded event so that
            // the view model can unregister its event handlers to the data model.
            this.Unloaded += CleanUpViewModel;
        }
        // Let the ViewModel know that it needs to should do any necessary clean-up before going out of scope
        private void CleanUpViewModel(object sender, RoutedEventArgs e)
        {
            CourseAndSpeedPageViewModel?.CleanUpViewModel();
        }
        // Handle a click event on the left app bar button
        private void appBarButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MinimumNavigationDataPage));
        }
        // Handle a click event on the right app bar button
        private void appBarButtonRight_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SendCommandsPage));
        }
    }

}
