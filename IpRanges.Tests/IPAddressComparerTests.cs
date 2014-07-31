using System.Net;
using Xunit;

namespace IpRanges.Tests
{
    public class IPAddressComparerTests
    {
        [Fact]
        public void Compare_NullNull_0()
        {
            Assert.Equal(0, new IPAddressComparer().Compare(null, null));
        }

        [Fact]
        public void Compare_NullAny_Minus1()
        {
            Assert.Equal(-1, new IPAddressComparer().Compare(null, IPAddress.Any));
        }

        [Fact]
        public void Compare_NullAny_1()
        {
            Assert.Equal(1, new IPAddressComparer().Compare(IPAddress.Any, null));
        }

        [Fact]
        public void Compare_IPv4SameAsIPv4_0()
        {
            Assert.Equal(0, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("127.0.0.1")));
        }

        [Fact]
        public void Compare_IPv4LargerThanIPv4_1()
        {
            Assert.Equal(1, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.2"), IPAddress.Parse("127.0.0.1")));
        }

        [Fact]
        public void Compare_IPv4LargerThanIPv4LeastVsMostSignificant_1()
        {
            Assert.Equal(1, new IPAddressComparer().Compare(IPAddress.Parse("128.0.0.1"), IPAddress.Parse("127.0.0.2")));
        }

        [Fact]
        public void Compare_IPv4SmallerThanIPv4_Minus1()
        {
            Assert.Equal(-1, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("127.0.0.2")));
        }

        [Fact]
        public void Compare_IPv4SmallerThanIPv4LeastVsMostSignificant_Minus1()
        {
            Assert.Equal(-1, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.2"), IPAddress.Parse("128.0.0.1")));
        }

        [Fact]
        public void Compare_IPv6SameAsIPv6_0()
        {
            Assert.Equal(0, new IPAddressComparer().Compare(IPAddress.Parse("fe80::1"), IPAddress.Parse("fe80::1")));
        }

        [Fact]
        public void Compare_IPv6LargerThanIPv6_1()
        {
            Assert.Equal(1, new IPAddressComparer().Compare(IPAddress.Parse("fe80::2"), IPAddress.Parse("fe80::1")));
        }

        [Fact]
        public void Compare_IPv6LargerThanIPv6LeastVsMostSignificant_1()
        {
            Assert.Equal(1, new IPAddressComparer().Compare(IPAddress.Parse("fe81::1"), IPAddress.Parse("fe80::2")));
        }

        [Fact]
        public void Compare_IPv6SmallerThanIPv6_Minus1()
        {
            Assert.Equal(-1, new IPAddressComparer().Compare(IPAddress.Parse("fe80::1"), IPAddress.Parse("fe80::2")));
        }

        [Fact]
        public void Compare_IPv6SmallerThanIPv6LeastVsMostSignificant_Minus1()
        {
            Assert.Equal(-1, new IPAddressComparer().Compare(IPAddress.Parse("fe80::2"), IPAddress.Parse("fe81::1")));
        }

        [Fact]
        public void Compare_IPv6VersusIPv4_1()
        {
            Assert.Equal(1, new IPAddressComparer().Compare(IPAddress.Parse("fe80::1"), IPAddress.Parse("127.0.0.1")));
        }

        [Fact]
        public void Compare_IPv4VersusIPv6_Minus1()
        {
            Assert.Equal(-1, new IPAddressComparer().Compare(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("fe80::1")));
        }
    }
}
