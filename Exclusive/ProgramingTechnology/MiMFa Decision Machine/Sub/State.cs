using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.DecisionMachine
{
    public class State
    {
        public virtual string Name { get; set; }
        public virtual Double Priority { get; set; }
       
        public State(string name, Double priority = 0)
        {
            Name = name;
            Priority = priority;
        }
    }
}
