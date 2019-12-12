using AdventOfCode.Dependencies;
using AdventOfCode.Dependencies.IntCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise11 : Exercise
    {
        public override string FileName => "Exercise11";

        List<long> Instructions => Inputs.Split(',').Select(x => long.Parse(x)).ToList();

        public override string ProblemOne()
        {
            var robot = new EmergencyHullPaintingRobot(128, 128);
            robot.Execute(Instructions);
            var result = robot.Steps;
            return $"{result}";
        }

        public override string ProblemTwo()
        {
            var robot = new EmergencyHullPaintingRobot(128, 128, true);
            robot.Execute(Instructions);
            robot.DrawPanel();
            return "See above.";
        }

        class EmergencyHullPaintingRobot
        {
            HashSet<Vector> _painted;
            bool _paintInit;

            public Vector Current { get; set; }

            public RobotDirection Direction { get; set; }

            public int Steps => _painted.Count;

            public int[,] Canvas { get; set; }

            public EmergencyHullPaintingRobot(int width, int height, bool paintInit = false)
                : base()
            {
                _painted = new HashSet<Vector>();
                _paintInit = paintInit;

                Canvas = new int[width, height];
                Current = new Vector(width / 2, height / 2);
            }

            public void Execute(IEnumerable<long> instructions)
            {
                Reset();

                var intCode = new IntCode();

                while (true)
                {
                    intCode.SetInputs(GetPanel());

                    if (intCode.IsPaused) intCode.Resume();
                    else intCode.Execute(instructions);

                    var lastIndex = intCode.Outputs.Count - 1;
                    var panelColor = (int)intCode.Outputs[lastIndex - 1];
                    var clockwise = Convert.ToBoolean((int)intCode.Outputs[lastIndex]);

                    SetPanel(panelColor);
                    Rotate(clockwise);
                    MoveForward();

                    if (!intCode.IsPaused) break;
                }
            }

            public void Reset()
            {
                var width = Canvas.GetLength(0);
                var height = Canvas.GetLength(1);

                _painted.Clear();

                Current = new Vector(width / 2, height / 2);
                Direction = RobotDirection.Up;
                Canvas = new int[width, height];

                if (_paintInit)
                    Canvas[(int)Current.X, (int)Current.Y] = 1;
            }

            public void SetPanel(int color)
            {
                Canvas[(int)Current.X, (int)Current.Y] = color;

                if (!_painted.Contains(Current))
                    _painted.Add(Current);
            }

            public int GetPanel()
            {
                return Canvas[(int)Current.X, (int)Current.Y];
            }

            public void MoveForward()
            {
                switch (Direction)
                {
                    case RobotDirection.Up: Current -= Vector.UnitY; break;
                    case RobotDirection.Right: Current += Vector.UnitX; break;
                    case RobotDirection.Down: Current += Vector.UnitY; break;
                    case RobotDirection.Left: Current -= Vector.UnitX; break;
                    default: break;
                }
            }

            public void Rotate(bool clockwise = true)
            {
                var direction = (int)Direction;

                if (clockwise) direction++;
                else direction--;

                direction = ((direction % 4) + 4) % 4;
                Direction = (RobotDirection)direction;
            }

            public void DrawPanel()
            {
                for (int j = 0; j < Canvas.GetLength(1); j++)
                {
                    for (int i = 0; i < Canvas.GetLength(0); i++)
                    {
                        var color = Canvas[i, j];
                        DrawPixel(color);
                    }

                    Console.WriteLine();
                }
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
                Console.Write(colorBit.ToString());
                Console.BackgroundColor = lastBgColor;
                Console.ForegroundColor = lastFgColor;
            }
        }

        enum RobotDirection
        {
            Up,
            Right,
            Down,
            Left
        }
    }
}
