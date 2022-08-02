using System.Text;

namespace VMTranslator;

public static class CodeWriter
{
    private static int _eqCount;
    private static int _ltCount;
    private static int _gtCount;

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

    public static string WriteEq()
    {
        var line = $@"// eq
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
D=D-M
M=-1
@EQ.TRUE.{_eqCount}
D;JEQ
@SP
A=M
M=0
(EQ.TRUE.{_eqCount})
@SP
A=M
@SP
M=M+1
";
        _eqCount++;
        return line;
    }

    public static string WriteLt()
    {
        var line = $@"// gt
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
D=D-M
M=-1
@GT.TRUE.{_gtCount}
D;JGT
@SP
A=M
M=0
(GT.TRUE.{_gtCount})
@SP
A=M
@SP
M=M+1
";
        _gtCount++;
        return line;
    }

    public static string WriteGt()
    {
        var line = $@"// lt
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
D=D-M
M=-1
@LT.TRUE.{_ltCount}
D;JLT
@SP
A=M
M=0
(LT.TRUE.{_ltCount})
@SP
A=M
@SP
M=M+1
";
        _ltCount++;
        return line;
    }

    public static string WriteNeg()
    {
        return @"// neg
@SP
A=M-1
M=-M
";
    }

    public static string WriteAnd()
    {
        return @"// and
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
M=D&M
@SP
M=M+1
";
    }

    public static string WriteOr()
    {
        return @"// or
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
M=D|M
@SP
M=M+1
";
    }

    public static string WriteNot()
    {
        return @"// not
@SP
A=M-1
M=!M
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

        sb.AppendLine(@$"// {line}");

        if (segment == "temp")
        {
            sb.AppendLine($"@{5 + command.Arg2}");
        }
        else
        {
            sb.AppendLine($"@{segment}");
        }
        
        if (segment != "temp")
        {
            sb.AppendLine("A=M");
        }

        if (segment is "LCL" or "ARG" or "THIS" or "THAT")
        {
            var offset = command.Arg2;
            for (int i = 0; i < offset; i++)
            {
                sb.AppendLine("A=A+1");  // inefficient, I know
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
        if (segment is "LCL" or "ARG" or "THIS" or "THAT")
        {
            var offset = command.Arg2;
            for (int i = 0; i < offset; i++)
            {
                sb.AppendLine("A=A+1"); // inefficient, I know 
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

    private static string GetSegment(CommandInformation command)
    {
        return command.Arg1 switch
        {
            "local" => "LCL",
            "argument" => "ARG",
            "this" => "THIS",
            "that" => "THAT",
            "temp" => "temp",
            "constant" => "constant",
            "pointer" => "pointer",
            "static" => "static",
            _ => throw new NotImplementedException()
        };
    }
}