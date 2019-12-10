using AdventOfCode.Dependencies;
using AdventOfCode.Dependencies.IntCode;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise02 : Exercise
    {
        public override string FileName => "Exercise02";

        List<int> Instructions => Inputs.Split(',').Select(x => int.Parse(x)).ToList();

        public Exercise02()
            : base()
        {
        }

        public override string ProblemOne() => RunIntCode(12, 2).ToString();

        public override string ProblemTwo()
        {
            const long expected = 19690720;
            var result = 0;

            for (int i = 0; i < 100 * 100; i++)
            {
                var noun = i % 100;
                var verb = i / 100;
                var output = RunIntCode(noun, verb);

                if (output == expected)
                {
                    result = 100 * noun + verb;
                    break;
                }
            }

            return result.ToString();
        }

        long RunIntCode(int noun, int verb)
        {
            var instructions = Instructions.ToList();

            instructions[1] = noun;
            instructions[2] = verb;

            var intCode = new IntCode();
            return intCode.Execute(instructions);
        }
    }
}
