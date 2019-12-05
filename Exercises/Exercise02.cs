using AdventOfCode.Dependencies;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise02 : IExercise
    {
        public string Inputs => "1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,6,1,19,1,19,5,23,2,10,23,27,2,"
            + "27,13,31,1,10,31,35,1,35,9,39,2,39,13,43,1,43,5,47,1,47,6,51,2,6,51,55,1,5,55,59,2,9,"
            + "59,63,2,6,63,67,1,13,67,71,1,9,71,75,2,13,75,79,1,79,10,83,2,83,9,87,1,5,87,91,2,91,"
            + "6,95,2,13,95,99,1,99,5,103,1,103,2,107,1,107,10,0,99,2,0,14,0";

        List<int> Codes => Inputs.Split(',').Select(x => int.Parse(x)).ToList();

        public int ProblemOne() => RunIntCode(12, 2);

        public int ProblemTwo()
        {
            const int expected = 19690720;
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

            return result;
        }

        int RunIntCode(int noun, int verb)
        {
            const int perStep = 4;
            var codes = Codes.ToList();

            codes[1] = noun;
            codes[2] = verb;

            for (int i = 0; i < codes.Count / perStep; i++)
            {
                var offset = i * perStep;
                var opCode = codes[offset + 0];
                if (opCode == 99) break;

                int result;
                var op1 = codes[codes[offset + 1]];
                var op2 = codes[codes[offset + 2]];

                switch (opCode)
                {
                    case 1: result = op1 + op2; break;
                    case 2: result = op1 * op2; break;
                    default: result = 0; break;
                }

                codes[codes[offset + 3]] = result;
            }

            return codes[0];
        }
    }
}
