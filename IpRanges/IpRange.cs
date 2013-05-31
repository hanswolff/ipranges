using System;
using System.Diagnostics;
using System.Net;

namespace Dedimax.IpRanges
{
    [DebuggerDisplay("{From} - {To}")]
    public class IpRange
    {
        public IPAddress From { get; set; }
        public IPAddress To { get; set; }

        public IpRange(IPAddress from, IPAddress to)
        {
            From = from;
            To = to;
        }

        private static bool TryParseNetwork(string network, out IpRange range, out Exception exception)
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
                range = new IpRange(singleAddress, singleAddress);
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

            var subnetMask = IpHelper.CreateSubnetMaskIPv4(cidr);
            var fromIp = IpHelper.GetNetworkAddress(networkIp, subnetMask);
            var toIp = IpHelper.GetBroadcastAddress(networkIp, subnetMask);
            range = new IpRange(fromIp, toIp);

            return true;
        }

        public static IpRange Parse(string network)
        {
            IpRange range;
            Exception exception;
            if (!TryParseNetwork(network, out range, out exception))
                throw exception;

            return range;
        }
    }
}
