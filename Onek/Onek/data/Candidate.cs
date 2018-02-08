using System;
using System.Collections.Generic;
using System.Text;

namespace Onek
{
    public class Candidate
    {
        public int Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FullName { get => FirstName + " " + LastName; } 

    }
}