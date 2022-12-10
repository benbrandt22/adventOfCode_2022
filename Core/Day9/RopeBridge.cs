using System.Diagnostics;
using System.Text.RegularExpressions;
using Core.Shared;
using Core.Shared.Modules;

namespace Core.Day9;

public class RopeBridge : BaseDayModule
{
    public override int Day => 9;

    public override void Execute()
    {
        Run("Day9/sample.txt");
        Run("Day9/part2sample.txt");
        Run("Day9/input.txt");
    }

    private void Run(string filename)
    {
        WriteLine();
        WriteLine($"Analyzing {filename} ...");

        var moves = TextFileLoader
            .LoadLines(filename)
            .Select(Move.FromLine)
            .ToList();
        
        WriteLine($"Loaded {moves.Count} moves"); 
        WriteLine();

        WriteLine("Part 1:");
        AnalyzeRopeTail(2, moves);
        WriteLine();

        WriteLine("Part 2:");
        AnalyzeRopeTail(10, moves);
        WriteLine();
        
        WriteHorizontalRule();
    }
    
    public void AnalyzeRopeTail(int knots, List<Move> moves)
    {
        var rope = new Rope(knots);
        moves.ForEach(m => rope.MoveHead(m));
        WriteLine($"{knots}-Knot Rope: tail visited {rope.Tail.AllLocationsVisited.Distinct().Count()} distinct locations");
    }

    public enum Direction { Up, Down, Left, Right }

    public static Direction ToDirection(string directionString) =>
        directionString.ToUpper() switch
        {
            "U" => Direction.Up,
            "D" => Direction.Down,
            "L" => Direction.Left,
            "R" => Direction.Right,
            _ => throw new ArgumentException("Unsupported direction")
        };

    [DebuggerDisplay("{Direction} {Distance}")]
    public record Move(Direction Direction, int Distance)
    {
        private static Regex _linePattern = new Regex(@"(?<direction>\S) (?<distance>\d+)");
        public static Move FromLine(string inputLine)
        {
            var match = _linePattern.Match(inputLine);
            return new Move(ToDirection(match.Groups["direction"].Value), int.Parse(match.Groups["distance"].Value));
        }
    }

    public class RopeKnot
    {
        public string Name { get; }
        public List<RopeCoordinate> AllLocationsVisited = new();
        public RopeCoordinate CurrentLocation;
        public event EventHandler<OnLeadingKnotMovedEventArgs> KnotMoved;

        public RopeKnot(string name, int startX, int startY)
        {
            Name = name;
            SetLocation(startX, startY);
        }

        public void SetLocation(int x, int y)
        {
            Debug($"{Name} to ({x},{y})");
            CurrentLocation = new RopeCoordinate(x, y);
            AllLocationsVisited.Add(new RopeCoordinate(x, y));
            // tell the downstream knot that I just moved
            KnotMoved?.Invoke(this, new OnLeadingKnotMovedEventArgs(new RopeCoordinate(x, y)));
        }

        public class OnLeadingKnotMovedEventArgs : EventArgs
        {
            public OnLeadingKnotMovedEventArgs(RopeCoordinate leadingKnot)
            {
                LeadingKnot = leadingKnot;
            }

            public RopeCoordinate LeadingKnot { get; set; }
        }
        
        public void OnLeadingKnotMoved(object sender, OnLeadingKnotMovedEventArgs e)
        {
            
            if (e.LeadingKnot.IsTouching(CurrentLocation)) { return; }

            // (assumes the leading knot only moved 1 space)
            
            var newTailX = CurrentLocation.X; 
            var newTailY = CurrentLocation.Y;
            
            if (CurrentLocation.X < e.LeadingKnot.X)
            {
                newTailX = CurrentLocation.X + 1;
            } else if (CurrentLocation.X > e.LeadingKnot.X)
            {
                newTailX = CurrentLocation.X - 1;
            }
            
            if (CurrentLocation.Y < e.LeadingKnot.Y)
            {
                newTailY = CurrentLocation.Y + 1;
            } else if (CurrentLocation.Y > e.LeadingKnot.Y)
            {
                newTailY = CurrentLocation.Y - 1;
            }
            
            SetLocation(newTailX, newTailY);
        }
        
    }

    public record RopeCoordinate(int X, int Y)
    {
        /// <summary>
        /// Returns true if points are the same or adjacent horizontally/vertically/diagonally 
        /// </summary>
        public bool IsTouching(RopeCoordinate other)
        {
            return (Math.Abs(X - other.X) <= 1) && (Math.Abs(Y - other.Y) <= 1);
        }
    }

    public class Rope
    {
        private List<RopeKnot> _knots = new();
        
        public Rope(int knots)
        {
            if (knots < 2) { throw new ArgumentException("rope must have at least 2 knots"); }
            // create each knot
            for (int i = 0; i < knots; i++)
            {
                _knots.Add(new RopeKnot(i.ToString(), 0, 0));
            }
            // wire up events so leading knots can notify trailing knots when they move
            for (int i = 0; i < (knots-1); i++)
            {
                _knots[i].KnotMoved += _knots[i + 1].OnLeadingKnotMoved;
            }
        }

        public RopeKnot Head => _knots.First();
        public RopeKnot Tail => _knots.Last();

        public void MoveHead(Move move)
        {
            Debug($"[ Move Head {move.Direction} {move.Distance} ]");
            var dx = 0;
            var dy = 0;
            switch (move.Direction)
            {
                case Direction.Up:
                    dx = 0;
                    dy = 1;
                    break;
                case Direction.Down:
                    dx = 0;
                    dy = -1;
                    break;
                case Direction.Left:
                    dx = -1;
                    dy = 0;
                    break;
                case Direction.Right:
                    dx = 1;
                    dy = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            for (int i = 0; i < move.Distance; i++)
            {
                MoveHead(dx, dy);
            }
        }
        
        private void MoveHead(int dx, int dy)
        {
            Head.SetLocation(Head.CurrentLocation.X + dx, Head.CurrentLocation.Y + dy);
        }

    }
}