using System.Text;
using System;

namespace VMTranslator
{
    public static class CodeWriter
    {
        private static int _eqCount;
        private static int _ltCount;
        private static int _gtCount;
        private static int _frameCount;
        private static int _retCount;
        private static int _retAddrCount;

        public static string WriteInit()
        {
            var sb = new StringBuilder();
            sb.Append(@"@256
D=A
@SP
M=D         // bootstrap
");
            sb.Append(WriteCall(new CommandInformation(CommandType.Call, "Sys.init", 0), "call Sys.init 0"));
            return sb.ToString();
        }

        public static string WriteAdd()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@SP      // line = add");
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
            sb.Append("M=M+1");

            return sb.ToString();
        }

        public static string WriteSub()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@SP      // line = sub");
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
            sb.Append("M=M+1");
            
           return sb.ToString();
        }

        public static string WriteEq()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@SP      // line = eq");
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
            sb.Append("M=M+1");

            _eqCount++;
            return sb.ToString();
        }

        public static string WriteLt()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@SP      // line = lt");
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
            sb.Append("M=M+1");

            _gtCount++;
            return sb.ToString();
        }

        public static string WriteGt()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@SP      // line = gt");
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
            sb.Append("M=M+1");

            _ltCount++;

            return sb.ToString();
        }

        public static string WriteNeg()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@SP      // line = neg");
            sb.AppendLine("A=M-1");
            sb.Append("M=-M");

            return sb.ToString();
        }

        public static string WriteAnd()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@SP      // line = and");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D&M");
            sb.AppendLine("@SP");
            sb.Append("M=M+1");

            return sb.ToString();
        }

        public static string WriteOr()
        {
            var sb = new StringBuilder();
            sb.AppendLine("@SP      // line = or");

            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D|M");
            sb.AppendLine("@SP");
            sb.Append("M=M+1");

            return sb.ToString();
        }

        public static string WriteNot()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@SP      // line = not");
            sb.AppendLine("A=M-1");
            sb.Append("M=!M");

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

            if (segment == "temp")
            {
                sb.AppendLine($"@{5 + command.Arg2}     // line = {line}");
            }
            else
            {
                sb.AppendLine($"@{segment}      // line = {line}");
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
            sb.Append("M=M+1");

            return sb.ToString();
        }

        public static string WritePop(CommandInformation command, string line, string fileName)
        {
            var sb = new StringBuilder();

            var segment = GetSegment(command);

            switch (segment)
            {
                case "static":
                    return WritePopStatic(command, line, fileName);
                case "pointer":
                    return WritePopPointer(command, line);
            }

            sb.AppendLine($"@SP     // line = {line}");
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

            sb.Append("M=D");
            return sb.ToString();
        }

        public static string WriteLabel(CommandInformation command, string line, string currentFunction)
        {
            var sb = new StringBuilder();

            sb.Append($"({currentFunction}${command.Arg1})        // line = {line}");

            return sb.ToString();
        }

        public static string WriteIfGoto(CommandInformation command, string line, string currentFunction)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"@SP     // line = {line}");
            sb.AppendLine("AM=M-1");
            sb.AppendLine("D=M");
            sb.AppendLine($"@{currentFunction}${command.Arg1}");
            sb.Append("D;JNE");

            return sb.ToString();
        }

        public static string WriteGoto(CommandInformation command, string line, string currentFunction)
        {

            var sb = new StringBuilder();

            sb.AppendLine($"@{currentFunction}${command.Arg1}     // line = {line}");
            sb.Append("0;JMP");

            return sb.ToString();
        }

        public static string WriteFunction(CommandInformation command, string line)
        {
            var sb = new StringBuilder();

            sb.Append($"({command.Arg1})        // line = {line}");
            for (int i = 0; i < command.Arg2; i++)
            {
                sb.Append(@"
@SP
A=M
M=0
@SP
M=M+1");
            }

            return sb.ToString();
        }

        public static string WriteReturn(string line, string currentFunction)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"@LCL        // line = {line}");
            sb.AppendLine("D=M");
            sb.AppendLine($"@{currentFunction}$FRAME.{_frameCount}");
            sb.AppendLine("M=D // FRAME = LCL");

            sb.AppendLine("@5");
            sb.AppendLine("D=A");
            sb.AppendLine($"@{currentFunction}$FRAME.{_frameCount}");
            sb.AppendLine("D=M-D");
            sb.AppendLine("A=D");
            sb.AppendLine("D=M");
            sb.AppendLine($"@{currentFunction}$RET.{_retCount}");
            sb.AppendLine("M=D      // RET = *(FRAME - 5)");

            sb.AppendLine("@SP");
            sb.AppendLine("M=M-1");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine("@ARG");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D      // *ARG = pop()");

            sb.AppendLine("@ARG");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("M=D+1    // SP = ARG + 1");

            sb.AppendLine("@1");
            sb.AppendLine("D=A");
            sb.AppendLine($"@{currentFunction}$FRAME.{_frameCount}");
            sb.AppendLine("D=M-D");
            sb.AppendLine("A=D");
            sb.AppendLine("D=M");
            sb.AppendLine("@THAT");
            sb.AppendLine("M=D      // THAT = *(FRAME-1)");

            sb.AppendLine("@2");
            sb.AppendLine("D=A");
            sb.AppendLine($"@{currentFunction}$FRAME.{_frameCount}");
            sb.AppendLine("D=M-D");
            sb.AppendLine("A=D");
            sb.AppendLine("D=M");
            sb.AppendLine("@THIS");
            sb.AppendLine("M=D      // THIS = *(FRAME-2)");

            sb.AppendLine("@3");
            sb.AppendLine("D=A");
            sb.AppendLine($"@{currentFunction}$FRAME.{_frameCount}");
            sb.AppendLine("D=M-D");
            sb.AppendLine("A=D");
            sb.AppendLine("D=M");
            sb.AppendLine("@ARG");
            sb.AppendLine("M=D      // ARG = *(FRAME-3)");

            sb.AppendLine("@4");
            sb.AppendLine("D=A");
            sb.AppendLine($"@{currentFunction}$FRAME.{_frameCount}");
            sb.AppendLine("D=M-D");
            sb.AppendLine("A=D");
            sb.AppendLine("D=M");
            sb.AppendLine("@LCL");
            sb.AppendLine("M=D      // LCL = *(FRAME-4)");

            sb.AppendLine($"@{currentFunction}$RET.{_retCount}");
            sb.AppendLine("A=M");
            sb.Append("0;JMP        // goto RET");

            _frameCount++;
            _retCount++;

            return sb.ToString();
        }

        public static string WriteCall(CommandInformation command, string line)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"@RET_ADDRESS.{command.Arg1}.{_retAddrCount}     // line = {line}");
            sb.AppendLine("D=A");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1        // push return-address");
            sb.AppendLine("@LCL");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1        // push LCL");
            sb.AppendLine("@ARG");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1        // push ARG");
            sb.AppendLine("@THIS");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1        // push THIS");
            sb.AppendLine("@THAT");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.AppendLine("M=M+1        // push THAT");
            sb.AppendLine("@SP");
            sb.AppendLine("D=M");
            sb.AppendLine($"@{command.Arg2}");
            sb.AppendLine("D=D-A");
            sb.AppendLine("@5");
            sb.AppendLine("D=D-A");
            sb.AppendLine("@ARG");
            sb.AppendLine("M=D          // ARG=SP-n-5");
            sb.AppendLine("@SP");
            sb.AppendLine("D=M");
            sb.AppendLine("@LCL");
            sb.AppendLine("M=D          // LCL=SP");
            sb.AppendLine($"@{command.Arg1}");
            sb.AppendLine("0;JMP");
            sb.Append($"(RET_ADDRESS.{command.Arg1}.{_retAddrCount})");
            
            _retAddrCount++;
            return sb.ToString();
        }

        static string WritePopStatic(CommandInformation command, string line, string fileName)
        {
            var className = fileName.Split('.')[0];
            var sb = new StringBuilder();

            sb.AppendLine($"@SP     // line = {line}");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine($"@{className}.{command.Arg2}");
            sb.Append("M=D");

            return sb.ToString();
        }

        static string WritePushStatic(CommandInformation command, string line, string fileName)
        {
            var className = fileName.Split('.')[0];
            var sb = new StringBuilder();

            sb.AppendLine($"@{className}.{command.Arg2}     // line = {line}");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.Append("M=M+1");

            return sb.ToString();
        }

        static string WritePushConstant(CommandInformation command, string line)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"@{command.Arg2}     // line = {line}");
            sb.AppendLine("D=A");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.Append("M=M+1");
            return sb.ToString();
        }

        static string WritePopPointer(CommandInformation command, string line)
        {
            string thisOrThat = command.Arg2 == 0 ? "THIS" : "THAT";
            var sb = new StringBuilder();
            sb.AppendLine($"@SP     // line = {line}");
            sb.AppendLine("M=M-1");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("D=M");
            sb.AppendLine($"@{thisOrThat}");
            sb.AppendLine("M=D");
            sb.Append("@SP");

            return sb.ToString();
        }

        static string WritePushPointer(CommandInformation command, string line)
        {
            string thisOrThat = command.Arg2 == 0 ? "THIS" : "THAT";
            var sb = new StringBuilder();
            sb.AppendLine($"@{thisOrThat}       // line = {line}");
            sb.AppendLine("D=M");
            sb.AppendLine("@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");
            sb.AppendLine("@SP");
            sb.Append("M=M+1");

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
