using System.Collections.Generic;

namespace RssClient.Categories
{
    public class Category
    {
        public readonly CategoryType Type;

        public List<string> UrlList { get; set; }
        public List<Feed> FeedList { get; set; }

        public string HtmlCategory
        {
            get
            {
                return GetHtmlCategory();
            }
        }

        public Category(CategoryType type)
        {
            Type = type;
            FeedList = new List<Feed>();
            UrlList = new List<string>();
        }

        public void AddFeed(Feed feed)
        {
            if (feed != null)
            {
                if (!FeedList.Contains(feed))
                {
                    UrlList.Add(feed.Link);
                    FeedList.Add(feed);
                }
            }
        }

        private string GetHtmlCategory()
        {
            string result = string.Empty;
            foreach(Feed feed in FeedList)
            {
                result += feed.HtmlFeed;
            }
            return result;
        }
    }
}
