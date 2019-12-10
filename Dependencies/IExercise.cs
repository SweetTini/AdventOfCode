namespace AdventOfCode.Dependencies
{
    public interface IExercise
    {
        string FileName { get; }

        string ProblemOne();

        string ProblemTwo();
    }
}
