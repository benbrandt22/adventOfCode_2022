namespace Core.Modules;

public class DayNotFoundModule : BaseDayModule
{
    public override int Day => -1;
    public override void Execute()
    {
        Output.WriteLine("Module not found");
    }
}