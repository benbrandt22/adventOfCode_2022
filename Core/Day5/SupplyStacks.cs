using System.Diagnostics;
using System.Text.RegularExpressions;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day5;

public class SupplyStacks : BaseDayModule
{
    public override int Day => 5;  
    public override void Execute()
    {
        Run("Day5/sample.txt", StackCollection.ApplyMoveV1, "Part 1 (single crate) moving logic");
        Run("Day5/input.txt", StackCollection.ApplyMoveV1, "Part 1 (single crate) moving logic");
        
        Run("Day5/sample.txt", StackCollection.ApplyMoveV2, "Part 2 (multiple crate) moving logic");
        Run("Day5/input.txt", StackCollection.ApplyMoveV2, "Part 2 (multiple crate) moving logic");
    }

    private void Run(string filename, Action<StackCollection, CrateMoveInstruction> movingAction, string description)
    {
        WriteLine();
        WriteLine($"Analyzing stack plan from {filename} with {description}...");

        var sections = TextFileLoader
            .LoadLines(filename)
            .SplitOnEmptyStrings()
            .ToArray();

        var startingArrangement = sections[0];
        var stackCollection = new StackCollection(startingArrangement);
        
        var moves = sections[1];
        
        WriteLine($"Loaded starting arrangement of {stackCollection.Stacks.Count} stacks");
        WriteLine($"Applying {moves.Count} move instructions...");

        moves.ForEach(line => movingAction(stackCollection, new CrateMoveInstruction(line)));
        
        WriteLine($"Top of each stack: {stackCollection.TopOfEachStack()}");
    }

    public class StackCollection
    {
        public readonly Dictionary<int, Stack<char>> Stacks;
        
        public StackCollection(List<string> lines)
        {
            // since stack IDs are single digit numbers, get the stack number and it's column position to help parse the other lines
            var stackRefs = lines.Last()
                .Select((c, i) => Tuple.Create(c, i))
                .Where(t => t.Item1 != ' ') // remove spaces
                .Select(t => (StackId: int.Parse(t.Item1.ToString()), Column: t.Item2))
                .ToList();

            // initialize the stacks
            Stacks = new Dictionary<int, Stack<char>>();
            stackRefs.ForEach(x => Stacks[x.StackId] = new Stack<char>());
            
            // build up the stacks
            for (int l = (lines.Count-2); l >= 0; l--)
            {
                stackRefs.ForEach(x =>
                {
                    var crate = lines[l][x.Column];
                    if (crate != ' ')
                    {
                        Stacks[x.StackId].Push(crate);    
                    }
                });
            }
        }
        
        public static void ApplyMoveV1(StackCollection stacks, CrateMoveInstruction move)
        {
            for (int i = 0; i < move.Amount; i++)
            {
                var crate = stacks.Stacks[move.Start].Pop();
                stacks.Stacks[move.End].Push(crate);
            }
        }
        
        public static void ApplyMoveV2(StackCollection stacks, CrateMoveInstruction move)
        {
            var heldInCrane = new Stack<char>();
            for (int i = 0; i < move.Amount; i++)
            {
                var crate = stacks.Stacks[move.Start].Pop();
                heldInCrane.Push(crate);
            }

            for (int i = 0; i < move.Amount; i++)
            {
                var crate = heldInCrane.Pop();
                stacks.Stacks[move.End].Push(crate);
            }
        }

        public string TopOfEachStack()
        {
            var tops = Stacks.Select(x => x.Value.Peek());
            return string.Join("", tops);
        }
    }
    
    public class CrateMoveInstruction
    {
        public readonly int Start;
        public readonly int End;
        public readonly int Amount;
            
        private static Regex _moveRegex = new Regex(@"move (?<amount>\d+) from (?<start>\d+) to (?<end>\d+)");
        public CrateMoveInstruction(string moveLine)
        {
            var match = _moveRegex.Match(moveLine);
            Start = int.Parse(match.Groups["start"].Value);
            End = int.Parse(match.Groups["end"].Value);
            Amount = int.Parse(match.Groups["amount"].Value);
        }
    }

}