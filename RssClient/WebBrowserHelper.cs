using mshtml;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace RssClient
{
    public static class WebBrowserHelper
    {
        /// <summary>
        /// Body of a WebBrowser
        /// </summary>
        public static readonly DependencyProperty BodyProperty =
                DependencyProperty.RegisterAttached("Body", typeof(string), typeof(WebBrowserHelper), new PropertyMetadata(OnBodyChanged));

        public static string GetBody(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(BodyProperty);
        }

        public static void SetBody(DependencyObject dependencyObject, string body)
        {
            dependencyObject.SetValue(BodyProperty, body);
        }

        private static void OnBodyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = (System.Windows.Controls.WebBrowser)obj;
            string msg = (string)e.NewValue;
            if (string.IsNullOrEmpty(msg))
            {
                msg = "<h3></h3>";
            }
            webBrowser.NavigateToString(msg);
        }

        /// <summary>
        /// Applying CSS styles on document diaplayed in WebBrowser
        /// </summary>
        /// <param name="browser">Current WebBrowser</param>
        /// <param name="style">String containing CSS styles</param>
        public static void ConfigureBrowser(this System.Windows.Controls.WebBrowser browser, string style)
        {
            if (string.IsNullOrEmpty(style))
                return;

            if (browser.Document is HTMLDocument document)
            {
                IHTMLStyleSheet styleSheet = document.createStyleSheet("", 0);
                styleSheet.cssText = style;
            }
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        private const int INTERNET_OPTION_SUPPRESS_BEHAVIOR = 81;
        private const int INTERNET_SUPPRESS_COOKIE_PERSIST = 3;

        /// <summary>
        /// Method for suppressing saving cookies in a WebBrowser
        /// </summary>
        public static void SuppressCookiePersistence()
        {
            var lpBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
            Marshal.StructureToPtr(INTERNET_SUPPRESS_COOKIE_PERSIST, lpBuffer, true);

            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SUPPRESS_BEHAVIOR, lpBuffer, sizeof(int));
            Marshal.FreeCoTaskMem(lpBuffer);
        }

        public static string LoadStyle(string path)
        {
            try
            {
                using (var streamReader = new StreamReader(path))
                {
                    string style = streamReader.ReadToEnd();
                    streamReader.Close();
                    return style;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}