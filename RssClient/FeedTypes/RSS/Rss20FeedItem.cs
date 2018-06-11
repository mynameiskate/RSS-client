using RssClient.FeedTypes.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;

namespace RssClient.FeedTypes.RSS
{
    [Serializable]
    class Rss20FeedItem : DefaultFeedItem
    {

        public string Description { get; set; }

        public string Author { get; set; }

        public string Comments { get; set; }

        public string PublishDateString { get; set; }

        public DateTime? PublishDate { get; set; }

       // public FeedItemSource Source { get; set; }

        public ICollection<string> Categories { get; set; }

        public string Content { get; set; }

        public Rss20FeedItem() { }

        public Rss20FeedItem(XElement item)
            : base(item)
        {
            Comments = item.GetValue("comments");
            Author = item.GetValue("author");
            PublishDateString = item.GetValue("pubDate");
            PublishDate = StringParser.TryParseDateTime(PublishDateString);
           // Source = new FeedItemSource(item.GetElement("source"));
            var categories = item.GetElements("category");
            Categories = categories.Select(x => x.GetValue()).ToList();
            Description = item.GetValue("description");
            Content = item.GetValue("content:encoded");
            if (!string.IsNullOrEmpty(Content))
            {
               Content = HttpUtility.HtmlDecode(Content);
            }
        }

        public override FeedItem ToFeedItem()
        {
            FeedItem item = new FeedItem(this)
            {
                Author = this.Author,
                Categories = this.Categories,
                Content = this.Content,
                Description = this.Description,
                PublishDate = this.PublishDate,
                PublishDateString = this.PublishDateString
            };
            return item;
        }
    }
}
