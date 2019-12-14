using AdventOfCode.Dependencies;
using AdventOfCode.Dependencies.IntCode;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Exercises
{
    public class Exercise13 : Exercise
    {
        public override string FileName => "Exercise13";

        List<long> Instructions => Inputs.Split(',').Select(x => long.Parse(x)).ToList();

        public override string ProblemOne()
        {
            var intCode = new IntCode(4096);
            intCode.Execute(Instructions);

            var tiles = GetTiles(intCode.Outputs.ToList());
            var result = tiles.Count(x => x.TileType == TileType.Block);
            return result.ToString();
        }

        public override string ProblemTwo()
        {
            var result = PlayGame();
            return result.ToString();
        }

        List<Tile> GetTiles(List<long> output)
        {
            var tiles = new List<Tile>();
            var offset = 0;

            while (offset < output.Count)
            {
                tiles.Add(new Tile(
                    (int)output[offset], 
                    (int)output[offset + 1], 
                    (int)output[offset + 2]));

                offset += 3;
            }

            return tiles;
        }

        long PlayGame()
        {
            var intCode = new IntCode(4096);
            var instructions = Instructions.ToList();
            instructions[0] = 2;

            var score = 0L;

            while (true)
            {
                if (intCode.IsPaused) intCode.Resume();
                else intCode.Execute(instructions);

                var tiles = GetTiles(intCode.Outputs.ToList());
                var paddle = (Tile?)tiles.FirstOrDefault(x => x.TileType == TileType.Paddle);
                var ball = (Tile?)tiles.FirstOrDefault(x => x.TileType == TileType.Ball);
                var scoreInfo = (Tile?)tiles.FirstOrDefault(x => x.IsScore);

                if (scoreInfo.HasValue)
                    score = scoreInfo.Value.Score;

                intCode.ClearOutput();

                if (!intCode.IsPaused) break;

                if (paddle.HasValue && ball.HasValue)
                    intCode.SetInputs(GetInput(paddle.Value, ball.Value));
                else intCode.SetInputs(0);
            }

            return score;
        }

        int GetInput(Tile paddle, Tile ball)
        {
            return ball.X < paddle.X ? -1 : ball.X > paddle.X ? 1 : 0;
        }

        struct Tile
        {
            public Vector Position { get; set; }

            public int X => (int)Position.X;

            public int Y => (int)Position.Y;

            public TileType TileType { get; set; }

            public long Score { get; set; }

            public bool IsScore => Position == new Vector(-1, 0);

            public Tile(int x, int y, long type)
                : this()
            {
                Position = new Vector(x, y);

                if (IsScore) Score = type;
                else TileType = (TileType)type;
            }
        }

        enum TileType
        {
            Empty,
            Wall,
            Block,
            Paddle,
            Ball
        }
    }
}
