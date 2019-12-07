namespace AdventOfCode.Dependencies
{
    public interface IExercise
    {
        string FileName { get; }

        int ProblemOne();

        int ProblemTwo();
    }
}
