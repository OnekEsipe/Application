using System;
using System.Collections.Generic;

namespace Onek
{
    /// <summary>
    /// Data class to store Jury information
    /// </summary>
    public class Jury
    {
        //Properties
        public int Id { get; set; }
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public String Login { get; set; }
        public List<Candidate> Candidates { get; set; } = new List<Candidate>();
    }
}