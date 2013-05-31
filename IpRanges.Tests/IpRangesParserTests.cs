using NUnit.Framework;
using System.IO;
using System.Xml;

namespace Dedimax.IpRanges.Tests
{
    [TestFixture]
    public class IpRangesParserTests
    {
        [Test]
        [ExpectedException(typeof(XmlException))]
        public void describe_incorrect_root_element()
        {
            const string xml = "<root></root>";
            IpRangesParser.ParseFromXml(xml);
        }

        [Test]
        public void describe_root_element_with_name()
        {
            const string xml = "<group name='test'></group>";
            var group = IpRangesParser.ParseFromXml(xml);

            Assert.AreEqual("test", group.Name);
            Assert.AreEqual(0, group.Regions.Count);
        }

        [Test]
        public void describe_region_element()
        {
            const string xml = "<group><region name='test' /></group>";
            var group = IpRangesParser.ParseFromXml(xml);

            Assert.AreEqual(1, group.Regions.Count);
            Assert.AreEqual("test", group.Regions[0].Name);
        }

        [Test]
        public void describe_multiple_region_elements()
        {
            const string xml = "<group><region name='test1' /><region name='test2'></region></group>";
            var group = IpRangesParser.ParseFromXml(xml);

            Assert.AreEqual(2, group.Regions.Count);
            Assert.AreEqual("test1", group.Regions[0].Name);
            Assert.AreEqual("test2", group.Regions[1].Name);
        }

        [Test]
        public void describe_range_element_with_network_attribute_only()
        {
            const string xml = "<group><region><range network='192.168.1.1/16' /></region></group>";
            var group = IpRangesParser.ParseFromXml(xml);

            Assert.AreEqual(1, group.Regions.Count);
            Assert.AreEqual(1, group.Regions[0].Ranges.Count);
            IpRange range = group.Regions[0].Ranges[0];
            Assert.AreEqual("192.168.0.0", range.From.ToString());
            Assert.AreEqual("192.168.255.255", range.To.ToString());
        }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void describe_range_element_with_network_attribute_and_inconstistent_from_value()
        {
            const string xml = "<group><region><range network='192.168.1.1/16' from='192.168.123.123' /></region></group>";
            IpRangesParser.ParseFromXml(xml);
        }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void describe_range_element_with_network_attribute_and_inconstistent_to_value()
        {
            const string xml = "<group><region><range network='192.168.1.1/16' to='192.168.123.123' /></region></group>";
            IpRangesParser.ParseFromXml(xml);
        }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void describe_range_element_with_from_attribute_and_missing_to_value()
        {
            const string xml = "<group><region><range from='192.168.0.0' /></region></group>";
            IpRangesParser.ParseFromXml(xml);
        }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void describe_range_element_with_to_attribute_and_missing_from_value()
        {
            const string xml = "<group><region><range to='192.168.0.0' /></region></group>";
            IpRangesParser.ParseFromXml(xml);
        }

        [Test]
        public void describe_range_element_with_from_and_to_attributes()
        {
            const string xml = "<group><region><range from='192.168.0.1' to='192.168.0.2' /></region></group>";
            var group = IpRangesParser.ParseFromXml(xml);

            Assert.AreEqual(1, group.Regions.Count);
            Assert.AreEqual(1, group.Regions[0].Ranges.Count);
            IpRange range = group.Regions[0].Ranges[0];
            Assert.AreEqual("192.168.0.1", range.From.ToString());
            Assert.AreEqual("192.168.0.2", range.To.ToString());
        }

        [Test]
        public void describe_multiple_range_elements()
        {
            const string xml = "<group><region><range network='192.1.1.1/16' /><range network='192.2.1.1/16' /></region></group>";
            var group = IpRangesParser.ParseFromXml(xml);

            Assert.AreEqual(1, group.Regions.Count);
            Assert.AreEqual(2, group.Regions[0].Ranges.Count);
            IpRange range1 = group.Regions[0].Ranges[0];
            Assert.AreEqual("192.1.0.0", range1.From.ToString());
            Assert.AreEqual("192.1.255.255", range1.To.ToString());
            IpRange range2 = group.Regions[0].Ranges[1];
            Assert.AreEqual("192.2.0.0", range2.From.ToString());
            Assert.AreEqual("192.2.255.255", range2.To.ToString());
        }

        [Test]
        public void parse_resources_and_check_consistency()
        {
            var any = false;
            var prefix = typeof (IpRangesParser).Namespace;
            foreach (var group in IpRangesParser.ParseFromResources(prefix + ".Resources"))
            {
                any = true;
                Assert.IsFalse(string.IsNullOrEmpty(group.Name));
                Assert.AreNotEqual(0, group.Regions.Count);

                foreach (var region in group.Regions)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(region.Name));
                    Assert.AreNotEqual(0, region.Ranges.Count);
                }
            }

            Assert.IsTrue(any);
        }
    }
}
