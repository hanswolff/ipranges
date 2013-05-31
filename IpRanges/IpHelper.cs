using System;
using System.Net;

namespace Dedimax.IpRanges
{
    public static class IpHelper
    {
        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (subnetMask == null) throw new ArgumentNullException("subnetMask");

            byte[] addressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (addressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("IP address length does not match subnet mask length");

            var broadcastAddress = new byte[addressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(addressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (subnetMask == null) throw new ArgumentNullException("subnetMask");

            byte[] addressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (addressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("IP address length does not match subnet mask length");

            var broadcastAddress = new byte[addressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(addressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static bool AreInSameSubnet(IPAddress first, IPAddress second, IPAddress subnetMask)
        {
            IPAddress network1 = GetNetworkAddress(first, subnetMask);
            IPAddress network2 = GetNetworkAddress(second, subnetMask);

            return network1.Equals(network2);
        }

        public static IPAddress CreateSubnetMaskIPv4(int cidr)
        {
            if (cidr < 0) throw new ArgumentOutOfRangeException("cidr", cidr, "CIDR network prefix cannot be smaller than 0");
            if (cidr > 32) throw new ArgumentOutOfRangeException("cidr", cidr, "CIDR network prefix cannot be larger than 32 for IPv4");

            var zeroBits = 32 - cidr;
            var result = uint.MaxValue;
            result &= (uint) ((((ulong) 0x1 << cidr) - 1) << zeroBits);
            result = (uint)IPAddress.HostToNetworkOrder((int)result);
            return new IPAddress(BitConverter.GetBytes(result));
        }
    }
}
