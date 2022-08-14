namespace JackCompiler.JackAnalyzer
{
    public record Token(string Value, TokenType Type, Marker Marker);
}