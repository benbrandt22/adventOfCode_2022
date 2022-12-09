using System.Diagnostics;
using Core.Shared;
using Core.Shared.Modules;

namespace Core.Day3;

public class RucksackReorganization : BaseDayModule
{
    public override int Day => 3;
    
    public override void Execute()
    {
        AnalyzeRucksacks("Day3/sample.txt");
        AnalyzeRucksacks("Day3/input.txt");
    }

    private void AnalyzeRucksacks(string filename)
    {
        WriteLine();
        WriteLine($"Analyzing Rucksacks from {filename} ...");

        var rucksacks = TextFileLoader
            .LoadLines(filename)
            .Select(line => new Rucksack(line))
            .ToList();
        
        WriteLine($"Loaded {rucksacks.Count} rucksacks");

        var prioritySum = rucksacks.Sum(r => r.PriorityScore);
        
        WriteLine($"Total Priority Score: {prioritySum}");
        
        WriteLine("Part 2...");
        const int groupSize = 3;
        var elfGroups = rucksacks.Chunk(groupSize).ToList();
        WriteLine($"Split into {elfGroups.Count} groups of {groupSize}");

        var badgePrioritySum = elfGroups
            .Select(g =>
            {
                var allRucksackContents = g.Select(r => $"{r.Compartment1}{r.Compartment2}").ToArray();
                var commonChars = CharactersFoundInAllStrings(allRucksackContents);
                return commonChars.Single(); // should throw error if more than one is found
            })
            .Select(GetItemPriorityScore)
            .Sum();

        WriteLine($"Badge priority sum of all groups: {badgePrioritySum}");
    }
    
    public static int GetItemPriorityScore(char c)
    {
        // a through z have priorities 1 through 26, A through Z have priorities 27 through 52
        const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (!letters.Contains(c)) { throw new ArgumentException($"Unsupported character: {c}"); }
        var priorityScore = (letters.IndexOf(c) + 1);
        return priorityScore;
    }
    
    public static char[] CharactersFoundInAllStrings(params string[] strings)
    {
        var allUniqueChars = string.Join("", strings).Distinct().ToList();
        var charsInAllStrings = allUniqueChars
            .Where(c => strings.All(s => s.Contains(c)))
            .ToArray();
        return charsInAllStrings;
    }
}

[DebuggerDisplay("{Compartment1} | {Compartment2} => {CommonItem} => Score {PriorityScore}")]
public class Rucksack
{
    public readonly string Compartment1;
    public readonly string Compartment2;
    public readonly char CommonItem;
    public readonly int PriorityScore;

    public Rucksack(string contents)
    {
        if (string.IsNullOrWhiteSpace(contents)) { throw new ArgumentException("empty contents"); }
        if (contents.Length % 2 != 0) { throw new ArgumentException("expected contents to have an even-numbered length"); }
        // split the contents string into two halves
        Compartment1 = contents.Substring(0, contents.Length / 2);
        Compartment2 = contents.Substring((contents.Length / 2), contents.Length / 2);
        CommonItem = GetCommonCharacter(Compartment1, Compartment2);
        PriorityScore = RucksackReorganization.GetItemPriorityScore(CommonItem);
    }

    private static char GetCommonCharacter(string string1, string string2)
    {
        var charsInBothStrings = RucksackReorganization.CharactersFoundInAllStrings(string1, string2);
        if (charsInBothStrings.Length != 1)
        {
            throw new ArgumentException(
                $"Expected only one common character, but found {charsInBothStrings.Length}: {string.Join("/", charsInBothStrings)}");
        }

        return charsInBothStrings[0];
    }
}
