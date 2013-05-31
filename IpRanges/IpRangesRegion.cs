using System.Collections.Generic;
using System.Diagnostics;

namespace Dedimax.IpRanges
{
    [DebuggerDisplay("Name: '{Name}', Description: '{Description}'")]
    public class IpRangesRegion
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<IpRange> Ranges { get; private set; }

        public IpRangesRegion()
        {
            Ranges = new List<IpRange>();
        }
    }
}
