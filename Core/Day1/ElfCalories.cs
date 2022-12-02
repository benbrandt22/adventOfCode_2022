using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day1;

public class ElfCalories : BaseDayModule
{
    public override int Day => 1;
    public override void Execute()
    {
        GetCalories("Day1/day1-sample.txt");
        GetCalories("Day1/day1-input.txt");
    }

    private void GetCalories(string filename)
    {
        WriteLine($"Finding the elf with the most calories in {filename} ...");

        var elfCalorieGroups = TextFileLoader
            .LoadLines(filename)
            .SplitOnEmptyStrings()
            .Select(stringList => stringList.Select(int.Parse).ToList())
            .ToList();
        
        //elfCalorieGroups.ForEach(ints => WriteLine( string.Join('/',ints) ));

        var elfTotals = elfCalorieGroups
            .Select(cals =>
            {
                var elfTotal = cals.Sum();
                //WriteLine($"{string.Join('/', cals)} = {elfTotal}");
                return elfTotal;
            })
            .ToList();
        
        WriteLine($"[{filename}]");
        WriteLine($"Most calories held by one elf: {elfTotals.Max()}");

        var topX = 3;
        var topElvesByCalories = elfTotals
            .OrderByDescending(x => x)
            .Take(topX)
            .ToList();
        
        WriteLine($"Top {topX} elves by calories: {string.Join('/', topElvesByCalories)}");
        WriteLine($"Total of those top {topX}: {topElvesByCalories.Sum()}");
    }
}