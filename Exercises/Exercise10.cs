using AdventOfCode.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Exercises
{
    public class Exercise10 : Exercise
    {
        public override string FileName => "Exercise10";

        public override string ProblemOne()
        {
            var asteroids = GetAsteroidCoordinates();
            var lineOfSightCount = asteroids
                .Select(x => GetAsteroidSightings(x, asteroids))
                .OrderByDescending(x => x.Count)
                .ToList();
            var result = lineOfSightCount.First();
            return $"{result.Coord} = {result.Count}";
        }

        public override string ProblemTwo()
        {
            var asteroids = GetAsteroidCoordinates();
            var baseAsteroid = asteroids
                .Select(x => GetAsteroidSightings(x, asteroids))
                .OrderByDescending(x => x.Count)
                .First();
            var result = GetVaporizingOrder(baseAsteroid.Coord, asteroids)
                .Select((x, i) => new { Rank = i + 1, Coord = x })
                .First(x => x.Rank == 200).Coord;
            return $"{(result.X * 100 + result.Y)}";
        }

        List<Vector> GetAsteroidCoordinates()
        {
            return Inputs
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select((y, ri) =>
                    y.Select((x, ci) => new { Index = ci, Column = x.ToString() })
                        .Where(x => x.Column == "#")
                        .Select(x => new Vector(x.Index, ri))
                        .ToList())
                .SelectMany(v => v)
                .ToList();
        }

        AsteroidSighting GetAsteroidSightings(Vector asteroid, List<Vector> coords)
        {
            coords = coords.Where(x => x != asteroid).ToList();

            var sightings = coords
                .Select(x => new { Angle = Vector.Angle(asteroid, x), Coord = x })
                .GroupBy(x => x.Angle)
                .Count();

            return new AsteroidSighting(asteroid, sightings);
        }

        List<Vector> GetVaporizingOrder(Vector asteroid, List<Vector> coords)
        {    
            coords = coords.Where(x => x != asteroid).ToList();

            var sightings = coords
                .Select(x => new
                {
                    Angle = (int)Math.Floor(Vector.Angle(asteroid, x)) % 360,
                    PreciseAngle = Vector.Angle(asteroid, x) % 360,
                    Distance = Vector.DistanceSquared(asteroid, x),
                    Coord = x
                })
                .GroupBy(x => x.Angle)
                .OrderBy(x => x.Key)
                .ToDictionary(
                    x => x.Key, 
                    y => y.Select(x => new AsteroidLineOfSight(x.Coord, x.PreciseAngle, x.Distance)).ToList());

            return Vaporize(sightings);
        }

        List<Vector> Vaporize(Dictionary<int, List<AsteroidLineOfSight>> sightings)
        {
            var order = new List<Vector>();
            var start = 270;
            var end = start + 360;

            for (int i = start; i <= end; i++)
            {
                List<AsteroidLineOfSight> sighting;

                if (sightings.TryGetValue(i % 360, out sighting))
                {
                    var asteroids = sighting
                        .GroupBy(x => x.Angle)
                        .OrderBy(x => x.Key)
                        .Select(x => x.OrderBy(y => y.Distance).First())
                        .ToList();

                    foreach (var asteroid in asteroids)
                    {
                        order.Add(asteroid.Coord);
                        sighting.Remove(asteroid);
                    }
                }
            }

            return order;
        }

        struct AsteroidSighting
        {
            public Vector Coord { get; set; }

            public int Count { get; set; }

            public AsteroidSighting(Vector coord, int count)
                : this()
            {
                Coord = coord;
                Count = count;
            }
        }

        struct AsteroidLineOfSight
        {
            public Vector Coord { get; set; }

            public float Angle { get; set; }

            public float Distance { get; set; }

            public AsteroidLineOfSight(Vector coord, float angle, float distance)
                : this()
            {
                Coord = coord;
                Angle = angle;
                Distance = distance;
            }

            public override string ToString()
            {
                return $"{Coord}, DST={Distance}, ANG={Angle}";
            }
        }
    }
}
