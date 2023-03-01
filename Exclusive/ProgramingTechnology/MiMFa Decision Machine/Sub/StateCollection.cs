using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.DecisionMachine
{
    public class StateCollection<T> : List<T> where T : State
    {
        public int PositiveCount
        {
            get
            {
                int c = 0;
                foreach (var item in this)
                    if ( item.Priority>0) c++;
                return c;
            }
        }
        public int MiddleCount
        {
            get
            {
                int c = 0;
                foreach (var item in this)
                    if (item.Priority == 0) c++;
                return c;
            }
        }
        public int NegativeCount
        {
            get
            {
                int c = 0;
                foreach (var item in this)
                    if (item.Priority < 0) c++;
                return c;
            }
        }
        public Double SumPriority
        {
            get
            {
                Double s = 0;
                foreach (var item in this)
                    s += item.Priority;
                return s;
            }
        }
        public Double SumPositive
        {
            get
            {
                Double s = 0;
                foreach (var item in this)
                   if(item.Priority> 0) s += item.Priority;
                return s;
            }
        }
        public Double SumNegative
        {
            get
            {
                Double s = 0;
                foreach (var item in this)
                    if (item.Priority < 0) s += item.Priority;
                return s;
            }
        }

        public new void Sort()
        {
            this.Sort(delegate (T x, T y)
             {
                 Double sx = x.Priority;
                 Double sy = y.Priority;
                 return sx.CompareTo(sy);
             });
            this.Reverse();
        }
    }
}
