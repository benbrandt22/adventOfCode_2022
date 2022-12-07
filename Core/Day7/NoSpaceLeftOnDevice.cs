using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day7;

public class NoSpaceLeftOnDevice : BaseDayModule
{
    public override int Day => 7;  
    public override void Execute()
    {
        Run("Day7/sample.txt");
        Run("Day7/input.txt");
    }

    private void Run(string filename)
    {
        WriteLine("");
        WriteLine($"Loading filesystem from input {filename}...");
        WriteLine("");

        var fileSystemAnalysisItems = TextFileLoader
            .LoadLines(filename)
            .Select(ToAnalysisItem)
            .ToList();

        var fileSystem = new DeviceFileSystem();
        fileSystemAnalysisItems.ForEach(x => x.Apply(fileSystem));
        
        WriteLine(fileSystem.RootFolder.DisplayContents());
        WriteLine("");
        
        WriteLine($"Filesystem loaded, root folder size is: {fileSystem.RootFolder.Size}");

        var allFolders = fileSystem.RootFolder
            .SelectRecursively(folder => folder.Items.OfType<DeviceFolder>())
            .ToList();
        
        WriteLine("");
        long partOneMaxFolderSize = 100000;
        WriteLine($"Part 1: find the total size of all folders with a size of at most {partOneMaxFolderSize}");
        var foldersUpToMaxSize = allFolders.Where(f => f.Size <= partOneMaxFolderSize).ToList();
        
        WriteLine($"Found {foldersUpToMaxSize.Count} matching folders. Total size: {foldersUpToMaxSize.Sum(f => f.Size)}");
        
        // Part 2:
        WriteLine("");
        long maxFileSystemSize = 70000000;
        long minimumNeededUnusedSpace = 30000000;
        WriteLine("Part 2: find the ideal folder to delete...");
        WriteLine($"File system currently occupies {fileSystem.RootFolder.Size} out of the max {maxFileSystemSize}");
        var currentUnusedSpace = (maxFileSystemSize - fileSystem.RootFolder.Size);
        WriteLine($"Current Unused Space = {currentUnusedSpace}");
        var spaceWeNeedToFreeUp = minimumNeededUnusedSpace - currentUnusedSpace;
        WriteLine($"Need to free up at least: {spaceWeNeedToFreeUp}");

        var candidateFolderForDeletion = allFolders
            .Where(f => f.Size >= spaceWeNeedToFreeUp)
            .OrderBy(f => f.Size)
            .First();
        
        WriteLine($"Folder to delete is \"{candidateFolderForDeletion.Name}\" at a size of: {candidateFolderForDeletion.Size}");
        
        WriteLine("");
        WriteLine(new string('-',80));
    }

    private FileSystemAnalysisItem ToAnalysisItem(string line)
    {
        var consoleLineParsers = new List<(Regex Format, Func<Match, FileSystemAnalysisItem> Factory)>()
        {
            (Format: new Regex(@"\$ cd (?<dirName>.+)"),
                Factory: match => new ChangeDirectoryCommand(match.Groups["dirName"].Value)),
            (Format: new Regex(@"\$ ls"),
                Factory: match => new ListCommand()),
            (Format: new Regex(@"(?<size>\d+) (?<fileName>.+)"),
                Factory: match => new FileInfoLine(long.Parse((string)match.Groups["size"].Value), match.Groups["fileName"].Value)),
            (Format: new Regex(@"dir (?<dirName>.+)"),
                Factory: match => new DirectoryInfoLine(match.Groups["dirName"].Value))
        };

        var parser = consoleLineParsers.First(x => x.Format.IsMatch(line));
        return parser.Factory(parser.Format.Match(line));
    }

    public abstract class FileSystemAnalysisItem
    {
        public abstract void Apply(DeviceFileSystem fileSystem);
    }
    
    [DebuggerDisplay("Change Directory: {DirName}")]
    public class ChangeDirectoryCommand : FileSystemAnalysisItem
    {
        public readonly string DirName;
        public ChangeDirectoryCommand(string dirName) => DirName = dirName;
        public override void Apply(DeviceFileSystem fileSystem)
        {
            if (DirName == "/")
            {
                fileSystem.CurrentFolder = fileSystem.RootFolder;
            } else if (DirName == "..")
            {
                fileSystem.CurrentFolder = fileSystem.CurrentFolder.Parent!;
            }
            else
            {
                fileSystem.CurrentFolder = fileSystem.CurrentFolder.Items
                    .OfType<DeviceFolder>()
                    .First(f => f.Name == DirName);
            }
        }
    }
    
    [DebuggerDisplay("List contents")]
    public class ListCommand : FileSystemAnalysisItem
    {
        public override void Apply(DeviceFileSystem fileSystem)
        {
            // this does not help us build the file system; do nothing
        }
    }
    
    [DebuggerDisplay("File Info: {FileName} ({Size})")]
    public class FileInfoLine : FileSystemAnalysisItem
    {
        public readonly long Size;
        public readonly string FileName;
        public FileInfoLine(long size, string fileName)
        {
            Size = size;
            FileName = fileName;
        }

        public override void Apply(DeviceFileSystem fileSystem)
        {
            fileSystem.CurrentFolder.Items.Add(new DeviceFile(FileName, Size));
        }
    }
    
    [DebuggerDisplay("Directory: {DirName}")]
    public class DirectoryInfoLine : FileSystemAnalysisItem
    {
        public readonly string DirName;
        public DirectoryInfoLine(string dirName) => DirName = dirName;
        public override void Apply(DeviceFileSystem fileSystem)
        {
            fileSystem.CurrentFolder.Items.Add(new DeviceFolder(DirName, fileSystem.CurrentFolder));
        }
    }

    public interface IDeviceFileSystemItem
    {
        public string Name { get; }
        public long Size { get; }
    }

    public class DeviceFileSystem
    {
        public readonly DeviceFolder RootFolder = new DeviceFolder("/", null);

        public DeviceFolder CurrentFolder;

        public DeviceFileSystem()
        {
            CurrentFolder = RootFolder;
        }
    }
    
    [DebuggerDisplay("{Name} (dir)")]
    public class DeviceFolder : IDeviceFileSystemItem
    {
        public DeviceFolder(string name, DeviceFolder? parent)
        {
            Name = name;
            Parent = parent;
        }

        public string Name { get; }
        public DeviceFolder? Parent { get; }
        public List<IDeviceFileSystemItem> Items = new();
        public long Size => Items.Sum(x => x.Size);

        /// <summary>
        /// Returns a text preview of the folder hierarchy with all files & folders and their sizes
        /// </summary>
        public string DisplayContents(int indent = 0)
        {
            var output = new StringBuilder();
            output.AppendLine($"{new string(' ', indent)}- {this.Name} (dir, size={this.Size})");
            Items.ForEach(x =>
            {
                var line = x switch {
                    DeviceFile file => $"{new string(' ', indent+2)}- {file.Name} (file, size={file.Size})",
                    DeviceFolder folder => folder.DisplayContents(indent+2),
                    _ => throw new ArgumentOutOfRangeException(x.GetType().Name)
                };
                output.AppendLine(line);
            });
            return output.ToString().RemoveEmptyLines();
        }
    }
    
    [DebuggerDisplay("{Name} (file, size={Size})")]
    public class DeviceFile : IDeviceFileSystemItem
    {
        public DeviceFile(string name, long size)
        {
            Name = name;
            Size = size;
        }

        public string Name { get; }
        public long Size { get; }
    }

}