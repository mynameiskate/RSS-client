using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace RssClient.FeedTypes.Default
{  
    public abstract class DefaultFeed
    {
        public DefaultFeed()
        {
            FeedItemList = new ObservableCollection<DefaultFeedItem>();
            ItemTable = new Hashtable();
        }

        /// <summary>
        /// Creates an istance of the <see cref="DefaultFeed"/> 
        /// using given XML representation of the element.
        /// </summary>
        /// <param name="xmlFeed">XML representation of the feed</param>
        /// <param name="channel">Channel element in the XML</param>
        protected DefaultFeed(string xmlFeed, XElement channel)
        {
            FeedItemList = new ObservableCollection<DefaultFeedItem>();
            ItemTable = new Hashtable();
            OriginalDocument = xmlFeed;
            Title = channel.GetValue("title");
            Link = channel.GetValue("link");
            Element = channel;
        }

        /// <summary>
        /// Generates Feed object
        /// </summary>
        public abstract Feed ConvertToFeed();

        public string Title { get; set; }

        public string Link { get; set; }

        public XElement Element { get; }

        public ICollection<DefaultFeedItem> FeedItemList { get; set; }

        public Hashtable ItemTable { get; set; }

        public string OriginalDocument { get; private set; }
    }
}