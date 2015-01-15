using System.Net;
using Xunit;
using Xunit.Extensions;

namespace IpRanges.Tests
{
    public class IPAddressHelperTests
    {
        [Theory]
        [InlineData("192.168.1.1", "255.255.255.0", "192.168.1.255")]
        [InlineData("192.168.1.1", "255.255.0.0", "192.168.255.255")]
        [InlineData("192.168.1.1", "255.0.0.0", "192.255.255.255")]
        [InlineData("192.168.1.1", "255.255.255.192", "192.168.1.63")]
        [InlineData("192.168.1.65", "255.255.255.192", "192.168.1.127")]
        public void GetBroadcastAddress(string address, string subnetMask, string expected)
        {
            Assert.Equal(IPAddress.Parse(expected), IPAddressHelper.GetBroadcastAddress(IPAddress.Parse(address), IPAddress.Parse(subnetMask)));
        }

        [Theory]
        [InlineData("192.168.1.1", "255.255.255.0", "192.168.1.0")]
        [InlineData("192.168.1.1", "255.255.0.0", "192.168.0.0")]
        [InlineData("192.168.1.1", "255.0.0.0", "192.0.0.0")]
        [InlineData("192.168.1.1", "255.255.255.192", "192.168.1.0")]
        [InlineData("192.168.1.65", "255.255.255.192", "192.168.1.64")]
        public void GetNetworkAddress(string address, string subnetMask, string expected)
        {
            Assert.Equal(IPAddress.Parse(expected), IPAddressHelper.GetNetworkAddress(IPAddress.Parse(address), IPAddress.Parse(subnetMask)));
        }

        [Theory]
        [InlineData("192.168.1.1", "192.168.1.2", "255.255.255.0")]
        public void AreInSameSubnet_True(string address1, string address2, string subnetMask)
        {
            Assert.True(IPAddressHelper.AreInSameSubnet(IPAddress.Parse(address1), IPAddress.Parse(address2), IPAddress.Parse(subnetMask)));
        }

        [Theory]
        [InlineData("192.168.1.1", "192.168.2.1", "255.255.255.0")]
        public void AreInSameSubnet_False(string address1, string address2, string subnetMask)
        {
            Assert.False(IPAddressHelper.AreInSameSubnet(IPAddress.Parse(address1), IPAddress.Parse(address2), IPAddress.Parse(subnetMask)));
        }

        [Theory]
        [InlineData(0, "0.0.0.0")]
        [InlineData(1, "128.0.0.0")]
        [InlineData(24, "255.255.255.0")]
        [InlineData(25, "255.255.255.128")]
        [InlineData(32, "255.255.255.255")]
        public void CreateSubnetMaskIPv4(int cidr, string subnetMask)
        {
            Assert.Equal(subnetMask, IPAddressHelper.CreateSubnetMaskIPv4((byte)cidr).ToString());
        }

        [Theory]
        [InlineData(0, "::")]
        [InlineData(1, "8000::")]
        [InlineData(24, "ffff:ff00::")]
        [InlineData(25, "ffff:ff80::")]
        [InlineData(32, "ffff:ffff::")]
        [InlineData(64, "ffff:ffff:ffff:ffff::")]
        [InlineData(128, "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")]
        public void CreateSubnetMaskIPv6(int cidr, string subnetMask)
        {
            Assert.Equal(subnetMask, IPAddressHelper.CreateSubnetMaskIPv6((byte)cidr).ToString());
        }
    }
}
