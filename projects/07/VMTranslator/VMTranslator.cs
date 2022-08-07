using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace VMTranslator
{
    public class VMTranslator
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Provide a source file");
                return;
            }
            var path = args[0];

            FileAttributes attr = File.GetAttributes(path);
            var isDirectory = (attr & FileAttributes.Directory) == FileAttributes.Directory;

            if (isDirectory)
            {
                var endsWithDirSeparator = Path.EndsInDirectorySeparator(path);
                path = endsWithDirSeparator ? path.TrimEnd(Path.DirectorySeparatorChar) : path;

                var code = new List<string>();
                var vmFiles = Directory.EnumerateFiles(path, "*.vm");

                // bootstrap
                code.Add(CodeWriter.WriteInit());

                foreach (var file in vmFiles)
                {
                    var asmLines = ProcessFile(file);
                    code.AddRange(asmLines);
                }

                var dirName = path.Split(Path.DirectorySeparatorChar).Last();

                File.WriteAllText($"{dirName}{Path.DirectorySeparatorChar}{path}.asm", string.Join(Environment.NewLine, code));
            }
            else
            {
                var assemblyFile = path.Replace(".vm", ".asm", StringComparison.OrdinalIgnoreCase);
                var asmLines = ProcessFile(path);

                File.WriteAllText(assemblyFile, string.Join(Environment.NewLine, asmLines));
            }

        }

        static List<string> ProcessFile(string path)
        {
            var asmLines = new List<string>();
            var fileName = Path.GetFileName(path);

            var lines = File.ReadAllLines(path).ToList();
            var code = RemoveComments(lines);

            var currentFunction = "";
            foreach (var line in code)
            {
                var cmd = Parser.Parse(line);
                string asmLine = cmd.Type switch
                {
                    CommandType.Sub => CodeWriter.WriteSub(),
                    CommandType.Add => CodeWriter.WriteAdd(),
                    CommandType.Eq => CodeWriter.WriteEq(),
                    CommandType.Gt => CodeWriter.WriteGt(),
                    CommandType.Lt => CodeWriter.WriteLt(),
                    CommandType.Neg => CodeWriter.WriteNeg(),
                    CommandType.And => CodeWriter.WriteAnd(),
                    CommandType.Or => CodeWriter.WriteOr(),
                    CommandType.Not => CodeWriter.WriteNot(),
                    CommandType.Push => CodeWriter.WritePush(cmd, line, fileName),
                    CommandType.Pop => CodeWriter.WritePop(cmd, line, fileName),
                    CommandType.Label => CodeWriter.WriteLabel(cmd, line, currentFunction),
                    CommandType.If => CodeWriter.WriteIfGoto(cmd, line, currentFunction),
                    CommandType.Goto => CodeWriter.WriteGoto(cmd, line, currentFunction),
                    CommandType.Function => CodeWriter.WriteFunction(cmd, line),
                    CommandType.Return => CodeWriter.WriteReturn(line, currentFunction),
                    CommandType.Call => CodeWriter.WriteCall(cmd, line),
                    _ => throw new ArgumentOutOfRangeException(line)
                };

                if (cmd.Type is CommandType.Function)
                {
                    currentFunction = cmd.Arg1;
                }

                asmLines.Add(asmLine);
            }

            return asmLines;
        }

        static List<string> RemoveComments(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = StripComments(list[i]);
            }

            var linesToRemove = list.Where(string.IsNullOrWhiteSpace).ToList();

            foreach (var line in linesToRemove)
            {
                list.Remove(line);
            }

            return list;

        }
        static string StripComments(string line)
        {
            var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
            return Regex.Replace(line, re, "$1");
        }
    }
}

