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
            "eq" => CommandType.Eq,
            "lt" => CommandType.Lt,
            "gt" => CommandType.Gt,
            "neg" => CommandType.Neg,
            "and" => CommandType.And,
            "or" => CommandType.Or,
            "not" => CommandType.Not,
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
    Lt,
    Gt,
    Eq,
    Neg,
    And,
    Or,
    Not,
    Label,
    Goto,
    If,
    Function,
    Return,
    Call
}