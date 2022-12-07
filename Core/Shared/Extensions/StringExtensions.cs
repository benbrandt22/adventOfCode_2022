namespace Core.Shared.Extensions;

public static class StringExtensions
{
    public static string IndentAllLines(this string input, int spaces)
    {
        var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var prepend = new string(' ', spaces);
        return string.Join(Environment.NewLine, lines.Select(line => $"{prepend}{line}"));
    }
    
    public static string RemoveEmptyLines(this string input)
    {
        var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(Environment.NewLine, lines);
    }
}