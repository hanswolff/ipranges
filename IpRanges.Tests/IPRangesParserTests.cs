using System;
using System.IO;
using System.Xml;
using Xunit;

namespace IpRanges.Tests
{
    public class IPRangesParserTests
    {
        [Fact]
        public void describe_incorrect_root_element()
        {
            const string xml = "<root></root>";
            Assert.Throws<XmlException>(() => IPRangesParser.ParseFromXml(xml));
        }

        [Fact]
        public void describe_root_element_with_name()
        {
            const string xml = "<group name='test'></group>";
            var group = IPRangesParser.ParseFromXml(xml);

            Assert.Equal("test", group.Name);
            Assert.Equal(0, group.Regions.Count);
        }

        [Fact]
        public void describe_region_element()
        {
            const string xml = "<group><region id='testid' name='testname' description='description' /></group>";
            var group = IPRangesParser.ParseFromXml(xml);

            Assert.Equal(1, group.Regions.Count);
            Assert.Equal("testid", group.Regions[0].Id);
            Assert.Equal("testname", group.Regions[0].Name);
            Assert.Equal("description", group.Regions[0].Description);
        }

        [Fact]
        public void describe_multiple_region_elements()
        {
            const string xml = "<group><region id='test1' /><region id='test2'></region></group>";
            var group = IPRangesParser.ParseFromXml(xml);

            Assert.Equal(2, group.Regions.Count);
            Assert.Equal("test1", group.Regions[0].Id);
            Assert.Equal("test2", group.Regions[1].Id);
        }

        [Fact]
        public void describe_range_element_with_network_attribute_only()
        {
            const string xml = "<group><region><range network='192.168.1.1/16' /></region></group>";
            var group = IPRangesParser.ParseFromXml(xml);

            Assert.Equal(1, group.Regions.Count);
            Assert.Equal(1, group.Regions[0].Ranges.Count);
            var range = group.Regions[0].Ranges[0];
            Assert.Equal("192.168.0.0", range.From.ToString());
            Assert.Equal("192.168.255.255", range.To.ToString());
        }

        [Fact]
        public void describe_range_element_with_network_attribute_and_inconstistent_from_value()
        {
            const string xml = "<group><region><range network='192.168.1.1/16' from='192.168.123.123' /></region></group>";
            Assert.Throws<InvalidDataException>(() => IPRangesParser.ParseFromXml(xml));
        }

        [Fact]
        public void describe_range_element_with_network_attribute_and_inconstistent_to_value()
        {
            const string xml = "<group><region><range network='192.168.1.1/16' to='192.168.123.123' /></region></group>";
            Assert.Throws<InvalidDataException>(() => IPRangesParser.ParseFromXml(xml));
        }

        [Fact]
        public void describe_range_element_with_from_attribute_and_missing_to_value()
        {
            const string xml = "<group><region><range from='192.168.0.0' /></region></group>";
            Assert.Throws<InvalidDataException>(() => IPRangesParser.ParseFromXml(xml));
        }

        [Fact]
        public void describe_range_element_with_to_attribute_and_missing_from_value()
        {
            const string xml = "<group><region><range to='192.168.0.0' /></region></group>";
            Assert.Throws<InvalidDataException>(() => IPRangesParser.ParseFromXml(xml));
        }

        [Fact]
        public void describe_range_element_with_from_and_to_attributes()
        {
            const string xml = "<group><region><range from='192.168.0.1' to='192.168.0.2' /></region></group>";
            var group = IPRangesParser.ParseFromXml(xml);

            Assert.Equal(1, group.Regions.Count);
            Assert.Equal(1, group.Regions[0].Ranges.Count);
            Assert.Same(group, group.Regions[0].ParentGroup);
            IPRange range = group.Regions[0].Ranges[0];
            Assert.Equal("192.168.0.1", range.From.ToString());
            Assert.Equal("192.168.0.2", range.To.ToString());
        }

        [Fact]
        public void describe_multiple_range_elements()
        {
            const string xml = "<group><region><range network='192.1.1.1/16' /><range network='192.2.1.1/16' /></region></group>";
            var group = IPRangesParser.ParseFromXml(xml);

            Assert.Equal(1, group.Regions.Count);
            Assert.Equal(2, group.Regions[0].Ranges.Count);
            IPRange range1 = group.Regions[0].Ranges[0];
            Assert.Equal("192.1.0.0", range1.From.ToString());
            Assert.Equal("192.1.255.255", range1.To.ToString());
            IPRange range2 = group.Regions[0].Ranges[1];
            Assert.Equal("192.2.0.0", range2.From.ToString());
            Assert.Equal("192.2.255.255", range2.To.ToString());
        }

        [Fact]
        public void parse_resources_and_check_consistency()
        {
            var any = false;
            var prefix = typeof(IPRangesParser).Namespace;

            foreach (var group in IPRangesParser.ParseFromResources(prefix + ".Resources"))
            {
                any = true;
                Assert.False(string.IsNullOrEmpty(group.Name));
                Assert.NotEqual(0, group.Regions.Count);

                foreach (var region in group.Regions)
                {
                    Assert.False(string.IsNullOrEmpty(region.Id), String.Format("Region is missing an ID: {0}", region.Name));
                    Assert.NotEqual(0, region.Ranges.Count);
                }
            }

            Assert.True(any);
        }
    }
}
