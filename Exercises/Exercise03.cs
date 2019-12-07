using AdventOfCode.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise03 : Exercise
    {
        public override string FileName => "Exercise03";

        List<List<Vector>> VectorSets => Inputs
            .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            .Select(GetVectors)
            .ToList();

        public Exercise03()
            : base()
        {
        }

        public override int ProblemOne()
        {
            var points = FindIntersections(VectorSets[0], VectorSets[1]);
            var result = points.Min(x => (int)Vector.ManhattanDistance(x));
            return result;
        }

        public override int ProblemTwo()
        {
            var points = FindIntersections(VectorSets[0], VectorSets[1]);
            var result = points.Min(x => GetSteps(x, VectorSets[0]) + GetSteps(x, VectorSets[1]));
            return result;
        }

        List<Vector> GetVectors(string input)
        {
            var vectors = new List<Vector>() { Vector.Zero };
            var paths = input.Split(',').ToList();
            var lastVector = vectors[0];

            foreach (var path in paths)
            {
                var direction = path.Substring(0, 1);
                var length = int.Parse(path.Substring(1, path.Length - 1));
                var vector = lastVector;

                switch (direction)
                {
                    case "L": vector = vector - Vector.UnitX * length; break;
                    case "R": vector = vector + Vector.UnitX * length; break;
                    case "U": vector = vector + Vector.UnitY * length; break;
                    case "D": vector = vector - Vector.UnitY * length; break;
                    default: throw new Exception("Unrecognizable direction.");
                }

                vectors.Add(vector);
                lastVector = vector;
            }

            return vectors;
        }

        List<Vector> FindIntersections(List<Vector> setOne, List<Vector> setTwo)
        {
            var vectors = new List<Vector>();

            for (int i = 0; i < setOne.Count - 1; i++)
            {
                for (int j = 0; j < setTwo.Count - 1; j++)
                {
                    Vector p;
                    var intersect = CheckIntersection(setOne[i], setOne[i + 1], setTwo[j], setTwo[j + 1], out p);
                    if (intersect && p != Vector.Zero) vectors.Add(p);
                }
            }

            return vectors;
        }

        bool CheckIntersection(Vector a1, Vector a2, Vector p)
        {
            var a = a2 - a1;
            var b = p - a1;
            var det = Vector.Det(a, b);
            if (det != 0f) return false;

            var dp = Vector.Dot(b, a);
            if (dp < 0) return false;
            var l = Vector.Dot(a, a);
            if (dp > l) return false;

            return true;
        }

        bool CheckIntersection(Vector a1, Vector a2, Vector b1, Vector b2, out Vector p)
        {
            p = Vector.Zero;

            var a = a2 - a1;
            var b = b2 - b1;
            var det = Vector.Det(a, b);
            if (det == 0f) return false;

            var c = b1 - a1;
            var t = Vector.Det(c, b) / det;
            if (!Between(t, 0, 1)) return false;
            var u = Vector.Det(c, a) / det;
            if (!Between(u, 0, 1)) return false;

            p = (a1 + t * a).Round();
            return true;
        }

        int GetSteps(Vector vector, List<Vector> set)
        {
            var totalSteps = 0;

            for (int i = 1; i < set.Count; i++)
            {
                var p1 = set[i - 1];
                var p2 = set[i];

                totalSteps += (int)Vector.ManhattanDistance(p2 - p1);

                if (CheckIntersection(p1, p2, vector))
                {
                    totalSteps -= (int)Vector.ManhattanDistance(p2 - vector);
                    break;
                }
            }

            return totalSteps;
        }

        bool Between(float x, float a, float b) => a <= x && x <= b;
    }
}
