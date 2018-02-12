﻿using Onek.data;
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
        public Boolean IsSigned { get; set; }
        public Boolean IsOpened { get; set; }
        public ObservableCollection<Criteria> Criterias { get; set; } = new ObservableCollection<Criteria>();
        public List<Jury> Jurys { get; set; } = new List<Jury>();
        public List<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

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
                //if (e.hasEvaluation(idCandidate))
                //{
                //    return e;
                //}
            }
            return null;
        }
    }
}