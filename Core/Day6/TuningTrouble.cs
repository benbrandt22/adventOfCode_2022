using Core.Shared;
using Core.Shared.Modules;

namespace Core.Day6;

public class TuningTrouble : BaseDayModule
{
    public override int Day => 6;  
    public override void Execute()
    {
        Run("Day6/sample.txt");
        Run("Day6/input.txt");
    }

    private void Run(string filename)
    {
        WriteLine();
        WriteHorizontalRule();
        WriteLine($"Analyzing signals from {filename}...");

        var signals = TextFileLoader
            .LoadLines(filename)
            .ToList();

        signals.ForEach(s =>
        {
            AnalyzeSignal(s, "start-of-packet", 4);
            AnalyzeSignal(s, "start-of-message", 14);
        });
        
    }

    private void AnalyzeSignal(string signal, string markerType, int markerLength)
    {
        WriteLine();
        WriteLine(signal);
        for (int i = (markerLength - 1); i < signal.Length; i++)
        {
            var possibleMarker = signal.Substring((i - (markerLength - 1)), markerLength);
            if (IsAllUnique(possibleMarker))
            {
                var lastCharacterIndex = i; // (zero-based)
                var lastCharacterNumber = lastCharacterIndex + 1; // (one-based)
                WriteLine($" First {markerType} marker was \"{possibleMarker}\" after character {lastCharacterNumber}");
                return;
            }
        }
        throw new Exception("No marker found");
    }

    private bool IsAllUnique(string text)
    {
        return text.Distinct().ToList().Count == text.Length;
    }
}