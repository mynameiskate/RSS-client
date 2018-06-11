using RssClient.FeedParsers;
using RssClient.FeedTypes;
using System;
using System.Xml.Linq;

namespace RssClient
{
    internal static class MainFeedParser
    {
        /// <summary>
        /// Getting feed type from document
        /// </summary>
        public static FeedType GetFeedType(XDocument doc)
        {
            string rootElement = doc.Root.Name.LocalName;

            if (rootElement.Equals("feed", StringComparison.OrdinalIgnoreCase))
                return FeedType.Atom;

            if (rootElement.Equals("rss", StringComparison.OrdinalIgnoreCase))
            {
                string version = doc.Root.Attribute("version").Value;
                if (version.Equals("2.0", StringComparison.OrdinalIgnoreCase))
                {
                    return FeedType.Rss_20;
                }
            }
            return FeedType.Unknown;
        }

        /// <summary>
        /// Returns the parsed feed
        /// </summary>
        /// <param name="feedContent">the feed document</param>
        /// <returns>parsed feed</returns>
        public static Feed GetFeed(string feedContent)
        {
            ///Replacing special chars
            feedContent = feedContent.Replace(((char)0x1C).ToString(), string.Empty); 
            feedContent = feedContent.Replace(((char)65279).ToString(), string.Empty); 

            XDocument document = XDocument.Parse(feedContent);

            var feedType = GetFeedType(document);
            var parser = FeedFactory.GetParser(feedType);
            if (parser != null)
            {
                var feed = parser.Parse(feedContent);
                return feed.ConvertToFeed();
            }
            else
            {
                throw new Exception("Feed is not supported");
            }
  
        }
    }
}