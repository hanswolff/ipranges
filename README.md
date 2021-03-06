### Overview

This package provides a lightweight offline classification of IP addresses. 
It determines if an IP address is within a specific range. Some common IP 
ranges are already included:

- Amazon
- Azure
- CloudFlare
- DigitalOcean
- LeaseWeb
- OVH

THERE IS NO GUARANTEE THAT THE PROVIDED IP RANGES ARE ACCURATE!
PLEASE READ THE "LICENSE" FILE!

### Get it on NuGet!

    Install-Package IpRanges
	
### Usage Example

```c#
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
```