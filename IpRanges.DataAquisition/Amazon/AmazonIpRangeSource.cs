using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IpRanges.DataAquisition.Amazon
{
    public class AmazonIpRangeSource : IGetRangesGroup
    {
        private IEnumerable<AmazonPrefix> LoadPrefixes()
        {
            using (var webClient = new WebClient())
            {
                var rawData = webClient.DownloadString("https://ip-ranges.amazonaws.com/ip-ranges.json");
                var amazonIpRanges = JsonConvert.DeserializeObject<AmazonIpRanges>(rawData);

                return amazonIpRanges.Prefixes;
            }
        }

        public IPRangesGroup GetRangesGroup()
        {
            var amazonRegionMapper = new AmazonRegionMapper();

            var prefixes = new AmazonIpRangeSource().LoadPrefixes();
            var rangesByRegion =
                prefixes.Select(
                    prefix =>
                    {
                        var regionInformation = amazonRegionMapper.GetIPRangesRegion(prefix.Region);
                        return new
                        {
                            regionInformation.Id,
                            regionInformation.Name,
                            regionInformation.Description,
                            prefix.IpPrefix
                        };
                    })
                    .ToLookup(x => x.Id);

            var regionIds = rangesByRegion.Select(x => x.Key).Distinct().OrderBy(x => x);

            var ipRangesGroup = new IPRangesGroup { Name = "AmazonAWS" };

            foreach (var regionId in regionIds)
            {
                IPRangesRegion ipRangesRegion = null;

                foreach (var range in rangesByRegion[regionId].Distinct())
                {
                    if (ipRangesRegion == null)
                    {
                        ipRangesRegion = new IPRangesRegion
                        {
                            Id = range.Id,
                            Name = range.Name,
                            Description = range.Description
                        };
                        ipRangesGroup.Regions.Add(ipRangesRegion);
                    }

                    var ipRange = IPRange.Parse(range.IpPrefix);
                    ipRangesRegion.Ranges.Add(ipRange);
                }
            }

            return ipRangesGroup;
        }
    }
}
