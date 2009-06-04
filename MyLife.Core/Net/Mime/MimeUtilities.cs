using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyLife.Net.Mime
{
    public static class MimeUtilities
    {
        public static DateTime ParseRfc2822Date(string date)
        {
            date = date.ToLower();
            date = date.Replace("bst", "+0100");
            date = date.Replace("gmt", "-0000");
            date = date.Replace("edt", "-0400");
            date = date.Replace("est", "-0500");
            date = date.Replace("cdt", "-0500");
            date = date.Replace("cst", "-0600");
            date = date.Replace("mdt", "-0600");
            date = date.Replace("mst", "-0700");
            date = date.Replace("pdt", "-0700");
            date = date.Replace("pst", "-0800");

            var parsedDateTime = DateTime.MinValue;

            var r =
                new Regex(
                    @"(?:(?:Mon|Tue|Wed|Thu|Fri|Sat|Sun), )?(?<DateTime>\d{1,2} (?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{4} \d{2}\:\d{2}(?:\:\d{2})?)(?: (?<TimeZone>[\+-]\d{4}))?",
                    RegexOptions.IgnoreCase);
            var m = r.Match(date);
            if (m.Success)
            {
                var dateTime = m.Groups["DateTime"].Value.TrimStart('0');
                parsedDateTime = DateTime.ParseExact(dateTime, new[] {"d MMM yyyy hh:mm", "d MMM yyyy hh:mm:ss"},
                                                     CultureInfo.InvariantCulture, DateTimeStyles.None);

                var timeZone = m.Groups["TimeZone"].Value;
                if (timeZone.Length == 5)
                {
                    var hour = Int32.Parse(timeZone.Substring(0, 3));
                    var minute = Int32.Parse(timeZone.Substring(3));
                    var offset = new TimeSpan(hour, minute, 0);
                    parsedDateTime = new DateTimeOffset(parsedDateTime, offset).UtcDateTime;
                }
            }
            return parsedDateTime;
        }
    }
}