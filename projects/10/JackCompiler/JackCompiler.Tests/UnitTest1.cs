using System.Text.RegularExpressions;

namespace JackCompiler.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");
        var source = File.ReadAllText(path);
        source = StripComments(source);
        var sut = new Lexer(source);

        var tokens = new List<TokenType>();
        TokenType? token = null;

        while (token != TokenType.EOF)
        {
            token = sut.GetToken();
            tokens.Add((TokenType) token);
        }
    }

    static string StripComments(string line)
    {
        var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
        return Regex.Replace(line, re, "$1");
    }
}