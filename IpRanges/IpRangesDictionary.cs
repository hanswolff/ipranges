using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace Dedimax.IpRanges
{
    public class IpRangesDictionary<T>
    {
        private readonly SortedList<ulong, SortedList<ulong, T>> _dictIpv4;
        private readonly SortedList<BigInteger, SortedList<BigInteger, T>> _dictIpv6;

        private int _count = -1;
        public int Count
        {
            get
            {
                var count = _count;
                if (count >= 0) return count;
                count = _dictIpv4.Sum(fromRange => fromRange.Value.Count);
                count += _dictIpv6.Sum(fromRange => fromRange.Value.Count);
                _count = count;
                return count;
            }
        }

        private List<ulong> _ipv4Keys;
        protected List<ulong> Ipv4Keys
        {
            get
            {
                var keys = _ipv4Keys;
                if (keys != null) return keys;
                keys = _dictIpv4.Keys.ToList();
                _ipv4Keys = keys;
                return keys;
            }
        }

        private List<BigInteger> _ipv6Keys;
        protected List<BigInteger> Ipv6Keys
        {
            get
            {
                var keys = _ipv6Keys;
                if (keys != null) return keys;
                keys = _dictIpv6.Keys.ToList();
                _ipv6Keys = keys;
                return keys;
            }
        }

        public IpRangesDictionary()
        {
            _dictIpv4 = new SortedList<ulong, SortedList<ulong, T>>();
            _dictIpv6 = new SortedList<BigInteger, SortedList<BigInteger, T>>();
        }

        public IpRangesDictionary(int capacityIpv4, int capacityIpv6)
        {
            _dictIpv4 = new SortedList<ulong, SortedList<ulong, T>>(capacityIpv4);
            _dictIpv6 = new SortedList<BigInteger, SortedList<BigInteger, T>>(capacityIpv6);
        }

        public IpRangesDictionary(int capacity)
            : this(capacity, capacity)
        {
        }

        public void Add(IpRange range, T value)
        {
            if (range == null) throw new ArgumentNullException("range");

            Add(range.From, range.To, value);
        }

        public void Add(IPAddress fromIp, IPAddress toIp, T value)
        {
            if (fromIp == null) throw new ArgumentNullException("fromIp");
            if (toIp == null) throw new ArgumentNullException("toIp");

            if (!fromIp.AddressFamily.HasFlag(AddressFamily.InterNetworkV6) && !toIp.AddressFamily.HasFlag(AddressFamily.InterNetworkV6))
            {
                AddIpv4(fromIp, toIp, value);
                return;
            }

            if (fromIp.AddressFamily.HasFlag(AddressFamily.InterNetworkV6) && toIp.AddressFamily.HasFlag(AddressFamily.InterNetworkV6))
            {
                AddIpv6(fromIp, toIp, value);
                return;
            }

            throw new ArgumentException("Cannot mix IPv4 and IPv6 range values");
        }

        private void AddIpv4(IPAddress fromIp, IPAddress toIp, T value)
        {
            if (fromIp == null) throw new ArgumentNullException("fromIp");
            if (toIp == null) throw new ArgumentNullException("toIp");

#pragma warning disable 612,618
            var fromBytes = fromIp.GetAddressBytes();
            var fromNumber = (ulong)fromBytes[0] << 24 | (ulong)fromBytes[1] << 16 | (ulong)fromBytes[2] << 8 | fromBytes[3];
            var toBytes = toIp.GetAddressBytes();
            var toNumber = (ulong)toBytes[0] << 24 | (ulong)toBytes[1] << 16 | (ulong)toBytes[2] << 8 | toBytes[3];
#pragma warning restore 612,618

            if (fromNumber > toNumber)
            {
                var temp = fromNumber;
                fromNumber = toNumber;
                toNumber = temp;
            }

            SortedList<ulong, T> subDict;
            if (!_dictIpv4.TryGetValue(fromNumber, out subDict))
            {
                subDict = new SortedList<ulong, T>(1);
                subDict[toNumber] = value;
                _dictIpv4[fromNumber] = subDict;
            }
            else subDict[toNumber] = value;
            _count = -1;
            _ipv4Keys = null;
        }

        private void AddIpv6(IPAddress fromIp, IPAddress toIp, T value)
        {
            if (fromIp == null) throw new ArgumentNullException("fromIp");
            if (toIp == null) throw new ArgumentNullException("toIp");

            var fromNumber = BigIntegerFromIpAddress(fromIp);
            var toNumber = BigIntegerFromIpAddress(toIp);

            if (fromNumber > toNumber)
            {
                var temp = fromNumber;
                fromNumber = toNumber;
                toNumber = temp;
            }

            SortedList<BigInteger, T> subDict;
            if (!_dictIpv6.TryGetValue(fromNumber, out subDict))
            {
                subDict = new SortedList<BigInteger, T>(1);
                subDict[toNumber] = value;
                _dictIpv6[fromNumber] = subDict;
            }
            else subDict[toNumber] = value;
            _count = -1;
            _ipv6Keys = null;
        }

        private BigInteger BigIntegerFromIpAddress(IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            var addressBytes = ipAddress.GetAddressBytes();
            Array.Reverse(addressBytes);

            var paddedAddressBytes = new byte[addressBytes.Length + 1];
            Array.Copy(addressBytes, paddedAddressBytes, addressBytes.Length);
            return new BigInteger(paddedAddressBytes);
        }

        public bool Contains(IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            T value;
            return TryGetValue(ipAddress, out value);
        }

        public bool TryGetValue(IPAddress ipAddress, out T value)
        {
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            if (!ipAddress.AddressFamily.HasFlag(AddressFamily.InterNetworkV6))
                return TryGetValueIpv4(ipAddress, out value);

            return TryGetValueIpv6(ipAddress, out value);
        }

        private bool TryGetValueIpv4(IPAddress ipAddress, out T value)
        {
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            var bytes = ipAddress.GetAddressBytes();
            var number = (ulong)bytes[0] << 24 | (ulong)bytes[1] << 16 | (ulong)bytes[2] << 8 | bytes[3];

            var keys = Ipv4Keys;
            int index = keys.BinarySearch(number);

            if (index < 0) index = Math.Abs(index) - 2;

            if (index >= 0 && index < keys.Count)
            {
                SortedList<ulong, T> subDict;
                if (_dictIpv4.TryGetValue(keys[index], out subDict))
                {
                    foreach (var pair in subDict)
                    {
                        if (number <= pair.Key)
                        {
                            value = pair.Value;
                            return true;
                        }
                    }
                }
            }

            value = default(T);
            return false;
        }

        private bool TryGetValueIpv6(IPAddress ipAddress, out T value)
        {
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            var number = BigIntegerFromIpAddress(ipAddress);

            var keys = Ipv6Keys;
            int index = keys.BinarySearch(number);

            if (index < 0) index = Math.Abs(index) - 2;

            if (index >= 0 && index < keys.Count)
            {
                SortedList<BigInteger, T> subDict;
                if (_dictIpv6.TryGetValue(keys[index], out subDict))
                {
                    foreach (var pair in subDict)
                    {
                        if (number <= pair.Key)
                        {
                            value = pair.Value;
                            return true;
                        }
                    }
                }
            }

            value = default(T);
            return false;
        }
    }
}
