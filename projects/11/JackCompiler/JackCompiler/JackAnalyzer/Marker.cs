namespace JackCompiler.JackAnalyzer;

public struct Marker(int pointer, int line, int column)
{
    public int Pointer { get; set; } = pointer;
    public int Line { get; set; } = line;
    public int Column { get; set; } = column;
}