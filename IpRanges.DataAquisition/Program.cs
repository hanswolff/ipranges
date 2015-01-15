using IpRanges.DataAquisition.Amazon;
using System;
using System.IO;
using System.Linq;

namespace IpRanges.DataAquisition
{
    class Program
    {
        static void Main()
        {
            var sources = new IGetRangesGroup[]
            {
                new AmazonIpRangeSource()
            };

            foreach (var source in sources)
            {
                var group = source.GetRangesGroup();
                var fileName = group.Name + ".xml";

                using (var file = File.CreateText(fileName))
                {
                    Console.WriteLine("Writing file {0}", fileName);

                    file.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");

                    WriteGroupsToFile(file, group);
                }

                Console.WriteLine();
                Console.WriteLine("Finished, press ENTER to exit.");

                Console.ReadLine();
            }
        }

        private static void WriteGroupsToFile(StreamWriter file, IPRangesGroup @group)
        {
            file.WriteLine("<group name=\"{0}\">", @group.Name);

            foreach (var region in group.Regions)
            {
                if (String.IsNullOrEmpty(region.Id))
                    throw new InvalidOperationException(String.Format("No mapping for region {0}", region.Name));

                var nameFragment = region.Name != null ? " name=\"" + region.Name + "\"" : "";
                file.WriteLine("  <region id=\"{0}\"{1} description=\"{2}\">", region.Id, nameFragment, region.Description);

                foreach (var range in region.Ranges.OrderBy(x => x.From, new IPAddressComparer()))
                {
                    file.WriteLine("    <range network=\"{0}\" from=\"{1}\" to=\"{2}\" />", range.GetNetwork(), range.From, range.To);
                }

                file.WriteLine("  </region>");
            }

            file.WriteLine("</group>");
        }
    }
}
