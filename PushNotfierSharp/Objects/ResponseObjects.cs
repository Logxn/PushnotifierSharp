using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using PushNotfierSharp.Util;

namespace PushNotfierSharp
{
    public partial class DeviceObject
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }


    public partial class LoginObject
    {
        public string username { get; set; }
        public string app_token { get; set; }
        public int expires_at { get; set; }
        public bool success { get; set; } = false;

        public bool isExpired()
        {
            if (DateTime.Now >= TimestampConvert.Convert(expires_at))
                return true;

            return false;
        }  
    }

    public class RefreshObject
    {
        public string app_token { get; set; }
        public int expires_at { get; set; }

        public bool isExpired()
        {
            if (DateTime.Now >= TimestampConvert.Convert(expires_at))
                return true;

            return false;
        }
    }

    public class SuccessObject
    {
        public string device_id { get; set; }
    }

    public class NotificationObject
    {
        public List<SuccessObject> success { get; set; }
        public List<string> error { get; set; }
    }
}
