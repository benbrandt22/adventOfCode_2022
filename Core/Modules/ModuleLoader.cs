using NodaTime;

namespace Core.Modules;

public static class ModuleLoader
{
    public static IDayModule GetModuleFor(LocalDate date)
    {
        if (date.Year == 2022 && date.Month == 12)
        {
            return GetModuleForDay(date.Day);
        }
        return new DayNotFoundModule();
    }
    
    public static IDayModule GetModuleForDay(int day) =>
        GetAllModules()
            .FirstOrDefault(m => m.Day == day) ?? new DayNotFoundModule();

    private static IEnumerable<IDayModule> GetAllModules() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(x => !x.IsAbstract && typeof(IDayModule).IsAssignableFrom(x))
            .Select(Activator.CreateInstance)!
            .Cast<IDayModule>();
}