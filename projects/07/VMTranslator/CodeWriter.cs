using System.Text;
using System;

namespace VMTranslator
{
    public static class CodeWriter
    {
        private static int _eqCount;
        private static int _ltCount;
        private static int _gtCount;

        public static string WriteAdd()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// add");

            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=M+D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");

            return sb.ToString();
        }

        public static string WriteSub()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// sub");

            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=M-D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");
            
           return sb.ToString();
        }

        public static string WriteEq()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// eq");

            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=D-M");
            sb.AppendLine("M=-1");
            sb.AppendLine($"@EQ.TRUE.{_eqCount}");
            sb.AppendLine("D;JEQ");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=0");
            sb.AppendLine($"(EQ.TRUE.{_eqCount})");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");

            _eqCount++;
            return sb.ToString();
        }

        public static string WriteLt()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// lt");

            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M-D");
            sb.AppendLine("M=-1");
            sb.AppendLine($"@LT.TRUE.{_gtCount}");
            sb.AppendLine("D;JLT");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=0");
            sb.AppendLine($"(LT.TRUE.{_gtCount})");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");

            _gtCount++;
            return sb.ToString();
        }

        public static string WriteGt()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// gt");

            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M-D");
            sb.AppendLine("M=-1");
            sb.AppendLine($"@GT.TRUE.{_ltCount}");
            sb.AppendLine("D;JGT");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=0");
            sb.AppendLine($"(GT.TRUE.{_ltCount})");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");

            _ltCount++;

            return sb.ToString();
        }

        public static string WriteNeg()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// neg");

            sb.AppendLine("@SP");
            sb.AppendLine("A=M-1");
            sb.AppendLine("M=-M");

            return sb.ToString();
        }

        public static string WriteAnd()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// and");

            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D&M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");

            return sb.ToString();
        }

        public static string WriteOr()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// or");
            sb.AppendLine("@SP");

            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D|M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");

            return sb.ToString();
        }

        public static string WriteNot()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// not");

            sb.AppendLine("@SP");
            sb.AppendLine("A=M-1");
            sb.AppendLine("M=!M");

            return sb.ToString();
        }

        public static string WritePush(CommandInformation command, string line, string fileName)
        {
            var sb = new StringBuilder();
            var segment = GetSegment(command);

            switch (segment)
            {
                case "static":
                    return WritePushStatic(command, line, fileName);
                case "constant":
                    return WritePushConstant(command, line);
                case "pointer":
                    return WritePushPointer(command, line);
            }

            sb.AppendLine($"// {line}");

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
                    sb.AppendLine("A=A+1"); // inefficient, I know
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

        public static string WritePop(CommandInformation command, string line, string fileName)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"// {line}");
            var segment = GetSegment(command);

            switch (segment)
            {
                case "static":
                    return WritePopStatic(command, line, fileName);
                case "pointer":
                    return WritePopPointer(command, line);
            }

            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");

            sb.AppendLine("D=M");
            if (segment == "temp")
            {
                sb.AppendLine($"@{5 + command.Arg2}");
            }
            else
            {
                sb.AppendLine($"@{segment}");
            }
            if (segment is "LCL" or "ARG" or "THIS" or "THAT")
            {
                sb.AppendLine("A=M");
                var offset = command.Arg2;
                for (int i = 0; i < offset; i++)
                {
                    sb.AppendLine("A=A+1"); // inefficient, I know 
                }
            }

            sb.AppendLine("M=D");
            return sb.ToString();
        }

        static string WritePopStatic(CommandInformation command, string line, string fileName)
        {
            var className = fileName.Split('.')[0];
            var sb = new StringBuilder();

            sb.AppendLine($"// {line}");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine($"@{className}.{command.Arg2}");
            sb.AppendLine("M=D");

            return sb.ToString();
        }

        static string WritePushStatic(CommandInformation command, string line, string fileName)
        {
            var className = fileName.Split('.')[0];
            var sb = new StringBuilder();

            sb.AppendLine($"// {line}");
            sb.AppendLine($"@{className}.{command.Arg2}");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");

            return sb.ToString();
        }

        static string WritePushConstant(CommandInformation command, string line)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"// {line}");
            sb.AppendLine($"@{command.Arg2}");
            sb.AppendLine("D=A");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");
            return sb.ToString();
        }

        static string WritePopPointer(CommandInformation command, string line)
        {
            string thisOrThat = command.Arg2 == 0 ? "THIS" : "THAT";
            var sb = new StringBuilder();
            sb.AppendLine($"// {line}");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine($"@{thisOrThat}");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");

            return sb.ToString();
        }

        static string WritePushPointer(CommandInformation command, string line)
        {
            string thisOrThat = command.Arg2 == 0 ? "THIS" : "THAT";
            var sb = new StringBuilder();
            sb.AppendLine($"// {line}");
            sb.AppendLine($"@{thisOrThat}");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1");

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
}