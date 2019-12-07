using System.IO;
using System.Reflection;

namespace AdventOfCode.Dependencies
{
    public abstract class Exercise : IExercise
    {
        public abstract string FileName { get; }

        public string Inputs { get; private set; }

        protected Exercise()
            : base()
        {
            LoadInputs();
        }

        public abstract int ProblemOne();

        public abstract int ProblemTwo();

        void LoadInputs()
        {
            try
            {
                var rootDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                Inputs = File.ReadAllText(Path.Combine(rootDirectory, $"Inputs\\{FileName}.txt"));
            }
            catch
            {
                Inputs = string.Empty;
            }
        }
    }
}
