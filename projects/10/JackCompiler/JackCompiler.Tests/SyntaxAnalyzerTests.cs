using FluentAssertions;

namespace JackCompiler.Tests;

public class SyntaxAnalyzerTests
{
    [Fact]
    public void ArrayTest()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");
        var source = File.ReadAllText(path);

        var analyzer = new SyntaxAnalyzer();
        analyzer.GetTokens(source);
        analyzer.AnalyzeClass();
    }

    [Fact]
    public void SquareMain()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Main.jack");
        var source = File.ReadAllText(path);

        var analyzer = new SyntaxAnalyzer();
        analyzer.GetTokens(source);
        analyzer.AnalyzeClass();
    }

    [Fact]
    public void Random()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\GameOfLife\", "Random.jack");
        var source = File.ReadAllText(path);

        var analyzer = new SyntaxAnalyzer();
        analyzer.GetTokens(source);
        analyzer.AnalyzeClass();
    }
}