using Newtonsoft.Json;

namespace IpRanges.DataAquisition.Amazon
{
    class AmazonPrefix
    {
        [JsonProperty("ip_prefix")]
        public string IpPrefix { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("service")]
        public string Service { get; set; }
    }
}
