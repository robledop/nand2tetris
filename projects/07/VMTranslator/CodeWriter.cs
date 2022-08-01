using System.Text;

namespace VMTranslator;

public static class CodeWriter
{
    public static string WriteAdd()
    {
        return @"// add
@SP
M=M-1
@SP
A=M
D=M
@SP
M=M-1
@SP
A=M
M=M+D
@SP
M=M+1
";
    }

    public static string WriteSub()
    {
        return @"// sub
@SP
M=M-1
@SP
A=M
D=M
@SP
M=M-1
@SP
A=M
M=M-D
@SP
M=M+1
";
    }

    public static string WritePush(CommandInformation command, string line)
    {
        var sb = new StringBuilder();
        var segment = GetSegment(command);

        if (segment == "constant")
        {
            return WritePushConstant(command, line);
        }

        //sb.AppendLine();
        sb.AppendLine(@$"// {line}");

        sb.AppendLine($"@{segment}");
        sb.AppendLine("A=M");

        if (segment is "LCL" or "ARG" or "THIS" or "THAT")
        {
            var offset = command.Arg2;
            for (int i = 0; i < offset; i++)
            {
                sb.AppendLine("A=A+1");
            }
        }

        sb.AppendLine("D=M");
        sb.AppendLine("@SP");
        sb.AppendLine("A=M");
        sb.AppendLine("M=D");
        sb.AppendLine("@SP");
        sb.AppendLine("M=M+1");

        return sb.ToString();
    }

    public static string WritePop(CommandInformation command, string line)
    {
        var sb = new StringBuilder();

        //sb.AppendLine();
        sb.AppendLine(@$"// {line}");
        var segment = GetSegment(command);

        sb.Append(@"@SP
M=M-1
@SP
A=M
");

        sb.AppendLine($"D=M");
        sb.AppendLine($"@{segment}");
        if (segment is "LCL" or "ARG" or "THIS" or "THAT")
        {
            sb.AppendLine($"A=M");
        }
        //sb.AppendLine(@"A=M");
        if (segment is "LCL" or "ARG" or "THIS" or "THAT")
        {
            var offset = command.Arg2;
            for (int i = 0; i < offset; i++)
            {
                sb.AppendLine("A=A+1");
            }
        }

        sb.AppendLine("M=D");
        return sb.ToString();
    }

    static string WritePushConstant(CommandInformation command, string line)
    {
        var sb = new StringBuilder();
        sb.AppendLine(@$"// {line}");
        sb.AppendLine($"@{command.Arg2}");
        sb.Append(@"D=A
@SP
A=M
M=D
@SP
M=M+1
");
        return sb.ToString();
    }

    private static string? GetSegment(CommandInformation command)
    {
        return command.Arg1 switch
        {
            "local" => "LCL",
            "argument" => "ARG",
            "this" => "THIS",
            "that" => "THAT",
            "temp" => (5 + command.Arg2).ToString(),
            "constant" => "constant",
            _ => throw new NotImplementedException()
        };
    }
}