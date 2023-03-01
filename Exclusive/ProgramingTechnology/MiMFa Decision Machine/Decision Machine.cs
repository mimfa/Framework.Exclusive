using MiMFa.General;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.DecisionMachine
{
    public class MiMFa_DecisionMachine<T> : IProgramingTechnology
    {
        #region Property
        public string Name => "MiMFa Decision Machine";
        public string Description => "Decision, select and sort for list machine";
        public string TempDirectory
        {
            get { return _TempDirectory; }
            set { PathService.CreateAllDirectories(_TempDirectory = value); }
        }
        public Version Version => new Version(1, 0,0, 0);

        public virtual bool SendT { get; set; } = false;
        public bool Shaked { get; set; } = false;
        public bool JustAllowanceMember { get; set; } = false;
        public double SumMaxPriorityLap { get; set; } = 999999999;
        public double SumMinPriorityLap { get; set; } = -999999999;
        public virtual object[] Conditions { get; set; }
        public virtual List<T> InputList { get; set; } = new List<T>();
        public virtual List<InferenceCollection> InferencesList { get; set; }
        public virtual List<T> OutputList { get; set; } = new List<T>();
        public virtual ConstraintCollection Constraints { get; set; }
        #endregion

        public MiMFa_DecisionMachine(List<T> inputList, ConstraintCollection constraints)
        {
            Set(inputList, constraints);
        }
        public MiMFa_DecisionMachine() { }

        public void Set(List<T> inputList, ConstraintCollection constraints)
        {
            InputList = inputList;
            Constraints = constraints;
        }
        public List<T> Get()
        {
            return OutputList = Sort(InputList, Constraints, Conditions);
        }
        public List<T> Get(params Object[] conditions)
        {
            return OutputList = Sort(InputList, Constraints, conditions);
        }

        public List<T> Sort(List<T> inputList, ConstraintCollection constraints, params object[] conditions)
        {
            if (Shaked) inputList = CollectionService.Shake(inputList);
            List<KeyValuePair<T, InferenceCollection>> lold = new List<KeyValuePair<T, InferenceCollection>>();
            for (int i = 0; i < inputList.Count; i++)
                lold.Add(new KeyValuePair<T, InferenceCollection>(inputList[i], new InferenceCollection()));
            constraints.Sort();
            Constraint constraint = null;
            Type t = inputList.First().GetType();
            FieldInfo[] fi = null;
            PropertyInfo[] pi = null;
            if (SendT)
                for (int index = 0; index < constraints.Count; index++)
                    for (int i = 0; i < inputList.Count; i++)
                        lold[i].Value.Add(constraints[index].Solver(inputList[i], conditions));
            else
                for (int index = 0; index < constraints.Count; index++)
                {
                    constraint = constraints[index];
                    fi =  InfoService.GetFields(inputList.First(), constraint.Name, false);
                    if (fi.Length > 0)
                        for (int i = 0; i < inputList.Count; i++)
                            lold[i].Value.Add( constraint.Solver( fi.First().GetValue(inputList[i]), conditions));
                    else
                    {
                        pi = InfoService.GetProperties(inputList.First(), constraint.Name, false);
                        if (pi.Length > 0)
                            for (int i = 0; i < inputList.Count; i++)
                                lold[i].Value.Add( constraint.Solver(  pi.First().GetValue(inputList[i]), conditions));
                    }
                }
            lold = Sort(lold);
            return AllowanceMember(lold);
        }

        private List<KeyValuePair<T, InferenceCollection>> Sort(List<KeyValuePair<T, InferenceCollection>> lodc)
        {
            lodc.Sort(delegate (KeyValuePair<T, InferenceCollection> x, KeyValuePair<T, InferenceCollection> y)
            {
                Double sx, sy;
                sx = x.Value.SumPriority;
                sy = y.Value.SumPriority;
                if (sx != sy) return sx.CompareTo(sy);
                sx = x.Value.SumNegative;
                sy = y.Value.SumNegative;
                if (sx != sy) return sx.CompareTo(sy);
                sx = x.Value.SumPositive;
                sy = y.Value.SumPositive;
                if (sx != sy) return sx.CompareTo(sy);
                sx = x.Value.NegativeCount;
                sy = y.Value.NegativeCount;
                if (sx != sy) return sx.CompareTo(sy);
                sx = x.Value.PositiveCount;
                sy = y.Value.PositiveCount;
                if (sx != sy) return sx.CompareTo(sy);
                return x.Value.Count.CompareTo(y.Value.Count);
            });
            lodc.Reverse();
            return lodc;
        }
        private List<T> AllowanceMember(List<KeyValuePair<T, InferenceCollection>> lold)
        {
            InferencesList = new List<InferenceCollection>();
            List<T> outputList = new List<T>();
            for (int i = 0; i < lold.Count; i++)
            {
                if (JustAllowanceMember && lold[i].Value.SumPriority < SumMinPriorityLap)
                    continue;
                outputList.Add(lold[i].Key);
                InferencesList.Add(lold[i].Value);
            }
            return outputList;
        }

        #region Private
        private string _TempDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\Temp\DecisionMachine\";

        #endregion
    }

}
