using RssClient.FeedTypes.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RssClient.FeedTypes.RSS
{
    /// <summary>
    /// RSS 2.0 feed accoring to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public class Rss20Feed : DefaultFeed
    {
        public string Description { get; set; }

        public string Language { get; set; }

        public string Copyright { get; set; }

        public string Docs { get; set; }

        public string LastBuildDateString { get; set; }

        public DateTime? LastBuildDate { get; set; }

        public string ManagingEditor { get; set; }

        public string PublishDateString { get; set; }

        public DateTime? PublishDate { get; set; }

        public string WebMaster { get; set; }

        public ICollection<string> Categories { get; set; } 

        public string Generator { get; set; }

        public string TTL { get; set; }

        public ICollection<string> SkipDays { get; set; }

        public ICollection<string> SkipHours { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss20Feed"/> class.
        /// Reads a rss 2.0 feed based on the xml given in channel
        /// </summary>
        /// <param name="feedXml">the entire feed xml as string</param>
        /// <param name="channel">the "channel" element in the xml as XElement</param>
        public Rss20Feed(string feedXml, XElement channel)
            : base(feedXml, channel)
        {
            Description = channel.GetValue("description");
            Language = channel.GetValue("language");
            Copyright = channel.GetValue("copyright");
            ManagingEditor = channel.GetValue("managingEditor");
            WebMaster = channel.GetValue("webMaster");
            Docs = channel.GetValue("docs");
            PublishDateString = channel.GetValue("pubDate");
            LastBuildDateString = channel.GetValue("lastBuildDate");
            PublishDate = StringParser.TryParseDateTime(PublishDateString, Language);
            LastBuildDate = StringParser.TryParseDateTime(LastBuildDateString, Language);
            var categories = channel.GetElements("category");
            Categories = categories.Select(x => x.GetValue()).ToList();
            Generator = channel.GetValue("generator");
            TTL = channel.GetValue("ttl");

            var skipHours = channel.GetElement("skipHours");
            if (skipHours != null)
                SkipHours = skipHours.GetElements("hour")?.Select(x => x.GetValue()).ToList();

            var skipDays = channel.GetElement("skipDays");
            if (skipHours != null)
                SkipDays = skipDays.GetElements("day")?.Select(x => x.GetValue()).ToList();

            var items = channel.GetElements("item");

            foreach (var item in items)
            {
                var feedItem = new Rss20FeedItem(item);
                if (!ItemTable.ContainsKey(feedItem.HashCode))
                {
                    ItemTable.Add(feedItem.HashCode, feedItem);
                }
            }
        }

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ConvertToFeed()
        {
            Feed f = new Feed(this)
            {
                Copyright = this.Copyright,
                Categories = this.Categories,
                Description = this.Description,
                Language = this.Language,
                LastUpdateDate = this.LastBuildDate,
                LastUpdateDateString = this.LastBuildDateString,
                Type = FeedType.Rss_20
            };
            return f;
        }
    }
}
