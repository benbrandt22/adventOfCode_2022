// See https://aka.ms/new-console-template for more information

using Core.Modules;
using NodaTime;
using NodaTime.Extensions;

void RunModuleFor(LocalDate date)
{
    var module = ModuleLoader.GetModuleFor(date);
    Console.WriteLine($"Day {module.Day} - {module.GetType().Name}");
    Console.WriteLine(new string('-',60));
    module.Execute();
}

var today = DateTime.Now.ToLocalDateTime().Date;

Console.WriteLine(new string('-',50));
Console.WriteLine($"Advent of Code 2022 / {today}");
Console.WriteLine(new string('-',60));

RunModuleFor(today);