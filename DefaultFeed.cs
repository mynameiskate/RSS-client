using System;

namespace RssClient.FeedTypes.Default
{
    public abstract class DefaultFeed
    {
        public DefaultFeed()
        {
            this.Items = new ObservableCollection<BaseFeedItem>();
        }

        /// <summary>
        /// Creates an istance of the <see cref="DefaultFeed"/> 
        /// using given XML representation of the element.
        /// </summary>
        /// <param name="xmlFeed">XML representation of the feed</param>
        /// <param name="channel">Channel element in the XML</param>
        protected DefaultFeed(string xmlFeed, XElement channel)
        {
            this.OriginalDocument = xmlFeed;
            this.Title = channel.GetValue("title");
            this.Link = channel.GetValue("link");
            this.Element = channel;
        }

        /// <summary>
        /// Generates Feed object
        /// </summary>
        public abstract Feed ConvertToFeed();

        public string Title { get; set; }

        public string Link { get; set; }

        public ObservableCollection<DefaultFeedItem> FeedItemList { get; set; }

        public string OriginalDocument { get; private set; }
    }
}