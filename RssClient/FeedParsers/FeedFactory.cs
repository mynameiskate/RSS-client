using RssClient.FeedTypes;

namespace RssClient.FeedParsers
{
    internal static class FeedFactory
    {
        public static FeedParser GetParser(FeedType type)
        {
            switch(type)
            {
                case (FeedType.Atom):
                {
                    return new Atom10FeedParser();
                }
                case (FeedType.Rss_20):
                {
                    return new Rss20FeedParser();
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}
