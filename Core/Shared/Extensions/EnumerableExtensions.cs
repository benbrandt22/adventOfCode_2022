namespace Core.Shared.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<List<string>> SplitOnEmptyStrings(this IEnumerable<string> lines)
    {
        var currentList = new List<string>();
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                currentList.Add(line);
            }
            else
            {
                yield return currentList;
                currentList = new List<string>();
            }
        }

        if (currentList.Count > 0)
        {
            yield return currentList;
        }
    }

    /// <summary>
    /// Recursively returns the starting item and all children (recursively) based on the child selector
    /// </summary>
    public static IEnumerable<T> SelectRecursively<T>(this T item, Func<T, IEnumerable<T>?> childrenSelector)
    {
        yield return item;
        var children = childrenSelector(item);
        if (children == null) yield break;
        foreach (var subItem in children.SelectMany(f => f.SelectRecursively(childrenSelector)))
        {
            yield return subItem;
        }
    }
}