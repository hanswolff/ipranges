using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Xunit;
using Xunit.Extensions;

namespace IpRanges.Tests
{
    // ReSharper disable InconsistentNaming
    public class IPRangeDictionaryTest
    {
        [Fact]
        public void count_is_zero_for_new_instance()
        {
            Assert.Equal(0, new IPRangeDictionary<object>().Count);
        }

        [Fact]
        public void count_is_one_after_inserting_an_IPv4_range()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.Any, IPAddress.Any, null);
            Assert.Equal(1, dict.Count);
        }

        [Fact]
        public void count_is_one_after_inserting_an_IPv6_range()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.IPv6Any, IPAddress.IPv6Any, null);
            Assert.Equal(1, dict.Count);
        }

        [Fact]
        public void count_is_one_after_inserting_same_IPv4_range_twice()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.Any, IPAddress.Any, null);
            dict.Add(IPAddress.Any, IPAddress.Any, null);
            Assert.Equal(1, dict.Count);
        }

        [Fact]
        public void count_is_one_after_inserting_same_IPv6_range_twice()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.IPv6Any, IPAddress.IPv6Any, null);
            dict.Add(IPAddress.IPv6Any, IPAddress.IPv6Any, null);
            Assert.Equal(1, dict.Count);
        }

        [Fact]
        public void count_is_one_after_inserting_same_IPv4_range_twice_with_inverted_from_and_to_range()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.Parse("192.168.1.255"), IPAddress.Parse("192.168.1.1"), null);
            dict.Add(IPAddress.Parse("192.168.1.1"), IPAddress.Parse("192.168.1.255"), null);
            Assert.Equal(1, dict.Count);
        }

        [Fact]
        public void count_is_one_after_inserting_same_IPv6_range_twice_with_inverted_from_and_to_range()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.Parse("fe80::"), IPAddress.Parse("fe81::"), null);
            dict.Add(IPAddress.Parse("fe81::"), IPAddress.Parse("fe80::"), null);
            Assert.Equal(1, dict.Count);
        }

        [Fact]
        public void count_is_two_after_inserting_two_different_IPv4_ranges()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.Any, IPAddress.Any, null);
            dict.Add(IPAddress.Loopback, IPAddress.Loopback, null);
            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void count_is_two_after_inserting_two_different_IPv6_ranges()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.IPv6Any, IPAddress.IPv6Any, null);
            dict.Add(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, null);
            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void count_is_two_after_inserting_two_different_IPv4_ranges_with_same_from_address()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.Any, IPAddress.Any, null);
            dict.Add(IPAddress.Any, IPAddress.Loopback, null);
            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void count_is_two_after_inserting_two_different_IPv6_ranges_with_same_from_address()
        {
            var dict = new IPRangeDictionary<object>();
            dict.Add(IPAddress.IPv6Any, IPAddress.IPv6Any, null);
            dict.Add(IPAddress.IPv6Any, IPAddress.IPv6Loopback, null);
            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void add_throws_exception_when_trying_to_add_a_range_with_IPv4_and_IPv6_values()
        {
            Assert.Throws<ArgumentException>(() => new IPRangeDictionary<object>().Add(IPAddress.Any, IPAddress.IPv6Any, null));
        }

        [Fact]
        public void contains_returns_always_false_for_empty_instance()
        {
            Assert.Equal(false, new IPRangeDictionary<object>().Contains(IPAddress.Any));
        }

        [Theory]
        [InlineData("0.0.0.0")]
        [InlineData("192.168.1.1")]
        [InlineData("192.168.1.255")]
        [InlineData("192.168.1.9")]
        [InlineData("192.168.1.16")]
        [InlineData("192.168.1.19")]
        [InlineData("192.168.1.26")]
        [InlineData("192.168.1.29")]
        [InlineData("192.168.1.36")]
        [InlineData("192.168.1.39")]
        [InlineData("192.168.1.46")]
        [InlineData("192.168.1.49")]
        [InlineData("192.168.1.56")]
        public void describe_dictionary_with_many_nonoverlapping_IPv4_ranges_check_contains_ip_that_is_not_in_any_range(string ipAddress)
        {
            var dict = new IPRangeDictionary<string>();
            dict.Add(IPAddress.Parse("192.168.1.10"), IPAddress.Parse("192.168.1.15"), null);
            dict.Add(IPAddress.Parse("192.168.1.20"), IPAddress.Parse("192.168.1.25"), null);
            dict.Add(IPAddress.Parse("192.168.1.30"), IPAddress.Parse("192.168.1.35"), null);
            dict.Add(IPAddress.Parse("192.168.1.40"), IPAddress.Parse("192.168.1.45"), null);
            dict.Add(IPAddress.Parse("192.168.1.50"), IPAddress.Parse("192.168.1.55"), null);

            Assert.Equal(false, dict.Contains(IPAddress.Parse(ipAddress)));
        }

        [Theory]
        [InlineData("192.168.1.10", "A")]
        [InlineData("192.168.1.11", "A")]
        [InlineData("192.168.1.15", "A")]
        [InlineData("192.168.1.20", "B")]
        [InlineData("192.168.1.21", "B")]
        [InlineData("192.168.1.25", "B")]
        [InlineData("192.168.1.30", "C")]
        [InlineData("192.168.1.31", "C")]
        [InlineData("192.168.1.35", "C")]
        [InlineData("192.168.1.40", "D")]
        [InlineData("192.168.1.41", "D")]
        [InlineData("192.168.1.45", "D")]
        [InlineData("192.168.1.50", "E")]
        [InlineData("192.168.1.51", "E")]
        [InlineData("192.168.1.55", "E")]
        public void describe_dictionary_with_many_nonoverlapping_IPv4_ranges_try_get_value_for_ip_in_range(string ipAddress, string expected)
        {
            var dict = new IPRangeDictionary<string>();
            dict.Add(IPAddress.Parse("192.168.1.10"), IPAddress.Parse("192.168.1.15"), "A");
            dict.Add(IPAddress.Parse("192.168.1.20"), IPAddress.Parse("192.168.1.25"), "B");
            dict.Add(IPAddress.Parse("192.168.1.30"), IPAddress.Parse("192.168.1.35"), "C");
            dict.Add(IPAddress.Parse("192.168.1.40"), IPAddress.Parse("192.168.1.45"), "D");
            dict.Add(IPAddress.Parse("192.168.1.50"), IPAddress.Parse("192.168.1.55"), "E");

            string value;
            Assert.Equal(true, dict.TryGetValue(IPAddress.Parse(ipAddress), out value));
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("::")]
        [InlineData("fe80:0:0:0:0::1")]
        [InlineData("fe80:0:0:0:ffff::1")]
        [InlineData("fe80:0:0:0:0009::1")]
        [InlineData("fe80:0:0:0:0016::1")]
        [InlineData("fe80:0:0:0:0019::1")]
        [InlineData("fe80:0:0:0:0026::1")]
        [InlineData("fe80:0:0:0:0029::1")]
        [InlineData("fe80:0:0:0:0036::1")]
        [InlineData("fe80:0:0:0:0039::1")]
        [InlineData("fe80:0:0:0:0046::1")]
        [InlineData("fe80:0:0:0:0049::1")]
        [InlineData("fe80:0:0:0:0056::1")]
        public void describe_dictionary_with_many_nonoverlapping_IPv6_ranges_check_contains_ip_that_is_not_in_any_range(string ipAddress)
        {
            var dict = new IPRangeDictionary<string>();
            dict.Add(IPAddress.Parse("fe80:0:0:0:0010::1"), IPAddress.Parse("fe80:0:0:0:0015::1"), null);
            dict.Add(IPAddress.Parse("fe80:0:0:0:0020::1"), IPAddress.Parse("fe80:0:0:0:0025::1"), null);
            dict.Add(IPAddress.Parse("fe80:0:0:0:0030::1"), IPAddress.Parse("fe80:0:0:0:0035::1"), null);
            dict.Add(IPAddress.Parse("fe80:0:0:0:0040::1"), IPAddress.Parse("fe80:0:0:0:0045::1"), null);
            dict.Add(IPAddress.Parse("fe80:0:0:0:0050::1"), IPAddress.Parse("fe80:0:0:0:0055::1"), null);

            Assert.Equal(false, dict.Contains(IPAddress.Parse(ipAddress)));
        }

        [Theory]
        [InlineData("fe80:0:0:0:0010::1", "A")]
        [InlineData("fe80:0:0:0:0011::1", "A")]
        [InlineData("fe80:0:0:0:0015::1", "A")]
        [InlineData("fe80:0:0:0:0020::1", "B")]
        [InlineData("fe80:0:0:0:0021::1", "B")]
        [InlineData("fe80:0:0:0:0025::1", "B")]
        [InlineData("fe80:0:0:0:0030::1", "C")]
        [InlineData("fe80:0:0:0:0031::1", "C")]
        [InlineData("fe80:0:0:0:0035::1", "C")]
        [InlineData("fe80:0:0:0:0040::1", "D")]
        [InlineData("fe80:0:0:0:0041::1", "D")]
        [InlineData("fe80:0:0:0:0045::1", "D")]
        [InlineData("fe80:0:0:0:0050::1", "E")]
        [InlineData("fe80:0:0:0:0051::1", "E")]
        [InlineData("fe80:0:0:0:0055::1", "E")]
        public void describe_dictionary_with_many_nonoverlapping_IPv6_ranges_try_get_value_for_ip_in_range(string ipAddress, string expected)
        {
            var dict = new IPRangeDictionary<string>();
            dict.Add(IPAddress.Parse("fe80:0:0:0:0010::1"), IPAddress.Parse("fe80:0:0:0:0015::1"), "A");
            dict.Add(IPAddress.Parse("fe80:0:0:0:0020::1"), IPAddress.Parse("fe80:0:0:0:0025::1"), "B");
            dict.Add(IPAddress.Parse("fe80:0:0:0:0030::1"), IPAddress.Parse("fe80:0:0:0:0035::1"), "C");
            dict.Add(IPAddress.Parse("fe80:0:0:0:0040::1"), IPAddress.Parse("fe80:0:0:0:0045::1"), "D");
            dict.Add(IPAddress.Parse("fe80:0:0:0:0050::1"), IPAddress.Parse("fe80:0:0:0:0055::1"), "E");

            string value;
            Assert.Equal(true, dict.TryGetValue(IPAddress.Parse(ipAddress), out value));
            Assert.Equal(expected, value);
        }

        [Fact]
        [RunnableInDebugOnly]
        public void PerformanceTestIpv4()
        {
            const long count = 1000000;
            var dict = new IPRangeDictionary<string>();
            for (var i = 1; i <= 100; i++)
                for (var j = 1; j <= 100; j++)
                    dict.Add(IPAddress.Parse(i + "." + j + ".0.1"), IPAddress.Parse(i + "." + j + ".0.128"), null);

            string value;
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
                dict.TryGetValue(IPAddress.Any, out value);
            stopwatch.Stop();
            Console.WriteLine("Elapsed in " + stopwatch.ElapsedMilliseconds + "ms, " + count * 1000 / stopwatch.ElapsedMilliseconds);
        }

        [Fact]
        [RunnableInDebugOnly]
        public void PerformanceTestIpv6()
        {
            const long count = 1000000;
            var dict = new IPRangeDictionary<string>();
            for (var i = 1; i <= 100; i++)
                for (var j = 1; j <= 100; j++)
                    dict.Add(IPAddress.Parse("fe80:" + i.ToString("X4") + ":" + j.ToString("X4") + ":0:0::1"), IPAddress.Parse("fe80:" + i.ToString("X4") + ":" + j.ToString("X4") + ":0:0::1"), null);

            string value;
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
                dict.TryGetValue(IPAddress.IPv6Any, out value);
            stopwatch.Stop();
            Console.WriteLine("Elapsed in " + stopwatch.ElapsedMilliseconds + "ms, " + count * 1000 / stopwatch.ElapsedMilliseconds);
        }

        [Fact]
        [RunnableInDebugOnly]
        public void run_example()
        {
            // get regions from resource
            var regions = IPRangesParser.ParseFromResources().SelectMany(x => x.Regions);

            // create regions dictionary
            var dictionary = new IPRangeDictionary<IPRangesRegion>();
            foreach (var region in regions)
                foreach (var range in region.Ranges)
                    dictionary.Add(range, region);

            var ipAddress = IPAddress.Parse("23.20.123.123");

            // test if IP address is within any region
            var foundRegion = dictionary[ipAddress]; // throws KeyNotFoundException if not found
            Console.WriteLine("IP address '{0}' is in region '{1}'", ipAddress, foundRegion);

            // or better (to avoid KeyNotFoundException)
            IPRangesRegion value;
            if (dictionary.TryGetValue(ipAddress, out value))
                Console.WriteLine("IP address '{0}' is in region '{1}'", ipAddress, foundRegion);
        }
    }
    // ReSharper restore InconsistentNaming
}
