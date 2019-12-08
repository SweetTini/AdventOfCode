using AdventOfCode.Dependencies;
using AdventOfCode.Dependencies.IntCode;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise05 : Exercise
    {
        public override string FileName => "Exercise05";

        List<int> Instructions => Inputs.Split(',').Select(x => int.Parse(x)).ToList();

        public Exercise05()
            : base()
        {
        }

        public override int ProblemOne()
        {
            var intCode = new IntCode();
            intCode.SetInputs(1);
            intCode.Execute(Instructions);
            return intCode.Output;
        }

        public override int ProblemTwo()
        {
            var intCode = new IntCode();
            intCode.SetInputs(5);
            intCode.Execute(Instructions);
            return intCode.Output;
        }
    }
}
