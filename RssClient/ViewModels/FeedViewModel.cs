using RssClient.Categories;
using RssClient.FeedManagement;
using RssClient.HttpProtocol;
using RssClient.SocialMediaHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace RssClient.ViewModels
{
    class FeedViewModel : INotifyPropertyChanged
    {
        private FeedItem _currentFeedItem;
        private const string InternetConnectionError = "<h2>No internet connection.</h2>";
        private const string LoadError = "<h2>An error occurred during load.</h2>";
        private const string EmptyFeed = "<h2>No articles found.</h2>";
        private const string MetaTag = "<meta http-equiv='X-UA-Compatible' charset='UTF-8' content='IE = 10'>";
        private const string SubscriptionPath = @"..\subscriptions.txt";
        private Array CategoryTypes;
        public Hashtable Items { get; set; }
        public Dictionary<string, Category> Categories { get; set; }
        private Hashtable Favourites { get; set; }

        public FeedViewModel()
        {
            Favourites = Serializator.DeserializeList();
            ResetFeed();
            Items = new Hashtable();
            FeedUrls = new ObservableCollection<string>();
            GetSubscriptions();
        }

        public void ReloadFeed()
        {
            ResetFeed();
            GetFeedAsync();
        }

        /// <summary>
        /// Reload current feed.
        /// </summary>
        private void ResetFeed()
        {
            _currentFeedItem = null;
            MainFeed = string.Empty;
            Response = new Feed();
            Article = string.Empty;
            SetCategories();
        }

        /// <summary>
        /// Feed article which is currently open.
        /// </summary>
        private string _article;
        public string Article
        {
            get
            {
                return _article;
            }
            set
            {
                _article = value;
                RaisePropertyChanged("Article");
            }
        }

        public string FavouriteArticles
        {
            get
            {
                string htmlFeed = string.Empty;
                foreach(DictionaryEntry entry in Favourites)
                {
                    if (entry.Value is FeedItem item)
                    {
                        htmlFeed += item.HtmlFeedItem;
                    }   
                }
                return htmlFeed;
            }
        }

        /// <summary>
        /// Urls of loaded feeds
        /// </summary>
        private ObservableCollection<string> _feedUrls;
        public ObservableCollection<string> FeedUrls
        {
            get
            {
                return _feedUrls;
            }
            set
            {
                _feedUrls = value;
                RaisePropertyChanged("FeedUrls");
            }
        }

        /// <summary>
        /// Current feed (consists of feeds from different sources)
        /// </summary>
        private string _mainFeed;
        public string MainFeed
        {
            get
            {
                return _mainFeed;
            }
            set
            {
                _mainFeed = value;
                RaisePropertyChanged("MainFeed");
            }
        }

        /// <summary>
        /// Newly loaded feed from one source.
        /// </summary>
        private Feed _response;
        public Feed Response
        {
            get
            {
                return _response;
            }
            set
            {
                _response = value;
                MainFeed += Response.HtmlFeed;
            }
        }

        /// <summary>
        /// Load saved subscribtions
        /// </summary>
        private void GetSubscriptions()
        {
            CacheHelper.GetSubscriptions(FeedUrls);
        }

        /// <summary>
        /// Save all feed urls to file.
        /// </summary>
        public void SaveSubscriptions()
        {
            CacheHelper.SaveSubscriptions(FeedUrls);
        }

        /// <summary>
        /// Adding new feed if it exists.
        /// </summary>
        /// <param name="url">feed url</param>
        /// <returns></returns>
        public async Task SubscribeToFeedAsync(string url)
        { 
            //getting feed links from HTML page
            string[] links = await HttpHelper.ParseFeedUrlsAsStringAsync(url);
            if (links.Length == 0)
            {
                try
                {
                    await AddNewFeedAsync(url);
                    FeedUrls.Add(url);
                }
                catch
                {
                    throw new Exception("Failed to find feed.\nTry checking the link.");
                }
            }
            else
            {
                //adding all feeds from found links
                foreach (string link in links)
                {
                    if (!FeedUrls.Contains(link))
                    {
                        try
                        {
                            await AddNewFeedAsync(link);
                        }
                        catch
                        {
                            throw new Exception("Couldn't find resource.");
                        }
                        FeedUrls.Add(link);
                    }
                }
            }
        }

        /// <summary>
        /// Loading current subscriptions from saved urls
        /// </summary>
        public async void GetFeedAsync()
        {
            try
            {
                foreach (string url in FeedUrls)
                {
                    await AddNewFeedAsync(url);
                }

                if (string.IsNullOrEmpty(MainFeed))
                {
                    MainFeed = EmptyFeed;
                }
            }
            catch (TimeoutException)
            {
                MainFeed = InternetConnectionError;
            }
            //reading response code from server
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                string msg = string.Empty;
                if (response == null)
                {
                    msg = ex.Message;
                }
                else
                {
                    string status;
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        status = reader.ReadToEnd();
                    }
                    MainFeed = $"<h2>Response Code: {(int)response.StatusCode} : {status}</h2>";
                }
            }
            catch
            {
                MainFeed = LoadError;
            }
        }

        /// <summary>
        /// Load a feed from url and add it to list
        /// </summary>
        private async Task AddNewFeedAsync(string url)
        {
            var response = await LoadFeedFromUrlAsync(url);
            string feed = MetaTag + response.HtmlFeed;
            response.HtmlFeed = feed;
            //Display new feed in main feed
            Response = response;

            foreach (DictionaryEntry entry in Response.Items)
            {
                if (!Items.ContainsKey(entry.Key))
                {
                    Items.Add(entry.Key, entry.Value);
                }
            }
        }

        /// <summary>
        /// Remove feed url from list of Urls
        /// </summary>
        /// <param name="url"></param>
        public void RemoveFeed(string url)
        {
            if (FeedUrls.Contains(url))
            {
                FeedUrls.Remove(url);
            }
        }

        /// <summary>
        /// Load new feed from url and sort it into category
        /// </summary>
        /// <param name="feedUrl">url of feed</param>
        /// <returns></returns>
        private async Task<Feed> LoadFeedFromUrlAsync(string feedUrl)
        {
            Feed response = await Task.Run(() => FeedReader.ReadAsync(feedUrl));
            SortIntoCategory(response);
            return response;    
        } 

        /// <summary>
        /// Add feed into all possible categories
        /// </summary>
        /// <param name="feed"></param>
        private void SortIntoCategory(Feed feed)
        {
            if (feed == null) return;
            foreach(string category in feed.Categories)
            {
                foreach(CategoryType categoryType in CategoryTypes)
                {
                    if (category.Contains(categoryType.ToString()))
                    {
                        Categories[categoryType.ToString()].AddFeed(feed);
                    }
                }
            }
            Categories[CategoryType.All.ToString()].AddFeed(feed);
        }

        /// <summary>
        /// Create list of categories 
        /// </summary>
        private void SetCategories()
        {
            Categories = new Dictionary<string, Category>();
            CategoryTypes = Enum.GetValues(typeof(CategoryType));
            foreach(CategoryType type in CategoryTypes)
            {
                Categories.Add(type.ToString(), new Category(type));
            }
        }

        /// <summary>
        /// Show category
        /// </summary>
        /// <param name="type">category type</param>
        public void SetCategory(string type)
        {
            if (Categories.TryGetValue(type, out Category category))
            {
                MainFeed = category.HtmlCategory;
            }
            else MainFeed = string.Empty;
            
            if (string.IsNullOrEmpty(MainFeed))
            {
                MainFeed = EmptyFeed;
            }
        }

        public void ShowFavourites()
        {
            MainFeed = MetaTag + FavouriteArticles;
        }
        /// <summary>
        /// Show an article
        /// </summary>
        /// <param name="uri">uri of an item</param>
        public void SetCurrentItem(Uri uri)
        {
            int hash = uri.OriginalString.GetHashCode();
            if (!TryGetArticle(Items, hash))
            {
                TryGetArticle(Favourites, hash);
            }
        }

        private bool TryGetArticle(Hashtable table, int hash)
        {
            if (table.Contains(hash))
            {
                if (table[hash] is FeedItem item)
                {
                    _currentFeedItem = item;
                    Article = MetaTag + item.HtmlArticle;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Share article via VK
        /// </summary>
        /// <param name="msg">comment</param>
        public void ShareArticle(string msg)
        {
            try
            {
                if (!string.IsNullOrEmpty(Article) && _currentFeedItem != null)
                {
                    string response = VkPoster.PostMessage(msg, _currentFeedItem.Link.ToString());
                }
                else throw new Exception("Choose an article to share");
            }
            catch
            {
                throw new Exception("Error sending post");
            }
        }

        public string SaveArticle()
        {
            var result = Serializator.BinarySerialize(_currentFeedItem);
            if (!Favourites.Contains(_currentFeedItem.HashCode))
            {
                Favourites.Add(_currentFeedItem.HashCode, _currentFeedItem);
            }
           // Categories[CategoryType.Favourites.ToString()].(_currentFeedItem);
            return result;
        }

        public bool RemoveArticle()
        {
            bool result = false;
            if (_currentFeedItem != null)
            {
                int hash = _currentFeedItem.HashCode;
                if (Favourites.Contains(_currentFeedItem.HashCode))
                {
                    Favourites.Remove(hash);
                    result = _currentFeedItem.RemoveFromFavourites();
                    MainFeed = MetaTag + FavouriteArticles;
                }
            }
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}