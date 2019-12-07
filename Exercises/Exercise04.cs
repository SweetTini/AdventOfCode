using AdventOfCode.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise04 : Exercise
    {
        public override string FileName => "Exercise04";

        int Lowest => int.Parse(Inputs.Split('-')[0]);

        int Highest => int.Parse(Inputs.Split('-')[1]);

        public Exercise04()
            : base()
        {
        }

        public override int ProblemOne()
        {
            var result = 0;
            var rules = SetUpRules(IsValidPassword, HasPairs, IsEachDigitIncreasing);

            for (int i = Lowest; i <= Highest; i++)
                if (rules.All(x => x.Invoke(i.ToString())))
                    result++;

            return result;
        }

        public override int ProblemTwo()
        {
            var result = 0;
            var rules = SetUpRules(IsValidPassword, HasAdjacentPair, IsEachDigitIncreasing);

            for (int i = Lowest; i <= Highest; i++)
                if (rules.All(x => x.Invoke(i.ToString())))
                    result++;

            return result;
        }

        List<Func<string, bool>> SetUpRules(params Func<string, bool>[] rules)
        {
            return new List<Func<string, bool>>(rules);
        }

        List<string> FindPairs(string input)
        {
            var pairs = new List<string>();
            var output = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i].ToString();

                if (string.IsNullOrEmpty(output))
                {
                    output = current;
                    continue;
                }

                if (output[output.Length - 1].ToString() == current)
                {
                    output += current;
                }
                else
                {
                    if (output.Length > 1)
                        pairs.Add(output);
                    output = current;
                }

                if (i == input.Length - 1 && output.Length > 1)
                    pairs.Add(output);
            }

            return pairs;
        }

        bool IsValidPassword(string input) => input.Length == 6;

        bool HasPairs(string input) => FindPairs(input).Any();

        bool IsEachDigitIncreasing(string input)
        {
            for (int i = 0; i < input.Length - 1; i++)
            {
                var current = int.Parse(input[i].ToString());
                var next = int.Parse(input[i + 1].ToString());
                if (current > next) return false;
            }

            return true;
        }

        bool HasAdjacentPair(string input)
        {
            var pairs = FindPairs(input);
            return pairs.Any() && pairs.Min(x => x.Length) == 2;
        }
    }
}
