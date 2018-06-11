using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RssClient.HttpProtocol
{
    public static class HttpHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as string</returns>
        public static async Task<string> DownloadAsync(string url)
        {
            url = WebUtility.UrlDecode(url);
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            }
            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 308) // moved permanently
                {
                    url = response.Headers?.Location?.AbsoluteUri ?? url;
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                }
            }

            return Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
        }

        public static async Task<string> SendRequestAsync(string url)
        {
            url = WebUtility.UrlDecode(url);
            string result = string.Empty;
            var httpRequest = new HttpRequest(url);
            result = await ReadResponseAsync(httpRequest.Request);
            return result;
        }

        private static async Task<string> ReadResponseAsync(WebRequest request)
        {
            string result = string.Empty;
            try
            {
                WebResponse response = await Task.Run(() => request.GetResponseAsync());
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// gets a url (with or without http) and returns the full url
        /// </summary>
        /// <param name="url">url with or without http</param>
        public static string GetAbsoluteUrl(string url)
        {
            return new UriBuilder(url).ToString();
        }


        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <returns>a list of links including the type and title, an empty list if no links are found</returns>
        public static async Task<IEnumerable<string>> GetFeedUrlsFromUrlAsync(string url)
        {
            url = GetAbsoluteUrl(url);
            string pageContent = await DownloadAsync(url);
            var links = ParseFeedUrlsFromHtml(pageContent);
            return links;
        }
        
        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// <returns>a list of links, an empty list if no links are found</returns>
        public static async Task<string[]> ParseFeedUrlsAsStringAsync(string url)
        {
            return (await GetFeedUrlsFromUrlAsync(url)).ToArray();
        } 

        /// <summary>
        /// Parses RSS links from html page and returns all links
        /// </summary>
        /// <param name="htmlContent">the content of the html page</param>
        /// <returns>all RSS/feed links</returns>
        public static IEnumerable<string> ParseFeedUrlsFromHtml(string htmlContent)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument()
            {
                OptionAutoCloseOnEnd = true,
                OptionFixNestedTags = true
            };

            htmlDoc.LoadHtml(htmlContent);

            if (htmlDoc.DocumentNode != null)
            {
                var links = htmlDoc.DocumentNode?.SelectNodes("//link");
                if (links == null)
                    yield break; // no links

                var nodes = links.Where(
                    x => x.Attributes["type"] != null &&
                    (x.Attributes["type"].Value.Contains("application/rss") || x.Attributes["type"].Value.Contains("application/atom")));

                foreach (var node in nodes)
                {
                    string url = HttpUtility.HtmlDecode(node.Attributes["href"]?.Value);
                    yield return url;
                }
            }
        }
    }
}
