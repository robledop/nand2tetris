using FluentAssertions;

namespace JackCompiler.Tests;

public class SyntaxAnalyzerTests
{
    [Fact]
    public void ArrayTest()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        parser.ParseClass();
    }

    [Fact]
    public void SquareMain()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Main.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        parser.ParseClass();
    }

    [Fact]
    public void Random()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\GameOfLife\", "Random.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        parser.ParseClass();
    }
}