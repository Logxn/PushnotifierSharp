using System;

namespace PushNotfierSharp.Util
{
    public class TimestampConvert
    {
        /* Accepts a unix timestamp and transforms it to its matching date
         * @Param   double   timestamp
         * @return  DateTime realDate
         */
        public static DateTime Convert(double timestamp)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            date = date.AddSeconds(timestamp).ToLocalTime();
            return date;
        }
    }
}
