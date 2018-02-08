using System;
using System.Collections.Generic;
using System.Text;

namespace Onek.data
{
    public class Evaluation
    {
        public int Id { get; set; }
        public int IdJury { get; set; }
        public int IdEvent { get; set; }
        public int IdCandidate { get; set; }
        public String Comment { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public List<Criteria> Criterias { get; set; }

        public Boolean hasEvaluation(int idCandidate)
        {
            return IdCandidate == idCandidate;
        }
    }
}