using Onek.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Onek
{
    class Event
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<Jury> Jurys { get; set; }
        public List<Evaluation> evaluations { get; set; }
    }
}
