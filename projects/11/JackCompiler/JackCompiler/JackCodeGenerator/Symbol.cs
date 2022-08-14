namespace JackCompiler.JackCodeGenerator
{
    public record Symbol(string Name, string Type, SymbolKind Kind, int Position);

    public enum SymbolKind
    {
        Argument,
        Local,
        Field,
        Static
    }
}