using System;
using System.Collections.Generic;
using System.Text;

namespace Onek
{
    class Jury
    {
        public int Id { get; set; }
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public List<Candidate> Candidates { get; set; } = new List<Candidate>();
    }
}