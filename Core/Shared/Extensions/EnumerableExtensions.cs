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
}