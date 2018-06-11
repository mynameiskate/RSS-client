using RssClient.FeedTypes.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RssClient.FeedTypes.Atom
{
    /// <summary>
    /// Atom 1.0 feed object according to specification: https://validator.w3.org/feed/docs/atom.html
    /// </summary>
    public class AtomFeed : DefaultFeed
    {
        /* /// <summary>
         /// The "author" element
         /// </summary>
         public AtomPerson Author { get; set; } 

         /// <summary>
         /// The "contributor" element
         /// </summary>
         public AtomPerson Contributor { get; set; } 

         /// <summary>
         /// All "link" elements
         /// </summary>
         public ICollection<AtomLink> Links { get; set; }
 */

        /// <summary>
        /// All "category" elements
        /// </summary>
        public ICollection<string> Categories { get; set; }

        /// <summary>
        /// The "generator" element
        /// </summary>
        public string Generator { get; set; }

        /// <summary>
        /// The "icon" element
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The "id" element
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The "logo" element
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// The "rights" element
        /// </summary>
        public string Rights { get; set; }

        /// <summary>
        /// The "subtitle" element
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// The "updated" element as string
        /// </summary>
        public string UpdateDateString { get; set; }

        /// <summary>
        /// The "updated" element as DateTime. Null if parsing failed of updatedDate is empty.
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomFeed"/> class.
        /// Reads an atom feed based on the xml given in channel
        /// </summary>
        /// <param name="feedXml">the entire feed xml as string</param>
        /// <param name="feed">the feed element in the xml as XElement</param>
        public AtomFeed(string feedXml, XElement feed)
            : base(feedXml, feed)
        {
            Link = feed.GetElement("link").Attribute("href")?.Value;
            var categories = feed.GetElements("category");
            Categories = categories.Select(x => x.GetValue()).ToList();
            Generator = feed.GetValue("generator");
            Icon = feed.GetValue("icon");
            Id = feed.GetValue("id");
            Logo = feed.GetValue("logo");
            Rights = feed.GetValue("rights");
            Subtitle = feed.GetValue("subtitle");
            UpdateDateString = feed.GetValue("updated");
            UpdateDate = StringParser.TryParseDateTime(UpdateDateString);

            var items = feed.GetElements("entry");

            foreach (var item in items)
            {
                var feedItem = new AtomFeedItem(item);
                if (!ItemTable.ContainsKey(feedItem.HashCode))
                {
                    ItemTable.Add(feedItem.HashCode, feedItem);
                }
                //this.FeedItemList.Add(new AtomFeedItem(item));
            }
        }
   

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ConvertToFeed()
        {
            var feed = new Feed(this)
            {
                Categories = this.Categories,
                Copyright = this.Rights,
                Description = null,
                ImageUrl = this.Icon,
                Language = null,
                LastUpdateDate = this.UpdateDate,
                LastUpdateDateString = this.UpdateDateString,
                Type = FeedType.Atom
            };
            return feed;
        }
    }
}
