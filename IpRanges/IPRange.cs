using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace IpRanges
{
    [DebuggerDisplay("{From} - {To}")]
    [Serializable]
    public class IPRange : IEquatable<IPRange>
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

        public string GetNetwork()
        {
            var lastBitsFrom = CountLastBits(From.GetAddressBytes(), false);
            var lastBitsTo = CountLastBits(To.GetAddressBytes(), true);

            var bitsInRange = Math.Min(lastBitsFrom, lastBitsTo);
            var bitsPerIp = From.GetAddressBytes().Length * 8;

            var network = From + "/" + (bitsPerIp - bitsInRange);

            var checkRegion = IPRange.Parse(network);
            if (!checkRegion.From.Equals(From) || !checkRegion.To.Equals(To))
                throw new InvalidOperationException(String.Format("Could not determine network for IP range {0} to {1}", From, To));

            return network;
        }

        private static int CountLastBits(byte[] array, bool bitsSet)
        {
            var result = 0;
            for (var i = array.Length; i > 0; i--)
            {
                var b = array[i - 1];

                for (var bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    var hasBit = (b & (1 << bitIndex)) > 0;
                    if (bitsSet != hasBit)
                    {
                        return result;
                    }
                    result++;
                }
            }
            return result;
        }

        public static bool TryParseNetwork(string network, out IPRange range, out Exception exception)
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

            var subnetMask = networkIp.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddressHelper.CreateSubnetMaskIPv6(cidr) : IPAddressHelper.CreateSubnetMaskIPv4(cidr);
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

        private static readonly IPAddressComparer IpAddressComparer = new IPAddressComparer();
        public bool Equals(IPRange other)
        {
            if (other == null) return false;
            return (IpAddressComparer.Compare(From, other.From) == 0) &&
                   (IpAddressComparer.Compare(To, other.To)) == 0;
        }

        public override bool Equals(object obj)
        {
            IPRange range = obj as IPRange;
            if (range == null) return false;

            return Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 
                    ((From != null ? From.GetHashCode() : 0) * 397) ^ 
                    (To != null ? To.GetHashCode() : 0);
            }
        }
    }
}
