using AdventOfCode.Dependencies;
using AdventOfCode.Dependencies.IntCode;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise09 : Exercise
    {
        public override string FileName => "Exercise09";

        List<long> Instructions => Inputs.Split(',').Select(x => long.Parse(x)).ToList();

        public override string ProblemOne()
        {
            var test = new IntCode();
            test.SetInputs(1);
            test.Execute(Instructions);

            var result = test.Outputs.Last();
            return result.ToString();
        }

        public override string ProblemTwo()
        {
            var test = new IntCode();
            test.SetInputs(2);
            test.Execute(Instructions);

            var result = test.Outputs.Last();
            return result.ToString();
        }
    }
}
