using AdventOfCode.Exercises;
using System;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var exercise = new Exercise11();

            Console.WriteLine("EXERCISE #11:");
            Console.WriteLine($"Problem #1 = {exercise.ProblemOne()}");
            Console.WriteLine($"Problem #2 = {exercise.ProblemTwo()}");
            Console.WriteLine("\nPress ENTER to exit console.");
            Console.ReadLine();
        }
    }
}
