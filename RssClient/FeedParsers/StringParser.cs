using System;
using System.Globalization;

namespace RssClient
{
    public static class StringParser
    {
        /// <summary>
        /// Method for converting string to DateTime
        /// </summary>
        public static DateTime? TryParseDateTime(string dateTime, CultureInfo cultureInfo = null)
        {
            if (string.IsNullOrWhiteSpace(dateTime)) return null;

            var dateTimeFormat = cultureInfo?.DateTimeFormat ?? DateTimeFormatInfo.CurrentInfo;

            if (!DateTimeOffset.TryParse(dateTime, dateTimeFormat, DateTimeStyles.None, out var dt))
            {
                if (dateTime.Contains(","))
                {
                    int pos = dateTime.IndexOf(',') + 1;
                    string newdtstring = dateTime.Substring(pos).Trim();

                    DateTimeOffset.TryParse(newdtstring, dateTimeFormat, DateTimeStyles.None, out dt);
                }
            }

            if (dt == default(DateTimeOffset))
                return null;

            return dt.UtcDateTime;
        }

        /// <summary>
        ///  Method for converting string to DateTime
        /// </summary>
        /// <param name="dateTime">string to convert from</param>
        /// <param name="language">language to convert according to</param>
        public static DateTime? TryParseDateTime(string dateTime, string language)
        {
            CultureInfo culture;
            try
            {
                culture = new CultureInfo(language);
            }
            catch
            {
                culture = null;
            }

            return TryParseDateTime(dateTime, culture);
        }

        /// <summary>
        ///  Method for converting string to int
        /// </summary>
        /// <param name="strNum">number as string</param>
        public static int? TryParseInt(string strNum)
        {
            if (!int.TryParse(strNum, out int result))
                return null;
            return result;
        }
    }
}