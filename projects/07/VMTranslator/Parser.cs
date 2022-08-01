namespace VMTranslator;

public static class Parser
{
    public static CommandInformation Parse(string line)
    {
        var parts = line.Split(' ');
        var command = parts[0];

        var type = command switch
        {
            "push" => CommandType.Push,
            "pop" => CommandType.Pop,
            "add" => CommandType.Add,
            "sub" => CommandType.Sub,
            _ => throw new NotSupportedException()
        };

        string? arg1 = null;
        int? arg2 = null;
        if (type is CommandType.Pop or CommandType.Push)
        {
            arg1 = parts[1];
            arg2 = int.Parse(parts[2]);
        }

        return new CommandInformation(type, arg1, arg2);
    }
}

public record CommandInformation(CommandType Type, string? Arg1, int? Arg2);

public enum CommandType
{
    Sub,
    Add,
    Push,
    Pop,
    Label,
    Goto,
    If,
    Function,
    Return,
    Call
}