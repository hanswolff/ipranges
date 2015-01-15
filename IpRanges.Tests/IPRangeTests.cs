using System;
using System.Net;
using System.Numerics;
using Xunit;
using Xunit.Extensions;

namespace IpRanges.Tests
{
    public class IPRangeTests
    {
        [Fact]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => IPRange.Parse(null));
        }

        [Fact]
        public void Parse_Empty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => IPRange.Parse(string.Empty));
        }

        [Fact]
        public void Parse_SingleIP_FromToSame()
        {
            var ip = IPAddress.Parse("1.2.3.4");
            var range = IPRange.Parse(ip.ToString());
            Assert.Equal(ip, range.From);
            Assert.Equal(ip, range.To);
        }

        [Fact]
        public void Parse_FromLargerThanTo_ReverseFromAndTo()
        {
            var from = IPAddress.Parse("1.2.3.5");
            var to = IPAddress.Parse("1.2.3.4");
            var range = new IPRange(from, to);
            Assert.Equal(to, range.From);
            Assert.Equal(from, range.To);
        }

        [Fact]
        public void Parse_InvalidIP_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => IPRange.Parse("257.2.3.4"));
        }

        [Theory]
        [InlineData("1.1.1.1", "1.1.1.1", "1.1.1.1", "1.1.1.1")]
        [InlineData("1.1.1.1", "1.1.1.100", "1.1.1.50", "1.1.1.150")]
        [InlineData("1.1.1.1", "1.1.1.100", "1.1.1.100", "1.1.1.200")]
        [InlineData("1.1.1.1", "1.1.1.100", "1.1.1.50", "1.1.1.60")]
        [InlineData("1.1.1.50", "1.1.1.150", "1.1.1.1", "1.1.1.100")]
        [InlineData("1.1.1.100", "1.1.1.200", "1.1.1.1", "1.1.1.100")]
        [InlineData("1.1.1.50", "1.1.1.60", "1.1.1.1", "1.1.1.100")]
        public void Overlaps_true(string from1, string to1, string from2, string to2)
        {
            var ipRange1 = new IPRange(IPAddress.Parse(from1), IPAddress.Parse(to1));
            var ipRange2 = new IPRange(IPAddress.Parse(from2), IPAddress.Parse(to2));

            Assert.Equal(true, ipRange1.Overlaps(ipRange2));
        }

        [Theory]
        [InlineData("1.1.1.1", "1.1.1.100", "1.1.1.101", "1.1.1.200")]
        [InlineData("1.1.1.101", "1.1.1.200", "1.1.1.1", "1.1.1.100")]
        public void Overlaps_false(string from1, string to1, string from2, string to2)
        {
            var ipRange1 = new IPRange(IPAddress.Parse(from1), IPAddress.Parse(to1));
            var ipRange2 = new IPRange(IPAddress.Parse(from2), IPAddress.Parse(to2));

            Assert.Equal(false, ipRange1.Overlaps(ipRange2));
        }

        [Theory]
        [InlineData("192.168.1.1", "192.168.1.1", "192.168.1.1")]
        [InlineData("192.168.1.1/32", "192.168.1.1", "192.168.1.1")]
        [InlineData("192.168.1.1/31", "192.168.1.0", "192.168.1.1")]
        [InlineData("192.168.1.1/24", "192.168.1.0", "192.168.1.255")]
        public void Parse(string network, string from, string to)
        {
            var fromIp = IPAddress.Parse(from);
            var toIp = IPAddress.Parse(to);
            var range = IPRange.Parse(network);
            Assert.Equal(fromIp, range.From);
            Assert.Equal(toIp, range.To);
        }

        [Theory]
        [InlineData("192.168.1.1", "192.168.1.1", "192.168.1.1/32")]
        [InlineData("192.168.1.0", "192.168.1.1", "192.168.1.0/31")]
        [InlineData("192.168.1.0", "192.168.1.255", "192.168.1.0/24")]
        [InlineData("46.51.216.0", "46.51.223.255", "46.51.216.0/21")]
        [InlineData("2604:A880::", "2604:A880:FFFF:FFFF:FFFF:FFFF:FFFF:FFFF", "2604:A880::/32")]
        public void GetNetwork(string from, string to, string expectedNetwork)
        {
            var fromIp = IPAddress.Parse(from);
            var toIp = IPAddress.Parse(to);
            var network = new IPRange(fromIp, toIp).GetNetwork();
            Assert.Equal(expectedNetwork, network.ToUpperInvariant());
        }

        [Theory]
        [InlineData("127.0.0.1", 1)]
        [InlineData("127.0.0.1/32", 1)]
        [InlineData("127.0.0.1/31", 2)]
        [InlineData("127.0.0.1/24", 256)]
        [InlineData("127.0.0.1/16", 65536)]
        public void Count(string network, long count)
        {
            Assert.Equal((ulong)count, IPRange.Parse(network).Count);
        }

        [Theory]
        [InlineData("127.0.0.1", 1)]
        [InlineData("127.0.0.1/24", 256)]
        public void BigCount(string network, long count)
        {
            Assert.Equal(new BigInteger(count), IPRange.Parse(network).BigCount);
        }
    }
}
