using System;
using System.Xml.Linq;

namespace RssClient.FeedTypes.Default
{
    [Serializable]
    public abstract class DefaultFeedItem
    {
        /// <summary>
        ///Creates <see cref="DefaultFeedItem"/>
        ///based on its XML
        /// </summary>
        /// <param name="item">XML representation of item</param>
        protected DefaultFeedItem(XElement item)
        {
            Title = item.GetValue("title");
            Link = item.GetValue("link");
            Element = item;
            HashCode = (Link).GetHashCode();
        }

        public DefaultFeedItem() { }

        public int HashCode { get; set; }

        public string Title { get; set; } // title

        public string Link { get; set; } // link

        public XElement Element { get { return element; } set { element = value; } }
        [NonSerialized]
        private XElement element;

        public abstract FeedItem ToFeedItem();  
    }
}