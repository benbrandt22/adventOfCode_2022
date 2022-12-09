namespace Core.Shared.Modules;

public abstract class BaseDayModule : IDayModule
{
    public abstract int Day { get; }
    public abstract void Execute();

    internal TextWriter Output = Console.Out;

    internal void WriteLine(string line = "") => Output.WriteLine(line);

    internal void WriteHorizontalRule(int length = 80) => WriteLine(new string('-',80));
}