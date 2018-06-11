using RssClient.FeedTypes.Atom;
using RssClient.FeedTypes.Default;
using System.Xml.Linq;

namespace RssClient.FeedParsers
{
    class Atom10FeedParser : FeedParser
    {
        public override DefaultFeed Parse(string xmlFeed, XDocument document)
        {
            AtomFeed feed = new AtomFeed(xmlFeed, document.Root);
            return feed;
        }
    }
}
