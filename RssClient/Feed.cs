using RssClient.FeedTypes;
using RssClient.FeedTypes.Default;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RssClient
{
    public class Feed
    {
        private const string MetaTag = "<meta http-equiv='X-UA-Compatible' charset='UTF-8' content='IE = 10'>";

        public Feed()
        {
            Items = new Hashtable();
            HtmlFeed = string.Empty;
        }

        public Feed(DefaultFeed feed)
        {
            SpecificFeed = feed;
            Title = feed.Title;
            Link = feed.Link;
            Items = new Hashtable();
            foreach (DictionaryEntry item in feed.ItemTable)
            {
                Items.Add(item.Key, ((DefaultFeedItem)item.Value).ToFeedItem());
            }
            //Items = feed.FeedItemList.Select(x => x.ToFeedItem()).ToList();
            HtmlFeed = ConvertToHtml();
            Link = Link;
        }

        /// <summary>
        /// Supported types of feed, e.g. Atom 1.0, RSS 2.0
        /// </summary>
        /// 
        public FeedType Type { get; set; }

        public ICollection<string> Categories { get; set; }

        public string Title { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public string Language { get; set; }

        public string Copyright { get; set; }

        public string LastUpdateDateString { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public string ImageUrl { get; set; }

        public Hashtable Items { get; set; }

        public string HtmlFeed { get; set; }
        /// <summary>
        /// String representation of the original feed 
        /// </summary>
        public string OriginalDocument
        {
            get { return SpecificFeed.OriginalDocument; }
        }

        /// <summary>
        /// The parsed feed element - e.g. of type <see cref="Rss20Feed"/> which contains
        /// e.g. the Generator property which does not exist in others.
        /// </summary>
        public DefaultFeed SpecificFeed { get; set; }

        private string ConvertToHtml()
        {
            string result = string.Empty;
            result += $"<a href='{Link}'><h1>{Title}</h1></a><br>";
            foreach(DictionaryEntry item in Items)
            {
                if (item.Value is FeedItem feedItem)
                {
                    result += $"{feedItem.HtmlFeedItem}<br>";
                }
            }
            return result; 
        }
    }
}