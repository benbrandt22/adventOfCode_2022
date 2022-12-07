using System.Diagnostics;
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
        var analysisItems = new List<(Regex Format, Type ItemType)>
        {
            (Format: ChangeDirectoryCommand.LineFormat, ItemType: typeof(ChangeDirectoryCommand)),
            (Format: ListCommand.LineFormat, ItemType: typeof(ListCommand)),
            (Format: FileInfoLine.LineFormat, ItemType: typeof(FileInfoLine)),
            (Format: DirectoryInfoLine.LineFormat, ItemType: typeof(DirectoryInfoLine))
        };

        var itemRef = analysisItems.First(x => x.Format.IsMatch(line));

        var regexMatch = itemRef.Format.Match(line);

        var item = (FileSystemAnalysisItem)Activator.CreateInstance(itemRef.ItemType, new[] { regexMatch });
        return item!;
    }

    public abstract class FileSystemAnalysisItem
    {
        public abstract void Apply(DeviceFileSystem fileSystem);
    }
    
    [DebuggerDisplay("Change Directory: {DirName}")]
    public class ChangeDirectoryCommand : FileSystemAnalysisItem
    {
        public readonly string DirName;
        public static Regex LineFormat => new Regex(@"\$ cd (?<dirName>.+)");

        public ChangeDirectoryCommand(Match match) => DirName = match.Groups["dirName"].Value;
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
        public static Regex LineFormat => new Regex(@"\$ ls");
        public ListCommand(Match match) { }
        public override void Apply(DeviceFileSystem fileSystem)
        {
            // do nothing
        }
    }
    
    [DebuggerDisplay("File Info: {FileName} ({Size})")]
    public class FileInfoLine : FileSystemAnalysisItem
    {
        public readonly long Size;
        public readonly string FileName;
        public static Regex LineFormat => new Regex(@"(?<size>\d+) (?<fileName>.+)");

        public FileInfoLine(Match match)
        {
            Size = long.Parse(match.Groups["size"].Value);
            FileName = match.Groups["fileName"].Value;
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
        public static Regex LineFormat => new Regex(@"dir (?<dirName>.+)");

        public DirectoryInfoLine(Match match) => DirName = match.Groups["dirName"].Value;
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