using System;

namespace IpRanges.DataAquisition.Amazon
{
    class AmazonRegionMapper
    {
        public IPRangesRegion GetIPRangesRegion(string regionId)
        {
            if (regionId == null) throw new ArgumentNullException("regionId");

            regionId = regionId.Trim().ToLowerInvariant();

            switch (regionId)
            {
                case "ap-northeast-1": return new IPRangesRegion("AWS_AsiaPacific_Tokyo", "Asia Pacific (Tokyo)", regionId);
                case "ap-southeast-1": return new IPRangesRegion("AWS_AsiaPacific_Singapore", "Asia Pacific (Singapore)", regionId);
                case "ap-southeast-2": return new IPRangesRegion("AWS_AsiaPacific_Sydney", "Asia Pacific (Sydney)", regionId);
                case "cn-north-1": return new IPRangesRegion("AWS_China_Beijing", "China (Beijing)", regionId);
                case "eu-central-1": return new IPRangesRegion("AWS_EU_Frankfurt", "EU (Frankfurt)", regionId);
                case "eu-west-1": return new IPRangesRegion("AWS_EU_Ireland", "EU (Ireland)", regionId);
                case "global": return new IPRangesRegion("Global", "Global", regionId);
                case "sa-east-1": return new IPRangesRegion("AWS_SouthAmerica_SaoPaulo", "South America (Sao Paulo)", regionId);
                case "us-east-1": return new IPRangesRegion("AWS_US_Virginia", "US East (Northern Virginia)", regionId);
                case "us-gov-west-1": return new IPRangesRegion("AWS_GovCloud", "GovCloud", regionId);
                case "us-west-1": return new IPRangesRegion("AWS_US_NorthernCalifornia", "US West (Northern California)", regionId);
                case "us-west-2": return new IPRangesRegion("AWS_US_Oregon", "US West (Oregon)", regionId);
            }

            return new IPRangesRegion("", "", regionId);
        }
    }
}
