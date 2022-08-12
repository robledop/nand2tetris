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
}