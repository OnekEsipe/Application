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
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        public List<Criteria> Criterias { get; set; } = new List<Criteria>();
        public List<Jury> Jurys { get; set; } = new List<Jury>();
        public List<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
