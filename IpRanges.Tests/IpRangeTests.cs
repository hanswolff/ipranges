using NUnit.Framework;
using System;
using System.Net;

namespace Dedimax.IpRanges.Tests
{
    [TestFixture]
    public class IpRangeTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            IpRange.Parse(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Empty_ThrowsArgumentException()
        {
            IpRange.Parse(string.Empty);
        }

        [Test]
        public void Parse_SingleIP_FromToSame()
        {
            var ip = IPAddress.Parse("1.2.3.4");
            var range = IpRange.Parse(ip.ToString());
            Assert.AreEqual(ip, range.From);
            Assert.AreEqual(ip, range.To);
        }

        [Test]
        [ExpectedException]
        public void Parse_InvalidIP_ThrowsException()
        {
            IPAddress.Parse("257.2.3.4");
        }
    }
}
