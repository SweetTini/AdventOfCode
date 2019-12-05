namespace AdventOfCode.Dependencies
{
    public interface IExercise
    {
        string Inputs { get; }

        int ProblemOne();

        int ProblemTwo();
    }
}
