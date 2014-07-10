using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernBrowserApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Events
        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateFromAddressBar();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (!MainFrame.NavigationService.CanGoBack) return;

            MainFrame.NavigationService.GoBack();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (!MainFrame.NavigationService.CanGoForward) return;

            MainFrame.NavigationService.GoForward();
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            // Loading is finished: de-activating the loading bar
            PgbFrameLoading.IsIndeterminate = false;
        }

        private void Application_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Navigate if "Enter" is pressed on the address bar or refresh if F5 is pressed anywhere.
            if (e.Key == Key.F5)
                MainFrame.NavigationService.Refresh();
            else if (sender.Equals(TxbAddressBar) && e.Key == Key.Enter)
                NavigateFromAddressBar();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update the address bar whenever necessary (for previous/next actions and redirections).
            if (MainFrame.Source == null) return;
            
            TxbAddressBar.Text = MainFrame.Source.AbsoluteUri;
        }

        private void MainFrame_OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // An error happened: We display the error page from ressources
            MainFrame.Content = Application.Current.Resources["ErrorPage"];

            // The error is handled and there is no reasons for it to continue.
            // If left to false, the event will continue and raise and exception.
            e.Handled = true;
        }

        #endregion

        #region Helper Methods
        private void NavigateFromAddressBar()
        {
            // Activating loading bar
            PgbFrameLoading.IsIndeterminate = true;
            // Getting text from text box
            var inputText = TxbAddressBar.Text;

            // Checking if protocol is missing (Default to HTTP)
            if (!inputText.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                inputText = "http://" + inputText;

            // Setting the frame new source
            MainFrame.Navigate(new Uri(inputText));

        }
        #endregion

    }
}
