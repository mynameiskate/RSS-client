using RssClient.FeedTypes.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RssClient.FeedTypes.Atom
{
    [Serializable]
    class AtomFeedItem : DefaultFeedItem
    {
        /// <summary>
        /// All "category" elements
        /// </summary>
        public ICollection<string> Categories { get; set; }

        /// <summary>
        /// The "content" element
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The "id" element
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The "published" date as string
        /// </summary>
        public string PublishDateString { get; set; }

        /// <summary>
        /// The "published" element as DateTime. Null if parsing failed or published is empty.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// The "rights" element
        /// </summary>
        public string Rights { get; set; }

        /// <summary>
        /// The "source" element
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The "summary" element
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The "updated" element
        /// </summary>
        public string UpdateDateString { get; set; }

        /// <summary>
        /// The "updated" element as DateTime. Null if parsing failed or updated is empty
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomFeedItem"/> class.
        /// Reads an atom feed based on the xml given in item
        /// </summary>
        /// <param name="item">feed item as xml</param>
        public AtomFeedItem(XElement item)
            : base(item)
        {
            Link = item.GetElement("link").Attribute("href")?.Value;
            HashCode = Link.GetHashCode();
            var categories = item.GetElements("category");
            Categories = categories.Select(x => x.Value.ToString()).ToList();
            Content = item.GetValue("content");// HttpUtility.HtmlDecode(StringParser.GetStringValue(item, "content"));
            Id = item.GetValue("id");
            PublishDateString = item.GetValue("published");
            PublishDate = StringParser.TryParseDateTime(PublishDateString);
            Rights = item.GetValue("rights");
            Source = item.GetValue("source");
            Summary = item.GetValue("summary");
            UpdateDateString = item.GetValue("updated");
            UpdateDate = StringParser.TryParseDateTime(UpdateDateString);
        }

        public AtomFeedItem() { }
        /// <inheritdoc/>
        public override FeedItem ToFeedItem()
        {
            FeedItem item = new FeedItem(this)
            {
                // Author = this.Author?.ToString(),
                Categories = this.Categories,
                Content = this.Content,
                Description = this.Summary,
                Id = this.Id,
                PublishDate = this.PublishDate,
                PublishDateString = this.PublishDateString               
            };
            return item;
        }
    }
}
