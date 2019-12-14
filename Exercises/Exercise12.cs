using AdventOfCode.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise12 : Exercise
    {
        public override string FileName => "Exercise12";

        public override string ProblemOne()
        {
            var coords = GetMoonCoordinates();
            var velocs = Enumerable.Repeat(new Vector3(), 4).ToList();
            var result = GetTotalEnergy(coords, velocs, 1000);
            return result.ToString();
        }

        public override string ProblemTwo()
        {
            var coords = GetMoonCoordinates();
            var velocs = Enumerable.Repeat(new Vector3(), 4).ToList();
            var result = SimulateOrbit(coords, velocs);
            return result.ToString();
        }

        int GetTotalEnergy(List<Vector3> coords, List<Vector3> velocs, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                ApplyGravity(coords, velocs);
                coords = coords.Select((x, j) => x + velocs[j]).ToList();
            }

            return coords
                .Select((x, i) => Vector3.Abs(x).Sum() * Vector3.Abs(velocs[i]).Sum())
                .Sum(x => x);
        }

        long SimulateOrbit(List<Vector3> coords, List<Vector3> velocs)
        {
            var steps = 0L;
            var xStep = 0L;
            var yStep = 0L;
            var zStep = 0L;

            var initCoords = coords.ToList();
            
            do
            {
                ApplyGravity(coords, velocs);
                coords = coords.Select((x, i) => x + velocs[i]).ToList();
                steps++;

                var xMatch = coords.Zip(initCoords, (n, o) => n.X == o.X).All(x => x) && velocs.All(x => x.X == 0);
                var yMatch = coords.Zip(initCoords, (n, o) => n.Y == o.Y).All(x => x) && velocs.All(x => x.Y == 0);
                var zMatch = coords.Zip(initCoords, (n, o) => n.Z == o.Z).All(x => x) && velocs.All(x => x.Z == 0);

                if (xStep == 0 && xMatch) xStep = steps;
                if (yStep == 0 && yMatch) yStep = steps;
                if (zStep == 0 && zMatch) zStep = steps;
            }
            while (xStep == 0 || yStep == 0 || zStep == 0);

            return GetLCM(xStep, GetLCM(yStep, zStep));
        }

        List<Vector3> GetMoonCoordinates()
        {
            return Inputs
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { ' ', '<', '>', '=', 'x', 'y', 'z', ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new Vector3(int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2])))
                .ToList();
        }

        void ApplyGravity(List<Vector3> coordinates, List<Vector3> velocities)
        {
            var exclude = new Dictionary<int, List<int>>();

            for (int i = 0; i < coordinates.Count * coordinates.Count; i++)
            {
                var iA = i % coordinates.Count;
                var iB = i / coordinates.Count;
                var canSkip = iA == iB
                        || exclude.ContainsKey(iA) && exclude[iA].Contains(iB)
                        || exclude.ContainsKey(iB) && exclude[iB].Contains(iA);
                if (canSkip) continue;

                var velA = velocities[iA];
                var velB = velocities[iB];

                ApplyGravity(ref velA, ref velB, coordinates[iA], coordinates[iB]);
                velocities[iA] = velA;
                velocities[iB] = velB;

                if (!exclude.ContainsKey(iA)) exclude.Add(iA, new List<int>());
                if (!exclude.ContainsKey(iB)) exclude.Add(iB, new List<int>());

                exclude[iA].Add(iB);
                exclude[iB].Add(iA);
            }
        }

        void ApplyGravity(ref Vector3 vA, ref Vector3 vB, Vector3 pA, Vector3 pB)
        {
            var gravity = new Vector3(
                ApplyGravity(pA.X, pB.X),
                ApplyGravity(pA.Y, pB.Y),
                ApplyGravity(pA.Z, pB.Z));

            vA += gravity;
            vB -= gravity;
        }

        int ApplyGravity(int cA, int cB) => cA < cB ? 1 : cA > cB ? -1 : 0;

        long GetGCD(long a, long b) => a == 0 ? b : GetGCD(b % a, a);

        long GetLCM(long a, long b) => (a * b) / GetGCD(a, b);

        struct Vector3
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Z { get; set; }

            public static Vector3 Zero => new Vector3(0, 0, 0);

            public Vector3(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public int Sum() => X + Y + Z;

            public static Vector3 operator -(Vector3 lhs) => new Vector3(-lhs.X, -lhs.Y, -lhs.Z);

            public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
            {
                return new Vector3(
                    lhs.X + rhs.X, 
                    lhs.Y + rhs.Y,
                    lhs.Z + rhs.Z);
            }

            public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
            {
                return new Vector3(
                    lhs.X - rhs.X,
                    lhs.Y - rhs.Y,
                    lhs.Z - rhs.Z);
            }

            public static bool operator ==(Vector3 lhs, Vector3 rhs)
            {
                return lhs.X == rhs.X
                    && lhs.Y == rhs.Y
                    && lhs.Z == rhs.Z;
            }

            public static bool operator !=(Vector3 lhs, Vector3 rhs)
            {
                return lhs.X != rhs.X
                    || lhs.Y != rhs.Y
                    || lhs.Z != rhs.Z;
            }

            public static Vector3 Abs(Vector3 v)
            {
                return new Vector3(
                    Math.Abs(v.X),
                    Math.Abs(v.Y),
                    Math.Abs(v.Z));
            }

            public override bool Equals(object obj)
            {

                return obj is Vector3
                    && ((Vector3)obj).X == X
                    && ((Vector3)obj).Y == Y
                    && ((Vector3)obj).Z == Z;
            }

            public override int GetHashCode() => X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();

            public override string ToString() => $"({X}, {Y}, {Z})";
        }
    }
}
