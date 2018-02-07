using System;
using System.Collections.Generic;
using System.Text;

namespace Onek.data
{
    class Criteria
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Text { get; set; }
        public String Category { get; set; }
        public List<Descriptor> Descriptor { get; set; } = new List<data.Descriptor>();
    }
}
