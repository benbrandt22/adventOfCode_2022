// See https://aka.ms/new-console-template for more information

using Core.Shared.Modules;


var module = ModuleLoader.GetModule();
Console.WriteLine(new string('-',80));
Console.WriteLine($"Advent of Code 2022 - Day {module.Day} - {module.GetType().Name}");
Console.WriteLine(new string('-',80));
module.Execute();