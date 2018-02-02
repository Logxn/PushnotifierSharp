using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotfierSharp
{
    public static class Extensions
    {
        public static DateTime toDateTime(this int obj)
        {
            return Util.TimestampConvert.Convert(obj);
        }
    }
}
