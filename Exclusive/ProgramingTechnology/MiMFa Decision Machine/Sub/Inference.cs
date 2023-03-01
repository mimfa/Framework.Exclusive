using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.DecisionMachine
{
    public class InferenceCollection : StateCollection<Inference> { }

    public class Inference : State
    {
        public Inference(string name, Double periority = 0) : base(name, periority) { }

        #region Analize
        public Inference Review
        {
            get
            {
                Inference CheckDecide = new Inference(Name, Priority);
                //analizition
                return CheckDecide;
            }
        }
        public Double Result => Review.Priority;

        public bool IsRisk
            => IsBadRisk || IsGoodRisk;
        public bool IsBadRisk
            => Priority < -100;
        public bool IsGoodRisk
            => Priority > 100;
        public bool IsCrisis
            => IsBadCrisis || IsGoodCrisis;
        public bool IsBadCrisis
           => Priority < -1000;
        public bool IsGoodCrisis
            => Priority > 1000;
        #endregion
    }
}
