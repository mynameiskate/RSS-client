using RssClient.SocialMediaHelpers.VK;
using System.Configuration;
using System.Net;
using System.Web;

namespace RssClient.SocialMediaHelpers
{
    public static class VkPoster
    {
        /// <summary>
        /// Method called to construct link to log in
        /// </summary>
        /// <returns></returns>
        public static string Authorize()
        {
            string reqStrTemplate =
              "https://oauth.vk.com/authorize?client_id={0}&scope=offline,wall&redirect_uri={1}&v=5.78&response_type=token";
              return string.Format(reqStrTemplate, ReadSetting("VKAppId"), ReadSetting("VKRedirectUri"));
        }

        /// <summary>
        /// Reading settings from App.config file
        /// </summary>
        /// <param name="key">setting</param>
        public static string ReadSetting(string key)
        {
            string value = string.Empty;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                value = appSettings[key];
                return value;
            }
            catch (ConfigurationErrorsException)
            {
                return value;
            }
        }

        /// <summary>
        /// Getting access token and User ID 
        /// </summary>
        public static void GetVkAccess(string uri)
        {
            uri = uri.Replace("#", "").Trim();
            var parameters = HttpUtility.ParseQueryString(uri);
            VkAccess.AccessToken = parameters.Get("access_token");
            VkAccess.UserId = parameters.Get("user_id");
        }

        /// <summary>
        ///Sharing wall posts
        /// </summary>
        /// <param name="Message">message to post</param>
        /// <param name="Link">link to share</param>
        /// <returns></returns>
        public static string PostMessage(string Message, string Link)
        {
            string reqStr = string.Format(
              "https://api.vk.com/method/wall.post?owner_id={0}&v=5.78&access_token={1}&message={2}",
              VkAccess.UserId, VkAccess.AccessToken, Message);

            if (!string.IsNullOrEmpty(Link))
                reqStr += string.Format("&attachment={0}", HttpUtility.UrlEncode(Link));

            WebClient webClient = new WebClient();
            return webClient.DownloadString(reqStr);
        }
    }
}
