using System.Diagnostics;
using Core.Shared;
using Core.Shared.Modules;

namespace Core.Day8;

public class TreetopTreeHouse : BaseDayModule
{
    public override int Day => 8;  
    public override void Execute()
    {
        Run("Day8/sample.txt");
        Run("Day8/input.txt");
    }

    private void Run(string filename)
    {
        WriteLine();
        WriteLine($"Analyzing forest from {filename} ...");

        var forest = new Forest(TextFileLoader.LoadLines(filename).ToList());

        WriteLine($"Loaded forest of size {forest.Width} x {forest.Height}");
        
        WriteLine();
        WriteLine($"Part 1: Total trees visible from outside: {forest.TotalTreesVisibleFromOutside()}");
        
        WriteLine();
        WriteLine($"Part 2: Maximum \"Scenic Score\" in the forest: {forest.MaxScenicScore()}");
        
        WriteLine();
        WriteHorizontalRule();
    }

    [DebuggerDisplay("{Width} x {Height}")]
    public class Forest
    {
        private int[,] _trees;
        
        public Forest(List<string> treeRows)
        {
            var totalRows = treeRows.Count;
            var totalColumns = treeRows.Select(row => row.Length).Distinct().Single(); // (throws error if not all rows are same length)
            _trees = new int[totalColumns, totalRows];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _trees[x, y] = int.Parse(treeRows[y][x].ToString());
                }
            }
        }

        public int Width => _trees.GetLength(0);
        public int Height => _trees.GetLength(1);

        /// <summary>
        /// Returns a list of tree heights starting at the given coordinate and moving out in the given direction
        /// </summary>
        private IEnumerable<int> GetTreeline(int startX, int startY, Direction direction)
        {
            int x = startX;
            int y = startY;
            while (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                yield return _trees[x, y];
                var move = DirectionTransform(direction);
                x = (x + move.dx);
                y = (y + move.dy);
            }
        }

        public List<List<int>> GreeTreelinesFromTree(int x, int y) =>
            new List<List<int>>()
            {
                GetTreeline(x, y, Direction.North).ToList(),
                GetTreeline(x, y, Direction.East).ToList(),
                GetTreeline(x, y, Direction.South).ToList(),
                GetTreeline(x, y, Direction.West).ToList(),
            };

        public int TotalTreesVisibleFromOutside()
        {
            int visibleCount = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (IsTreeVisibleFromOutside(x, y))
                    {
                        visibleCount++;
                    }
                }
            }

            return visibleCount;
        }

        private bool IsTreeVisibleFromOutside(int x, int y)
        {
            var treeLines = GreeTreelinesFromTree(x, y);

            foreach (var treeLine in treeLines)
            {
                if (treeLine.Count == 1) { return true; }

                var firstTreeHeight = treeLine[0];
                if (treeLine.Skip(1).All(t => t < firstTreeHeight))
                {
                    return true;
                }
            }
            
            // checked all four directions and tree was no9t found to be visible
            return false;
        }

        private int ScenicScore(int x, int y)
        {
            var treeLines = GreeTreelinesFromTree(x, y);

            var directionScores = treeLines.Select(treeline =>
            {
                var startingTreeHeight = treeline[0];
                var potentialTrees = treeline.Skip(1).ToList();

                var score = 0;
                foreach (var potentialTree in potentialTrees)
                {
                    if (potentialTree < startingTreeHeight)
                    {
                        score++;
                    }

                    if (potentialTree >= startingTreeHeight)
                    {
                        score++;
                        break;
                    }
                }

                return score;
            }).ToList();

            var scenicScore = directionScores.Aggregate(1, (a, b) => a * b);
            return scenicScore;
        }

        public int MaxScenicScore()
        {
            var maxScenicScore = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var treeScenicScore = ScenicScore(x, y);
                    maxScenicScore = Math.Max(maxScenicScore, treeScenicScore);
                }
            }

            return maxScenicScore;
        }

    }
    
    public enum Direction{ North, South, East, West }

    /// <summary>
    /// returns an x/y delta to describe how to move in the given direction
    /// </summary>
    public static (int dx, int dy) DirectionTransform(Direction direction) =>
        direction switch
        {
            Direction.North => (dx: 0, dy: -1),
            Direction.South => (dx: 0, dy: 1),
            Direction.East => (dx: 1, dy: 0),
            Direction.West => (dx: -1, dy: 0),
        };
}
