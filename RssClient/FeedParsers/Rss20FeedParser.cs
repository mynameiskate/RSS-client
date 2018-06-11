using RssClient.FeedTypes.Default;
using RssClient.FeedTypes.RSS;
using System.Xml.Linq;

namespace RssClient.FeedParsers
{
    class Rss20FeedParser : FeedParser
    {
        public override DefaultFeed Parse(string xmlFeed, XDocument document)
        {
            XElement root = document.Root;
            var channel = root.GetElement("channel");
            return new Rss20Feed(xmlFeed, channel);
        }
    }
}
