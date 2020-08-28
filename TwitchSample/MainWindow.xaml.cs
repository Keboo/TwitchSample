using System;
using System.Diagnostics;
using System.Windows;

namespace TwitchSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //This needs to match your apps registered OAuth Redirect URL
        private const string CallbackUrl = "http://localhost:8000";
        private string AccessToken { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            WebView.NavigationStarting += WebView_NavigationStarting;
        }

        private void WebView_NavigationStarting(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationStartingEventArgs e)
        {
            if (e.Uri.Authority == new Uri(CallbackUrl).Authority)
            {
                foreach(var keyValuePair in e.Uri.Fragment.Split('#', '&'))
                {
                    var parts = keyValuePair.Split('=');
                    if (parts.Length != 2) continue;
                    string key = parts[0];
                    string value = parts[1];
                    Debug.WriteLine($"{key}={value}");
                    if (key == "access_token")
                    {
                        AccessToken = value;
                        //NB: The token will be cached with the browser so you wont need to save in your app
                        Dispatcher.Invoke(() => MyAuthToken.Text = AccessToken);
                    }
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string clientId = "you_client_id_here";
            //NB: This assumes your apps callback url is set to localhost
            string url = @$"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&redirect_uri={CallbackUrl}&response_type=token";

            WebView.Navigate(url);
        }
    }
}
