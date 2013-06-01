using System.Collections.Generic;
using System.Diagnostics;

namespace IpRanges
{
    [DebuggerDisplay("Name: '{Name}', Description: '{Description}'")]
    public class IPRangesRegion
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public IPRangesGroup ParentGroup { get; set; }

        public List<IPRange> Ranges { get; private set; }

        public IPRangesRegion()
        {
            Ranges = new List<IPRange>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
