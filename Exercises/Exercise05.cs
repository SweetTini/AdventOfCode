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

        public override string ProblemOne()
        {
            var intCode = new IntCode();
            intCode.SetInputs(1);
            intCode.Execute(Instructions);

            var result = intCode.Outputs.Last();
            return result.ToString();
        }

        public override string ProblemTwo()
        {
            var intCode = new IntCode();
            intCode.SetInputs(5);
            intCode.Execute(Instructions);

            var result = intCode.Outputs.Last();
            return result.ToString();
        }
    }
}
