using System;
using System.Collections;
using System.Net;
using System.Numerics;

namespace IpRanges
{
    public static class IPAddressHelper
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
            const int maskLength = 32;
            if (cidr < 0) throw new ArgumentOutOfRangeException("cidr", cidr, "CIDR network prefix cannot be smaller than 0");
            if (cidr > maskLength) throw new ArgumentOutOfRangeException("cidr", cidr, "CIDR network prefix cannot be larger than 32 for IPv4");

            var zeroBits = maskLength - cidr;
            var result = uint.MaxValue;
            result &= (uint) ((((ulong) 0x1 << cidr) - 1) << zeroBits);
            result = (uint)IPAddress.HostToNetworkOrder((int)result);
            return new IPAddress(BitConverter.GetBytes(result));
        }

        public static IPAddress CreateSubnetMaskIPv6(int cidr)
        {
            const int maskLength = 128;
            if (cidr < 0) throw new ArgumentOutOfRangeException("cidr", cidr, "CIDR network prefix cannot be smaller than 0");
            if (cidr > maskLength) throw new ArgumentOutOfRangeException("cidr", cidr, "CIDR network prefix cannot be larger than 128 for IPv6");

            var maskBits = new BitArray(maskLength);
            for (int i = 0; i < maskLength; i++)
            {
                var index = (((maskLength - 1) - i) / 8) * 8 + (i % 8);
                maskBits.Set(index, i >= (maskLength - cidr));
            }

            var bMaskData = new byte[maskLength / 8];
            maskBits.CopyTo(bMaskData, 0);
            return new IPAddress(bMaskData);
        }

        public static BigInteger BigIntegerFromIpAddress(IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            var addressBytes = ipAddress.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(addressBytes);

            var paddedAddressBytes = new byte[addressBytes.Length + 1];
            Array.Copy(addressBytes, paddedAddressBytes, addressBytes.Length);
            return new BigInteger(paddedAddressBytes);
        }
    }
}
