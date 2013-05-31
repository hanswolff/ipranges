using System.Collections.Generic;
using System.Diagnostics;

namespace Dedimax.IpRanges
{
    [DebuggerDisplay("Name: '{Name}'")]
    public class IpRangesGroup
    {
        public string Name { get; set; }

        public List<IpRangesRegion> Regions { get; private set; }

        public IpRangesGroup()
        {
            Regions = new List<IpRangesRegion>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
