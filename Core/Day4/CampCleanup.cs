using System.Diagnostics;
using Core.Shared;
using Core.Shared.Modules;

namespace Core.Day4;

public class CampCleanup : BaseDayModule
{
    public override int Day => 4;  
    public override void Execute()
    {
        Run("Day4/sample.txt");
        Run("Day4/input.txt");
    }

    private void Run(string filename)
    {
        WriteLine("");
        WriteLine($"Analyzing pairings from {filename} ...");

        var pairings = TextFileLoader
            .LoadLines(filename)
            .Select(ToRanges)
            .ToList();
        
        WriteLine($"Loaded {pairings.Count} pairings");

        var pairsThatFullyOverlap = pairings
            .Where(p => p.Item1.FullyContains(p.Item2) || p.Item2.FullyContains(p.Item1))
            .ToList();
        
        WriteLine($"PART 1: Pairings that fully overlap: {pairsThatFullyOverlap.Count}");
        
        var pairsThatOverlap = pairings
            .Where(p => p.Item1.OverlapsWith(p.Item2))
            .ToList();
        
        WriteLine($"PART 2: Pairings that overlap at all: {pairsThatOverlap.Count}");
    }

    private Tuple<Range,Range> ToRanges(string line)
    {
        var ranges = line.Split(",")
            .Select(rangeString =>
            {
                var limits = rangeString.Split("-").Select(int.Parse).ToList();
                return new Range(limits[0], limits[1]);
            })
            .ToList();
        return Tuple.Create(ranges[0], ranges[1]);
    }
}

[DebuggerDisplay("[{Start} - {End}]")]
public class Range
{
    public int Start { get; }
    public int End { get; }
    public Range(int start, int end)
    {
        if (end < start) { throw new ArgumentException("start must not come after end"); }
        Start = start;
        End = end;
    }

    public bool FullyContains(Range other) =>
        other.Start >= Start
        && other.Start <= End
        && other.End >= Start
        && other.End <= End;

    public bool OverlapsWith(Range other) =>
        other.Start <= End && other.End >= Start;
}