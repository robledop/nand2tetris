using JackCompiler.JackAnalyzer;
using JackCompiler.Tests.Helpers;

namespace JackCompiler.Tests;

public class SyntaxAnalyzerTests
{
    [Fact]
    public void ArrayTest()
    {
        string path = Path.Combine(Environment.CurrentDirectory, "TestSource", "ArrayTest", "Main.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var xml = parser.ParseClass();

        var testFile = Path.Combine(Environment.CurrentDirectory, "TestSource", "ArrayTest", "Main.xml");
        var destination = Path.Combine(Environment.CurrentDirectory, "TestSource", "ArrayTest", "Result", "Main.xml");
        xml.Save(destination);
        FileAssert.AreEqual(testFile, destination);
    }

    [Fact]
    public void SquareMain()
    {
        string path = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "Main.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var xml = parser.ParseClass();

        var testFile = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "Main.xml");
        var destination = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "Result", "Main.xml");
        xml.Save(destination);

        FileAssert.AreEqual(testFile, destination);
    }

    [Fact]
    public void Square()
    {
        string path = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "Square.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var xml = parser.ParseClass();

        var testFile = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "Square.xml");
        var destination = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "Result", "Square.xml");
        xml.Save(destination);

        FileAssert.AreEqual(testFile, destination);
    }

    [Fact]
    public void SquareGame()
    {
        string path = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "SquareGame.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var xml = parser.ParseClass();

        var testFile = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "SquareGame.xml");
        var destination = Path.Combine(Environment.CurrentDirectory, "TestSource", "Square", "Result", "SquareGame.xml");
        xml.Save(destination);

        FileAssert.AreEqual(testFile, destination);
    }

    [Fact]
    public void Random()
    {
        string path = Path.Combine(Environment.CurrentDirectory, "TestSource","GameOfLife", "Random.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);

        var xml = parser.ParseClass();

        var testFile = Path.Combine(Environment.CurrentDirectory, "TestSource", "GameOfLife", "Random.xml");
        var destination = Path.Combine(Environment.CurrentDirectory, "TestSource", "GameOfLife", "Result", "Random.xml");

        xml.Save(destination);

        FileAssert.AreEqual(testFile, destination);
    }
}