using Newtonsoft.Json;

namespace IpRanges.DataAquisition.Amazon
{
    class AmazonIpRanges
    {
        [JsonProperty("syncToken")]
        public string SyncToken { get; set; }

        [JsonProperty("createDate")]
        public string CreateDate { get; set; }

        [JsonProperty("prefixes")]
        public AmazonPrefix[] Prefixes { get; set; }
    }
}
