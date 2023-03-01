using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.DecisionMachine
{
    public delegate Inference Solver(Object obj,params Object[] conditions);
    public class ConstraintCollection : StateCollection<Constraint> { }

    public class Constraint : State
    {
        public Solver Solver { get; set; }

        public Constraint(ISolver solver) : base(solver.Name, solver.Priority)
        {
            Initialize(solver.Name,solver.Priority,solver.Solver);
        }
        public Constraint(string name, int priority = 0, Solver solver = null) : base(name, priority)
        {
            Initialize(name,  priority, solver);
        }
        public void Initialize(string name, int priority = 0, Solver solver = null)
        {
            if (solver == null)
                solver = delegate (Object o, Object[] c)
                {
                    return new Inference( Name, Convert.ToDouble(o));
                };
            Solver = delegate (Object o, Object[] c)
                {
                    Inference inference = solver(o, c);
                    if(inference.Priority == 0)inference.Priority = priority;
                    else if(inference.Priority < 0)  inference.Priority *= Math.Abs(priority);
                    else   inference.Priority *= priority;
                    return inference.Review;
                };
        }
    }
}
