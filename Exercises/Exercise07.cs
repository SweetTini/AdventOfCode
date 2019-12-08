using AdventOfCode.Dependencies;
using AdventOfCode.Dependencies.IntCode;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise07 : Exercise
    {
        public override string FileName => "Exercise07";

        List<int> Instructions => Inputs.Split(',').Select(x => int.Parse(x)).ToList();

        public Exercise07()
            : base()
        {
        }

        public override int ProblemOne()
        {
            var signals = GetSignalSequences();
            var result = signals.Select(CheckThrusters).Max();
            return result;
        }

        public override int ProblemTwo()
        {
            return 0;
        }

        List<string> GetSignalSequences()
        {
            return Enumerable.Range(0, 44444)
                .Select(x =>
                {
                    var valueAsString = x.ToString().PadLeft(5, '0');
                    var result = string.Empty;
                    for (int i = 0; i < valueAsString.Length; i++)
                        result += (int.Parse(valueAsString[i].ToString()) % 5).ToString();
                    return result;
                })
                .Distinct()
                .Where(x => x.ToCharArray().Distinct().Count() == x.Length)
                .ToList();
        }

        int CheckThrusters(string signalPhase)
        {
            var intCode = new IntCode();
            var output = 0;

            for (int i = 0; i < signalPhase.Length; i++)
            {
                var signal = int.Parse(signalPhase[i].ToString());
                intCode.SetInputs(signal, output);
                intCode.Execute(Instructions);
                output = intCode.Output;
            }

            return output;
        }
    }
}
