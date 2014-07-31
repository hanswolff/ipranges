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

        [InlineData("127.0.0.1", 1)]
        [InlineData("127.0.0.1/32", 1)]
        [InlineData("127.0.0.1/31", 2)]
        [InlineData("127.0.0.1/24", 256)]
        [InlineData("127.0.0.1/16", 65536)]
        public void Count(string network, long count)
        {
            Assert.Equal((ulong)count, IPRange.Parse(network).Count);
        }

        [InlineData("127.0.0.1", 1)]
        [InlineData("127.0.0.1/24", 256)]
        public void BigCount(string network, long count)
        {
            Assert.Equal(new BigInteger(count), IPRange.Parse(network).BigCount);
        }
    }
}
