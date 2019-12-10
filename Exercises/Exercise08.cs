using AdventOfCode.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise08 : Exercise
    {
        public override string FileName => "Exercise08";

        List<int> ColorBits => Inputs
            .ToCharArray()
            .Where(x => char.IsDigit(x))
            .Select(x => int.Parse(x.ToString()))
            .ToList();

        public override string ProblemOne()
        {
            var image = GetImage(ColorBits, 25, 6);
            var layer = image.OrderBy(x => x.Count(y => y == 0)).First();
            var result = layer.Count(x => x == 1) * layer.Count(x => x == 2);
            return result.ToString();
        }

        public override string ProblemTwo()
        {
            var image = GetImage(ColorBits, 25, 6);
            DrawImage(image, 25, 6);
            return "See above.";
        }

        List<int[]> GetImage(List<int> bits, int width, int height)
        {
            return bits
                .Select((x, i) => new { Index = i / (width * height), Data = x })
                .GroupBy(x => x.Index)
                .ToDictionary(x => x.Key, y => y.Select(x => x.Data).ToArray())
                .Select(x => x.Value)
                .ToList();
        }

        void DrawImage(List<int[]> image, int width, int height)
        {
            var area = width * height;

            for (int i = 0; i < area; i++)
            {
                var colorBit = image.Select(x => x[i]).Where(x => x < 2).First();
                if (i > 0 && i % width == 0) Console.WriteLine();
                DrawPixel(colorBit);
            }

            Console.WriteLine();
        }

        void DrawPixel(int colorBit)
        {
            var lastBgColor = Console.BackgroundColor;
            var lastFgColor = Console.ForegroundColor;
            ConsoleColor bgColor, fgColor;

            switch (colorBit)
            {
                case 0: bgColor = ConsoleColor.Black; fgColor = ConsoleColor.DarkGray; break;
                case 1: bgColor = ConsoleColor.Gray; fgColor = ConsoleColor.White; break;
                default: bgColor = ConsoleColor.Black; fgColor = ConsoleColor.Black; break;
            }

            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
            Console.Write(colorBit);
            Console.BackgroundColor = lastBgColor;
            Console.ForegroundColor = lastFgColor;
        }
    }
}
