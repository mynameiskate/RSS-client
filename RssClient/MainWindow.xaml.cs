using RssClient.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System;
using RssClient.SocialMediaHelpers;
using System.Reflection;
using RssClient.SocialMediaHelpers.VK;

namespace RssClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _message = string.Empty;
        private FeedViewModel feedViewModel;
        private const string FeedStylePath = @"..\feed_styles.css";
        private const string BrowserStylePath = @"..\browser_styles.css";
        private const string ShareInfo = "Enter your comment to post";
        private const string SubscribeInfo = "Enter URL of website or feed.";
        private readonly string FeedStyle;
        private readonly string BrowserStyle;
        public string Message
        {
            get
            {
                return ResultTextBlock.Text;
            }
            set
            {
                ResultTextBlock.Text = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            feedViewModel = new FeedViewModel();
            WebBrowserHelper.SuppressCookiePersistence();
            DataContext = feedViewModel;
            ResultTextBlock.DataContext = this;
            feedViewModel.GetFeedAsync();
            FeedStyle = WebBrowserHelper.LoadStyle(FeedStylePath);
            BrowserStyle = WebBrowserHelper.LoadStyle(BrowserStylePath);
            FeedCombobox.ItemsSource = feedViewModel.FeedUrls;
            Application.Current.MainWindow.Closed += new EventHandler(MainWindow_Closed);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            feedViewModel.SaveSubscriptions();
        }

        private void SideBar_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sideBar.Visibility == Visibility.Collapsed)
            {
                sideBar.Visibility = Visibility.Visible;
                (sender as Button).Content = "<";
            }
            else
            {
                sideBar.Visibility = Visibility.Collapsed;
                (sender as Button).Content = ">";
            }
        }

        private void FeedWebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (sender is WebBrowser webBrowser)
            {
                webBrowser.ConfigureBrowser(FeedStyle);
            } 
        }

        private void FeedWebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                feedViewModel.SetCurrentItem(e.Uri);
                e.Cancel = true;
            }
        }

        private void LinkWebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
           dynamic activeX = linkWebBrowser.GetType().InvokeMember("ActiveXInstance",
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, linkWebBrowser, new object[] { });
            activeX.Silent = true; 
        }

        private void LinkWebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (sender is WebBrowser webBrowser)
            {
                webBrowser.ConfigureBrowser(BrowserStyle);
            }
        }

        private void FavouriteItem_Click(object sender, RoutedEventArgs e)
        {
            feedViewModel.ShowFavourites();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            feedViewModel.RemoveArticle();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                string header = menuItem.Header.ToString();
                if (!string.IsNullOrEmpty(header))
                {
                    feedViewModel.SetCategory(header);
                }
            }
        }

        private void Subscribe_Button_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(InputBox);
            InfoTextBlock.Text = SubscribeInfo;
            SubmitButton.Visibility = Visibility.Visible;
            SendButton.Visibility = Visibility.Collapsed;
        }

        private async void SubmitButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text;
            try
            {
                await feedViewModel.SubscribeToFeedAsync(input);
                Message = "New feed was successfully added.";
            }
            catch(Exception ex)
            {
                Message = ex.Message;
            }
            HideGrid(InputBox);
            ShowGrid(ResultBox);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                feedViewModel.SaveArticle();
                Message = "Article is now in favourites.";
            }
            catch(Exception ex)
            {
                Message = ex.Message;
            }
            ShowGrid(ResultBox);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            HideGrid(InputBox);
            shareWebBrowser.Visibility = Visibility.Hidden;
        }

        private void HideGrid(Grid grid)
        {
            grid.Visibility = Visibility.Collapsed;
        }

        private void ShowGrid(Grid grid)
        {
            grid.Visibility = Visibility.Visible;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            HideGrid(ResultBox);
            Message = string.Empty;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string url = FeedCombobox.Text;
            if (!string.IsNullOrEmpty(url))
            {
                feedViewModel.RemoveFeed(url);
            }
            HideGrid(UnsubscribeBox);
        }

        private void CancelDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            HideGrid(UnsubscribeBox);
        }

        private void UnsubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(UnsubscribeBox);
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            feedViewModel.ReloadFeed();
        }

        private void ShareWebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            VkPoster.GetVkAccess(e.Uri.Fragment.Replace("#", "").Trim());
            if (VkAccess.IsAuthorized())
            {
                try
                {
                    feedViewModel.ShareArticle(_message);
                    Message = "Success";
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                }
                HideGrid(InputBox);
                shareWebBrowser.Visibility = Visibility.Hidden;
                ShowGrid(ResultBox);
                VkAccess.LogOut();
                WebBrowserHelper.SuppressCookiePersistence();
            }            
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            _message = InputTextBox.Text;
            shareWebBrowser.Visibility = Visibility.Visible;
            shareWebBrowser.Navigate(VkPoster.Authorize());
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(InputBox);
            SendButton.Visibility = Visibility.Visible;
            SubmitButton.Visibility = Visibility.Collapsed;
            InfoTextBlock.Text = ShareInfo;
        }
    }
}
