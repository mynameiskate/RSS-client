using RssClient.FeedTypes.Default;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RssClient
{
    [Serializable]
    public class FeedItem
    {
        /// <summary>
        /// Creates FeedItem object from DefaultFeedItem
        /// </summary>
        /// <param name="feedItem">Parsed feed item</param>
        public FeedItem(DefaultFeedItem feedItem)
        {
            Title = feedItem.Title;
            Link = feedItem.Link;
            Categories = new List<string>();
            SpecificItem = feedItem;
            HashCode = feedItem.HashCode;
        }

        /// <summary>
        /// Constructor with no parameters for serialization
        /// </summary>
        public FeedItem()
        {}

        public string Title { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public string PublishDateString { get; set; }

        /// <summary>
        /// Parsed PublishDate as DateTime
        /// </summary>
        public DateTime? PublishDate { get; set; }

        public string Author { get; set; }

        public string Id { get; set; }

        public readonly int HashCode;

        public ICollection<string> Categories { get; set; }

        public string Content { get; set; }

        public DefaultFeedItem SpecificItem { get; set; }

        public string HtmlFeedItem
        {
            get
            {
                return ConvertToHtml();
            }

        }

        public string HtmlArticle
        {
            get
            {
                return GetHtmlArticle();
            }
        }

        private string GetHtmlArticle()
        {
            var item = SpecificItem.ToFeedItem();
            string result = string.Empty;
            result += $"<h2>{Title}</h2><br>";
            result += $"<p>{item.Content}</p><br>";
            result += $"<h3>{item.Author}</h3><br>";
            result += $"<a href='{Link}'><h3>Read from source></h2></a><br>";
            //result = RemoveUnusedTags(result);
            return $"<div>{result}</div>";
        }

        private string GetCategories()
        {
            string categories = string.Empty;
            foreach (string category in Categories)
            {
                categories += $"{category}; ";
            }
            return categories;
        }

        private string ConvertToHtml()
        {
            var item = SpecificItem.ToFeedItem();
            string result = string.Empty;
            result += $"<a href='{Link}'><h2>{Title}</h2></a><br>";
            result += $"<h4>{item.PublishDateString}</h4><br>";
            string categories = GetCategories();
            if (!string.IsNullOrEmpty(categories))
            {
                result += $"<h4>Categories: {categories}</h4><br>";
            }
            result += $"<p>{item.Description}</p><br>";
            result += $"<h3>{item.Author}</h3><br>";
            result = RemoveUnusedTags(result);
            return $"<div>{result}</div>"; 
        }

        private string RemoveUnusedTags(string source)
        {
            return Regex.Replace(source, @"<(\w+)\b(?:\s+[\w\-.:]+(?:\s*=\s*(?:""[^""]*""|'[^']*'|[\w\-.:]+))?)*\s*/?>\s*</\1\s*>", string.Empty, RegexOptions.Multiline);
        }
    }
}