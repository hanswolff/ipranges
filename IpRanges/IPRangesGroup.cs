using System.Collections.Generic;
using System.Diagnostics;

namespace IpRanges
{
    [DebuggerDisplay("Name: '{Name}'")]
    public class IPRangesGroup
    {
        public string Name { get; set; }

        public List<IPRangesRegion> Regions { get; private set; }

        public IPRangesGroup()
        {
            Regions = new List<IPRangesRegion>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
