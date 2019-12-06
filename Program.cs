using AdventOfCode.Exercises;
using System;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var exercise = new Exercise05();

            Console.WriteLine("EXERCISE #5:");
            Console.WriteLine($"Problem #1 = {exercise.ProblemOne()}");
            Console.WriteLine($"Problem #2 = {exercise.ProblemTwo()}");
            Console.ReadLine();
        }
    }
}
