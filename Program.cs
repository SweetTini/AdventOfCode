using AdventOfCode.Exercises;
using System;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var exercise = new Exercise06();

            Console.WriteLine("EXERCISE #6:");
            Console.WriteLine($"Problem #1 = {exercise.ProblemOne()}");
            Console.WriteLine($"Problem #2 = {exercise.ProblemTwo()}");
            Console.ReadLine();
        }
    }
}
