using RssClient.FeedTypes;
using RssClient.HttpProtocol;
using System;
using System.Threading.Tasks;

namespace RssClient
{
    public static class FeedReader
    {
        /// <summary>
        /// gets a url (with or without http) and returns the full url
        /// </summary>
        /// <param name="url">url with or without http</param>
        /// <returns>full url</returns>
        public static string GetAbsoluteUrl(string url)
        {
            return new UriBuilder(url).ToString();
        }

        /// <summary>
        /// reads a feed from an url. the url must be a feed. Use ParseFeedUrlsFromHtml to
        /// parse the feeds from a url which is not a feed.
        /// </summary>
        /// <param name="url">the url to a feed</param>
        /// <returns>parsed feed</returns>
        public static async Task<Feed> ReadAsync(string url)
        {
            string feedContent = await HttpHelper.SendRequestAsync(GetAbsoluteUrl(url));
            return ReadFromString(feedContent);
        }

        /// <summary>
        /// reads a feed from the <paramref name="feedContent" />
        /// </summary>
        /// <param name="feedContent">the feed content (xml)</param>
        /// <returns>parsed feed</returns>
        public static Feed ReadFromString(string feedContent)
        {
            return MainFeedParser.GetFeed(feedContent);
        }
        
        /// <summary>
        /// read the rss feed type from the type statement of an html link
        /// </summary>
        /// <param name="linkType">application/rss+xml or application/atom+xml or ...</param>
        /// <returns>the feed type</returns>
        private static FeedType GetFeedTypeFromLinkType(string linkType)
        {
            if (linkType.Contains("application/rss"))
                return FeedType.Rss;

            if (linkType.Contains("application/atom"))
                return FeedType.Atom;

            return FeedType.Unknown;
        }
    }
}
