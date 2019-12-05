using System;

namespace AdventOfCode.Dependencies
{
    public struct Vector
    {
        public static Vector Zero => new Vector(0, 0);

        public static Vector One => new Vector(1, 1);

        public static Vector UnitX => new Vector(1, 0);

        public static Vector UnitY => new Vector(0, 1);

        public float X { get; set; }

        public float Y { get; set; }

        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector Round() => new Vector((float)Math.Round(X), (float)Math.Round(Y));

        public static float Dot(Vector a, Vector b) => a.X * b.X + a.Y * b.Y;

        public static float Det(Vector a, Vector b) => a.X * b.Y - a.Y * b.X;

        public static float ManhattanDistance(Vector a) => ManhattanDistance(Vector.Zero, a);

        public static float ManhattanDistance(Vector a, Vector b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        public static Vector operator -(Vector lhs) => new Vector(-lhs.X, -lhs.Y);

        public static Vector operator +(Vector lhs, Vector rhs) => new Vector(lhs.X + rhs.X, lhs.Y + rhs.Y);

        public static Vector operator -(Vector lhs, Vector rhs) => new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y);

        public static Vector operator *(Vector lhs, Vector rhs) => new Vector(lhs.X * rhs.X, lhs.Y * rhs.Y);

        public static Vector operator *(float lhs, Vector rhs) => new Vector(lhs * rhs.X, lhs * rhs.Y);

        public static Vector operator *(Vector lhs, float rhs) => new Vector(lhs.X * rhs, lhs.Y * rhs);

        public static Vector operator /(Vector lhs, Vector rhs) => new Vector(lhs.X / rhs.X, lhs.Y / rhs.Y);

        public static Vector operator /(Vector lhs, float rhs) => new Vector(lhs.X / rhs, lhs.Y / rhs);

        public static Vector operator /(float lhs, Vector rhs) => new Vector(lhs / rhs.X, lhs / rhs.Y);

        public static bool operator ==(Vector lhs, Vector rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;

        public static bool operator !=(Vector lhs, Vector rhs) => !(lhs == rhs);

        public override bool Equals(object obj) => this == (Vector)obj;

        public override int GetHashCode() => X.GetHashCode() + Y.GetHashCode();

        public override string ToString() => $"({X}, {Y})";
    }
}
