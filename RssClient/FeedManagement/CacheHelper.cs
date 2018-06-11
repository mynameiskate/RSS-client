using System.Collections.Generic;
using System.IO;
using System.Web;

namespace RssClient.FeedManagement
{
    public static class CacheHelper
    {
        private const string SubscriptionPath = @"..\subscriptions.txt";

        /// <summary>
        /// Load saved subscribtions
        /// </summary>
        public static ICollection<string> GetSubscriptions(ICollection<string> urlList)
        {
            try
            {
                if (File.Exists(SubscriptionPath))
                {
                    using (var reader = new StreamReader(SubscriptionPath))
                    {
                        string line = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            urlList.Add(HttpUtility.UrlPathEncode(line));
                        }
                        reader.Close();
                    }
                }
                return urlList;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Save all feed urls to file.
        /// </summary>
        public static bool SaveSubscriptions(ICollection<string> urlList)
        {
            try
            {
                if (File.Exists(SubscriptionPath))
                {
                    using (var writer = new StreamWriter(SubscriptionPath))
                    {
                        foreach (string url in urlList)
                        {
                            writer.WriteLine(url);
                        }
                        writer.Close();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
