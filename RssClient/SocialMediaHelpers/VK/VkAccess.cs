namespace RssClient.SocialMediaHelpers.VK
{
    public static class VkAccess
    {
        public static string AccessToken { get; set; }
        public static string UserId { get; set; }

        public static bool IsAuthorized()
        {
            return !string.IsNullOrEmpty(AccessToken) && 
                   !string.IsNullOrEmpty(UserId);
        }

        public static void LogOut()
        {
            AccessToken = string.Empty;
            UserId = string.Empty;
        }
    }
}
