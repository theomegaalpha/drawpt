namespace DrawPT.Common.Util
{
    public static class TimezoneHelper
    {
        public static DateTime ConvertToEasternTime(DateTime utcDateTime)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, easternZone);
        }

        public static DateTime Now()
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
        }
    }
}
