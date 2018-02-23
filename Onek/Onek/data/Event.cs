using Onek.data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Onek
{
    public class Event
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public Boolean SignatureNeeded { get; set; }
        public Boolean IsOpened { get; set; }
        public ObservableCollection<Criteria> Criterias { get; set; } = new ObservableCollection<Criteria>();
        public ObservableCollection<Jury> Jurys { get; set; } = new ObservableCollection<Jury>();
        public ObservableCollection<Evaluation> Evaluations { get; set; } = new ObservableCollection<Evaluation>();

        public Boolean hasEvaluation(int idCandidate)
        {
            Boolean hasEval = false;
            foreach(Evaluation eval in Evaluations)
            {
                hasEval = eval.hasEvaluation(idCandidate);
            }
            return hasEval;
        }

        public Evaluation GetEvaluationForCandidate(int idCandidate)
        {
            foreach(Evaluation e in Evaluations)
            {

                if (e.IdCandidate == idCandidate)
                    return e;
            }
            return null;
        }
    }
}