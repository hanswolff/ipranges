using NUnit.Framework;
using System.Net;

namespace IpRanges.Tests
{
    [TestFixture]
    public class IPAddressComparerTests
    {
        [Test]
        public void Compare_NullNull_0()
        {
            Assert.AreEqual(0, new IPAddressComparer().Compare(null, null));
        }

        [Test]
        public void Compare_NullAny_Minus1()
        {
            Assert.AreEqual(-1, new IPAddressComparer().Compare(null, IPAddress.Any));
        }

        [Test]
        public void Compare_NullAny_1()
        {
            Assert.AreEqual(1, new IPAddressComparer().Compare(IPAddress.Any, null));
        }

        [Test]
        public void Compare_IPv4SameAsIPv4_0()
        {
            Assert.AreEqual(0, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("127.0.0.1")));
        }

        [Test]
        public void Compare_IPv4LargerThanIPv4_1()
        {
            Assert.AreEqual(1, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.2"), IPAddress.Parse("127.0.0.1")));
        }

        [Test]
        public void Compare_IPv4LargerThanIPv4LeastVsMostSignificant_1()
        {
            Assert.AreEqual(1, new IPAddressComparer().Compare(IPAddress.Parse("128.0.0.1"), IPAddress.Parse("127.0.0.2")));
        }

        [Test]
        public void Compare_IPv4SmallerThanIPv4_Minus1()
        {
            Assert.AreEqual(-1, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("127.0.0.2")));
        }

        [Test]
        public void Compare_IPv4SmallerThanIPv4LeastVsMostSignificant_Minus1()
        {
            Assert.AreEqual(-1, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.2"), IPAddress.Parse("128.0.0.1")));
        }

        [Test]
        public void Compare_IPv6SameAsIPv6_0()
        {
            Assert.AreEqual(0, new IPAddressComparer().Compare(IPAddress.Parse("fe80::1"), IPAddress.Parse("fe80::1")));
        }

        [Test]
        public void Compare_IPv6LargerThanIPv6_1()
        {
            Assert.AreEqual(1, new IPAddressComparer().Compare(IPAddress.Parse("fe80::2"), IPAddress.Parse("fe80::1")));
        }

        [Test]
        public void Compare_IPv6LargerThanIPv6LeastVsMostSignificant_1()
        {
            Assert.AreEqual(1, new IPAddressComparer().Compare(IPAddress.Parse("fe81::1"), IPAddress.Parse("fe80::2")));
        }

        [Test]
        public void Compare_IPv6SmallerThanIPv6_Minus1()
        {
            Assert.AreEqual(-1, new IPAddressComparer().Compare(IPAddress.Parse("fe80::1"), IPAddress.Parse("fe80::2")));
        }

        [Test]
        public void Compare_IPv6SmallerThanIPv6LeastVsMostSignificant_Minus1()
        {
            Assert.AreEqual(-1, new IPAddressComparer().Compare(IPAddress.Parse("fe80::2"), IPAddress.Parse("fe81::1")));
        }

        [Test]
        public void Compare_IPv6VersusIPv4_1()
        {
            Assert.AreEqual(1, new IPAddressComparer().Compare(IPAddress.Parse("fe80::1"), IPAddress.Parse("127.0.0.1")));
        }

        [Test]
        public void Compare_IPv4VersusIPv6_Minus1()
        {
            Assert.AreEqual(-1, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("fe80::1")));
        }
    }
}
