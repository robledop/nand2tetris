using FluentAssertions;

namespace JackCompiler.Tests;

public class LexerTests
{
    [Fact]
    public void ArrayTest()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");
        var source = File.ReadAllText(path);
        var sut = new Lexer(source);

        var tokens = new List<Token>();
        Token? token = null;

        while (token?.Type != TokenType.EOF)
        {
            token = sut.GetToken();
            tokens.Add(token);
        }

        tokens.Count.Should().Be(141);
        
    }

    [Fact]
    public void SquareGame()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "SquareGame.jack");
        var source = File.ReadAllText(path);
        var sut = new Lexer(source);

        var tokens = new List<Token>();
        Token? token = null;

        while (token?.Type != TokenType.EOF)
        {
            token = sut.GetToken();
            tokens.Add(token);
        }

        tokens.Count.Should().Be(314);
    }

}