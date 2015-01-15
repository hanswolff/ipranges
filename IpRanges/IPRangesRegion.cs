using System.Collections.Generic;
using System.Diagnostics;

namespace IpRanges
{
    [DebuggerDisplay("Id: '{Id}', Name: '{Name}', Description: '{Description}'")]
    public class IPRangesRegion
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public IPRangesGroup ParentGroup { get; set; }

        public List<IPRange> Ranges { get; private set; }

        public IPRangesRegion()
            : this(null, null)
        {
        }

        public IPRangesRegion(string id, string description)
            : this(id, description, null)
        {

        }

        public IPRangesRegion(string id, string description, string name)
        {
            Id = id;
            Description = description;
            Name = name;

            Ranges = new List<IPRange>();
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
