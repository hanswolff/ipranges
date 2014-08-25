using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;

namespace IpRanges
{
    public static class IPRangesParser
    {
        public static IEnumerable<IPRangesGroup> ParseFromResources(string resourcePrefix = null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (IsDynamicAssembly(assembly))
                    continue;

                string[] names;
                try { names = assembly.GetManifestResourceNames(); }
                catch { continue; }

                foreach (var resName in names)
                {
                    if (!String.IsNullOrEmpty(resourcePrefix))
                    {
                        if (!resName.StartsWith(resourcePrefix)) continue;
                    }

                    if (!resName.EndsWith(".xml")) continue;

                    IPRangesGroup group;
                    using (var reader = new XmlTextReader(assembly.GetManifestResourceStream(resName)))
                    {
                        Exception exception;
                        if (!TryParseFromXml(reader, out group, out exception, true))
                        {
                            throw exception;
                        }
                    }

                    if (group.Regions.Any())
                        yield return group;
                }
            }
        }

        private static bool IsDynamicAssembly(Assembly assembly)
        {
            return (assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder);
        }

        public static IPRangesGroup ParseFromXml(string xml)
        {
            if (xml == null) throw new ArgumentNullException("xml");

            using (var mem = new MemoryStream(Encoding.Default.GetBytes(xml)))
            using (var reader = new StreamReader(mem))
                return ParseFromXml(reader.BaseStream);
        }

        public static IPRangesGroup ParseFromXml(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            using (var reader = new XmlTextReader(stream))
                return ParseFromXml(reader);
        }

        public static IPRangesGroup ParseFromXml(XmlTextReader reader)
        {
            Exception exception;
            IPRangesGroup group;
            if (!TryParseFromXml(reader, out group, out exception))
                throw exception;

            return group;
        }

        public static bool TryParseFromXml(XmlTextReader reader, out IPRangesGroup group, out Exception exception)
        {
            return TryParseFromXml(reader, out group, out exception, false);
        }

        public static bool TryParseFromXml(XmlTextReader reader, out IPRangesGroup group, out Exception exception, bool skipIfRootElementNotMatching)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            exception = null;
            group = null;

            IPRangesRegion region = null;
            IPRangesGroup result = null;

            var level = 0;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        level++;
                        try
                        {
                            var name = reader.Name.ToLowerInvariant();
                            if (level == 1)
                            {
                                if (name == "group")
                                {
                                    result = ReadGroupElement(reader);
                                    continue;
                                }

                                if (skipIfRootElementNotMatching)
                                {
                                    group = new IPRangesGroup();
                                    return true;
                                }
                                exception = new XmlException("Invalid root element('" + reader.Name + "'), expecting 'group'");
                                return false;
                            }

                            if (level == 2 && name == "region")
                            {
                                if (result == null)
                                {
                                    exception = new InvalidDataException("Missing appropriate root element");
                                    return false;
                                }
                                region = ReadRegionElement(reader);
                                region.ParentGroup = result;
                                result.Regions.Add(region);
                                continue;
                            }

                            if (level == 3 && (name == "range" || name == "iprange"))
                            {
                                if (result == null)
                                {
                                    exception = new InvalidDataException("Missing 'group' element");
                                    return false;
                                }
                                if (region == null)
                                {
                                    exception = new InvalidDataException("Missing 'region' element");
                                    return false;
                                }

                                region.Ranges.Add(ReadRangeElement(reader));
                            }
                        }
                        finally
                        {
                            if (reader.IsEmptyElement) level--;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        level--;
                        if (level == 0)
                        {
                            group = result;
                            return true;
                        }
                        break;
                }
            }

            group = result;
            return true;
        }

        private static IPRangesGroup ReadGroupElement(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            var group = new IPRangesGroup();
            while (reader.MoveToNextAttribute())
            {
                var attrName = reader.Name.ToLowerInvariant();
                if (attrName == "name") @group.Name = reader.Value;
            }
            reader.MoveToContent();
            return group;
        }

        private static IPRangesRegion ReadRegionElement(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            var region = new IPRangesRegion();
            while (reader.MoveToNextAttribute())
            {
                var attrName = reader.Name.ToLowerInvariant();
                switch (attrName)
                {
                    case "name": region.Name = reader.Value.Trim(); break;
                    case "description": region.Description = reader.Value; break;
                }
            }
            reader.MoveToContent();
            return region;
        }

        private static IPRange ReadRangeElement(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            string network = null;
            string from = null;
            string to = null;
            while (reader.MoveToNextAttribute())
            {
                var attrName = reader.Name.ToLowerInvariant();
                switch (attrName)
                {
                    case "network":
                    case "subnet":
                        network = reader.Value.Trim(); break;

                    case "from": from = reader.Value.Trim(); break;
                    case "to": to = reader.Value.Trim(); break;
                }
            }

            IPRange range = null;
            IPAddress fromIp = null;
            IPAddress toIp = null;

            if (!String.IsNullOrEmpty(network)) range = IPRange.Parse(network);
            if (!String.IsNullOrEmpty(from))
            {
                if (!IPAddress.TryParse(from, out fromIp))
                    throw new FormatException(String.Format("An invalid from IP address was specified ('{0}').", from));
                if (range != null && !fromIp.Equals(range.From))
                    throw new InvalidDataException(String.Format("From IP in range does not match calculated value, data seems to be inconsistent ({0} != {1})", fromIp, range.From));
            }
            if (!String.IsNullOrEmpty(to))
            {
                if (!IPAddress.TryParse(to, out toIp))
                    throw new FormatException(String.Format("An invalid to IP address was specified ('{0}').", to));
                if (range != null && !toIp.Equals(range.To))
                    throw new InvalidDataException(String.Format("To IP in range does not match calculated value, data seems to be inconsistent ({0} != {1})", toIp, range.To));
            }

            if (range == null)
            {
                if (fromIp == null) throw new InvalidDataException("Missing 'from' or 'network' attribute for range");
                if (toIp == null) throw new InvalidDataException("Missing 'to' or 'network' attribute for range");
                range = new IPRange(fromIp, toIp);
            }
            reader.MoveToContent();
            return range;
        }
    }
}
