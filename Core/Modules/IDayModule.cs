namespace Core.Modules;

public interface IDayModule
{
    public int Day { get; }
    public void Execute();
}