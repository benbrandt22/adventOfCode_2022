using System.Diagnostics;
using Core.Shared;
using Core.Shared.Modules;
#pragma warning disable CS8509

namespace Core.Day2;

public class RockPaperScissors : BaseDayModule
{
    public override int Day => 2;
    
    public override void Execute()
    {
        PlayRounds("Day2/day2-sample.txt");
        PlayRounds("Day2/day2-input.txt");
    }

    private void PlayRounds(string filename)
    {
        WriteLine("");
        WriteLine($"Playing Rock/Paper/Scissors rounds from {filename} ...");

        var rounds = TextFileLoader
            .LoadLines(filename)
            .Select(ToRoundV2)
            .ToList();
        
        WriteLine($"Loaded {rounds.Count} rounds");
        
        //rounds.ForEach(r => WriteLine($"Opp: {r.Opponent} / You: {r.You} / Your Score: {r.YourScore()}"));
        
        WriteLine($"Your total score: {rounds.Sum(r => r.YourScore())}");
    }

    /// <summary>
    /// generates a round based on part 1 of the challenge, where X/Y/Z was assumed to be your move
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    private Round ToRoundV1(string line)
    {
        var letters = line.Split(' ');
        
        RpcShape Decode(string code) =>
            code switch
            {
                "A" => RpcShape.Rock,
                "B" => RpcShape.Paper,
                "C" => RpcShape.Scissors,
                "X" => RpcShape.Rock,
                "Y" => RpcShape.Paper,
                "Z" => RpcShape.Scissors,
                _ => throw new ArgumentOutOfRangeException($"unsupported code: \"{code}\"")
            };
        
        return new Round(Decode(letters[0]), Decode(letters[1]));
    }
    
    /// <summary>
    /// generates a round based on part 2 of the challenge, where X/Y/Z was clarified as the desired outcome
    /// </summary>
    private Round ToRoundV2(string line)
    {
        var letters = line.Split(' ');
        var opponentMove = letters[0] switch
        {
            "A" => RpcShape.Rock,
            "B" => RpcShape.Paper,
            "C" => RpcShape.Scissors
        };
        
        var yourDesiredOutcome = letters[1] switch
        {
            "X" => RpcOutcome.Lose,
            "Y" => RpcOutcome.Draw,
            "Z" => RpcOutcome.Win
        };

        var yourMove = ShapeNeededForOutcome(opponentMove, yourDesiredOutcome);
        
        return new Round(opponentMove, yourMove);
    }

    private RpcShape ShapeNeededForOutcome(RpcShape opponentMove, RpcOutcome yourDesiredOutcome) =>
        opponentMove switch
        {
            RpcShape.Rock => yourDesiredOutcome switch
            {
                RpcOutcome.Win => RpcShape.Paper,
                RpcOutcome.Lose => RpcShape.Scissors,
                RpcOutcome.Draw => RpcShape.Rock,
                _ => throw new ArgumentOutOfRangeException(nameof(yourDesiredOutcome))
            },
            RpcShape.Paper => yourDesiredOutcome switch
            {
                RpcOutcome.Win => RpcShape.Scissors,
                RpcOutcome.Lose => RpcShape.Rock,
                RpcOutcome.Draw => RpcShape.Paper,
                _ => throw new ArgumentOutOfRangeException(nameof(yourDesiredOutcome))
            },
            RpcShape.Scissors => yourDesiredOutcome switch
            {
                RpcOutcome.Win => RpcShape.Rock,
                RpcOutcome.Lose => RpcShape.Paper,
                RpcOutcome.Draw => RpcShape.Scissors,
                _ => throw new ArgumentOutOfRangeException(nameof(yourDesiredOutcome))
            },
            _ => throw new ArgumentOutOfRangeException(nameof(opponentMove))
        };
}

public enum RpcShape
{
    Rock,
    Paper,
    Scissors
}

public enum RpcOutcome
{
    Win,
    Lose,
    Draw
}

[DebuggerDisplay("Opponent: {Opponent} / You: {You}")]
public record Round(RpcShape Opponent, RpcShape You)
{
    // This could be better or more flexible, with each player getting an outcome, but it does enough to satisfy the puzzle  :-)  
    
    public int YourScore() => ShapeScore() + WinLoseDrawScore();

    private int ShapeScore()
    {
        return You switch
        {
            RpcShape.Rock => 1,
            RpcShape.Paper => 2,
            RpcShape.Scissors => 3,
            _ => throw new ArgumentOutOfRangeException()
        };

    }
    
    private int WinLoseDrawScore()
    {
        var youWin = 6;
        var draw = 3;
        var youLose = 0;

        return You switch
        {
            RpcShape.Rock => Opponent switch
            {
                RpcShape.Rock => draw,
                RpcShape.Paper => youLose,
                RpcShape.Scissors => youWin,
                _ => throw new ArgumentOutOfRangeException()
            },
            RpcShape.Paper => Opponent switch
            {
                RpcShape.Rock => youWin,
                RpcShape.Paper => draw,
                RpcShape.Scissors => youLose,
                _ => throw new ArgumentOutOfRangeException()
            },
            RpcShape.Scissors => Opponent switch
            {
                RpcShape.Rock => youLose,
                RpcShape.Paper => youWin,
                RpcShape.Scissors => draw,
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
