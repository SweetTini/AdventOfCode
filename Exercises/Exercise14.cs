using AdventOfCode.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise14 : Exercise
    {
        public override string FileName => "Exercise14";

        public override string ProblemOne()
        {
            var reactions = GetReactions();
            var result = GetOresNeed(reactions, 1);
            return result.ToString();
        }

        public override string ProblemTwo()
        {
            var reactions = GetReactions();
            var result = GetFuelNeed(reactions, 1000000000000);
            return result.ToString();
        }

        long GetOresNeed(List<Reaction> reactions, long fuel)
        {
            var copy = reactions.ToList();
            var ores = copy.Where(x => x.Requirements.Any(y => y.Name == "ORE")).ToList();
            ores.ForEach(x => copy.Remove(x));

            var surplus = new Dictionary<string, long>();

            var materials = GetMaterial(copy, surplus, "FUEL", fuel)
                .GroupBy(x => x.Name)
                .Select(x => new Material(x.Key, x.Sum(y => y.Quantity)))
                .ToList();

            var oresNeed = materials
                .Select(x => GetOreAmount(ores, x))
                .Sum(x => x.Quantity);

            return oresNeed;
        }

        long GetFuelNeed(List<Reaction> reactions, long ores)
        {
            var lowest = ores / GetOresNeed(reactions, 1);
            var highest = ores;

            while ((highest - lowest) > 1)
            {
                var fuel = (lowest + highest) / 2L;
                if (GetOresNeed(reactions, fuel) <= ores)
                    lowest = fuel;
                else highest = fuel;
            }

            return lowest;
        }

        List<Material> GetMaterial(List<Reaction> reactions, Dictionary<string, long> surplus, string name, long amount)
        {
            var reaction = reactions.FirstOrDefault(x => x.Name == name);
            if (reaction == null) return new List<Material>() { new Material(name, amount) };
            if (surplus.ContainsKey(name))
            {
                amount -= surplus[name];
                surplus[name] = 0;
            }

            var scale = (long)Math.Ceiling(1f * amount / reaction.Quantity);
            if (reaction.Quantity * scale > amount)
            {
                var surplusAmount = (reaction.Quantity * scale) - amount;
                if (!surplus.ContainsKey(name)) surplus.Add(name, surplusAmount);
                else surplus[name] += surplusAmount;
            }

            return reaction.Requirements
                .Select(x => GetMaterial(reactions, surplus, x.Name, x.Quantity * scale))
                .SelectMany(x => x)
                .OrderBy(x => x.Name)
                .ToList();
        }

        Material GetOreAmount(List<Reaction> ores, Material material)
        {
            var ore = ores.First(x => x.Name == material.Name);
            var scale = (long)Math.Ceiling(1f * material.Quantity / ore.Quantity);
            var amount = ore.Requirements[0].Quantity * scale;
            return new Material("ORE", amount);
        }

        List<Reaction> GetReactions()
        {
            var reactions = new List<Reaction>();

            var inputs = Inputs
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            foreach (var input in inputs)
            {
                var breakdown = input
                    .Split(new[] { "=>" }, StringSplitOptions.None)
                    .Select(x => x.Trim())
                    .ToArray();

                var requirements = breakdown[0]
                    .Split(',')
                    .Select(x => x.Trim().Split(' '))
                    .Select(x => new Material(x[1], int.Parse(x[0])))
                    .ToList();

                var outputs = new Material(
                    breakdown[1].Split(' ')[1],
                    int.Parse(breakdown[1].Split(' ')[0]));

                reactions.Add(new Reaction()
                {
                    Output = outputs,
                    Requirements = requirements
                });
            }

            return reactions;
        }

        class Reaction
        {
            public Material Output { get; set; }

            public string Name => Output.Name;

            public long Quantity => Output.Quantity;

            public List<Material> Requirements { get; set; }
        }

        struct Material
        {
            public string Name { get; set; }

            public long Quantity { get; set; }

            public Material(string name, long quantity)
            {
                Name = name;
                Quantity = quantity;
            }

            public override string ToString()
            {
                return $"M={Name}, Q={Quantity}";
            }
        }
    }
}
