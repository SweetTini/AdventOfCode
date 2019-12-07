using AdventOfCode.Dependencies;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise06 : Exercise
    {
        public override string FileName => "Exercise06";

        List<string> OrbitMappings => Inputs
            .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        public Exercise06()
            : base()
        {
        }

        public override int ProblemOne()
        {
            var orbits = GetOrbits(OrbitMappings);
            return orbits.Sum(x => x.Count);
        }

        public override int ProblemTwo()
        {
            var orbits = GetOrbits(OrbitMappings);
            var you = orbits.First(x => x.Identity == "YOU");
            var santa = orbits.First(x => x.Identity == "SAN");
            return you.Search(santa);
        }

        HashSet<Orbit> GetOrbits(List<string> mappings)
        {
            var orbits = new HashSet<Orbit>();

            foreach (var mapping in mappings)
            {
                var temp = mapping.Split(')');
                var fromOrbit = orbits.FirstOrDefault(x => x.Identity == temp[0]) ?? new Orbit(temp[0]);
                var toOrbit = orbits.FirstOrDefault(x => x.Identity == temp[1]) ?? new Orbit(temp[1]);

                if (!orbits.Contains(fromOrbit)) orbits.Add(fromOrbit);
                if (!orbits.Contains(toOrbit)) orbits.Add(toOrbit);

                fromOrbit.Link(toOrbit);
            }

            return orbits;
        }

        class Orbit
        {
            List<Orbit> _next;

            public string Identity { get; private set; }

            public Orbit Previous { get; private set; } = null;

            public ReadOnlyCollection<Orbit> Next { get; private set; }

            public int Count
            {
                get
                {
                    var count = 0;
                    var lastOrbit = Previous;

                    while (lastOrbit != null)
                    {
                        count++;
                        lastOrbit = lastOrbit.Previous;
                    }

                    return count;
                }
            }

            Orbit()
                : base()
            {
                _next = new List<Orbit>();
                Next = new ReadOnlyCollection<Orbit>(_next);
            }

            public Orbit(string identity)
                : this()
            {
                Identity = identity;
            }

            public int Search(Orbit orbit)
            {
                var orbitsSearched = 0;
                Orbit nextOrbit = this;

                do
                {
                    var lastOrbit = nextOrbit;
                    nextOrbit = nextOrbit.Next.FirstOrDefault(x => x.IsLinked(orbit)) ?? nextOrbit.Previous;
                    if (!lastOrbit.Equals(this) && !nextOrbit.Equals(orbit))
                        orbitsSearched++;
                }
                while (!nextOrbit.Equals(orbit));

                return orbitsSearched;
            }

            public void Link(Orbit orbit)
            {
                _next.Add(orbit);
                orbit.Previous = this;
            }

            public bool IsLinked(Orbit orbit)
            {
                return orbit.Equals(this) || _next.Any(x => x.IsLinked(orbit));
            }
        }
    }
}
