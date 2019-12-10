using AdventOfCode.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise01 : Exercise
    {
        public override string FileName => "Exercise01";

        List<int> Masses => Inputs
            .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x))
            .ToList();

        public Exercise01()
            : base()
        {
        }

        public override string ProblemOne()
        {
            var result = 0;

            foreach (var mass in Masses)
                result += (mass / 3) - 2;

            return result.ToString();
        }

        public override string ProblemTwo()
        {
            var result = 0;

            foreach (var mass in Masses)
            {
                var fuel = Math.Max((mass / 3) - 2, 0);
                var maxFuel = fuel;

                while (fuel > 0)
                {
                    fuel = Math.Max((fuel / 3) - 2, 0);
                    maxFuel += fuel;
                }

                result += maxFuel;
            }

            return result.ToString();
        }
    }
}
