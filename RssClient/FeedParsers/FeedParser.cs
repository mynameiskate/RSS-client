using RssClient.FeedTypes.Default;
using System.Xml.Linq;

namespace RssClient.FeedParsers
{
    internal abstract class FeedParser
    {
        public DefaultFeed Parse(string xmlFeed)
        {
            XDocument document = XDocument.Parse(xmlFeed);
            return Parse(xmlFeed, document);
        }

        public abstract DefaultFeed Parse(string xmlFeed, XDocument document);
    }
}
