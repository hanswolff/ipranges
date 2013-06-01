using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace IpRanges
{
    [DebuggerDisplay("{From} - {To}")]
    [Serializable]
    public class IPRange
    {
        public IPAddress From { get; set; }
        public IPAddress To { get; set; }

        public ulong Count
        {
            get
            {
                var fromBytes = From.GetAddressBytes();
                var toBytes = To.GetAddressBytes();

                if (From.AddressFamily != AddressFamily.InterNetwork)
                    throw new InvalidOperationException("Count only works for IPv4 addresses, use BigCount property for IPv6");

#pragma warning disable 612,618
                var fromNumber = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(fromBytes, 0));
                var toNumber = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(toBytes, 0));
#pragma warning restore 612,618
                return toNumber - fromNumber + 1;
            }
        }

        public BigInteger BigCount
        {
            get
            {
                var fromNumber = IPAddressHelper.BigIntegerFromIpAddress(From);
                var toNumber = IPAddressHelper.BigIntegerFromIpAddress(To);
                var count = toNumber - fromNumber + 1;
                if (count < 0) count = -count;
                return count;
            }
        }

        public IPRange(IPAddress from, IPAddress to)
        {
            if (IPAddressComparer.Static.Compare(from, to) <= 0)
            {
                From = from;
                To = to;
            }
            else
            {
                From = to;
                To = from;
            }
        }

        private static bool TryParseNetwork(string network, out IPRange range, out Exception exception)
        {
            exception = null;
            range = null;

            if (network == null)
            {
                exception = new ArgumentNullException("network");
                return false;
            }
            network = network.Trim();

            IPAddress singleAddress;
            if (IPAddress.TryParse(network, out singleAddress))
            {
                range = new IPRange(singleAddress, singleAddress);
                return true;
            }

            var pos = network.IndexOf('/');
            if (pos < 0)
            {
                exception = new ArgumentException("Expected CIDR notation is missing network (correct example would be \"129.168.1.0/24\")", "network");
                return false;
            }

            IPAddress networkIp;
            if (!IPAddress.TryParse(network.Substring(0, pos), out networkIp))
            {
                exception = new ArgumentException("Cannot parse network part of IP address", "network");
                return false;
            }

            int cidr;
            if (!Int32.TryParse(network.Substring(pos + 1), out cidr))
            {
                exception = new ArgumentException("Cannot parse CIDR part of IP address", "network");
                return false;
            }

            var subnetMask = IPAddressHelper.CreateSubnetMaskIPv4(cidr);
            var fromIp = IPAddressHelper.GetNetworkAddress(networkIp, subnetMask);
            var toIp = IPAddressHelper.GetBroadcastAddress(networkIp, subnetMask);
            range = new IPRange(fromIp, toIp);

            return true;
        }

        public static IPRange Parse(string network)
        {
            IPRange range;
            Exception exception;
            if (!TryParseNetwork(network, out range, out exception))
                throw exception;

            return range;
        }
    }
}
