using System;
using System.Collections.Generic;
using System.Text;

namespace Onek.data
{
    public class Criteria
    {
        public int Id { get; set; }
        public String Text { get; set; }
        public String Category { get; set; }
        public String Comment { get; set; }
        public List<Descriptor> Descriptor { get; set; } = new List<data.Descriptor>();
        public int SelectedDescriptorIndex { get; set; } = -1;

        public int GetDescriptorIndex(Descriptor descriptor)
        {
            int index = 0;
            foreach(Descriptor d in Descriptor)
            {
                if (d.Equals(descriptor))
                {
                    return index;
                }
                index++;
            }
            return index;
        }
    }
}