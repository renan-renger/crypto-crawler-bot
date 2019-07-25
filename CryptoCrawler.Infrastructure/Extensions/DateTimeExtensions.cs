using System;

namespace CryptoCrawler.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AsUtc(this DateTime date)
        {
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
    }
}
