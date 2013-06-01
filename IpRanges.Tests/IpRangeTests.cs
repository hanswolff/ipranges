using NUnit.Framework;
using System;
using System.Net;
using System.Numerics;

namespace IpRanges.Tests
{
    [TestFixture]
    public class IPRangeTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            IPRange.Parse(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Empty_ThrowsArgumentException()
        {
            IPRange.Parse(string.Empty);
        }

        [Test]
        public void Parse_SingleIP_FromToSame()
        {
            var ip = IPAddress.Parse("1.2.3.4");
            var range = IPRange.Parse(ip.ToString());
            Assert.AreEqual(ip, range.From);
            Assert.AreEqual(ip, range.To);
        }

        [Test]
        public void Parse_FromLargerThanTo_ReverseFromAndTo()
        {
            var from = IPAddress.Parse("1.2.3.5");
            var to = IPAddress.Parse("1.2.3.4");
            var range = new IPRange(from, to);
            Assert.AreEqual(to, range.From);
            Assert.AreEqual(from, range.To);
        }

        [Test]
        [ExpectedException]
        public void Parse_InvalidIP_ThrowsException()
        {
            IPRange.Parse("257.2.3.4");
        }

        [TestCase("192.168.1.1", "192.168.1.1", "192.168.1.1")]
        [TestCase("192.168.1.1/32", "192.168.1.1", "192.168.1.1")]
        [TestCase("192.168.1.1/31", "192.168.1.0", "192.168.1.1")]
        [TestCase("192.168.1.1/24", "192.168.1.0", "192.168.1.255")]
        public void Parse(string network, string from, string to)
        {
            var fromIp = IPAddress.Parse(from);
            var toIp = IPAddress.Parse(to);
            var range = IPRange.Parse(network);
            Assert.AreEqual(fromIp, range.From);
            Assert.AreEqual(toIp, range.To);
        }

        [TestCase("127.0.0.1", 1)]
        [TestCase("127.0.0.1/32", 1)]
        [TestCase("127.0.0.1/31", 2)]
        [TestCase("127.0.0.1/24", 256)]
        [TestCase("127.0.0.1/16", 65536)]
        public void Count(string network, long count)
        {
            Assert.AreEqual(count, IPRange.Parse(network).Count);
        }

        [TestCase("127.0.0.1", 1)]
        [TestCase("127.0.0.1/24", 256)]
        public void BigCount(string network, long count)
        {
            Assert.AreEqual(new BigInteger(count), IPRange.Parse(network).BigCount);
        }
    }
}
