using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var file = args[0];

            var fileName = Path.GetFileName(file);
            var assemblyFile = file.Replace(".vm", ".asm", StringComparison.OrdinalIgnoreCase);
            var asmLines = new List<string>();

            var lines = File.ReadAllLines(file).ToList();
            var code = RemoveComments(lines);

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
                    _ => throw new ArgumentOutOfRangeException()
                };

                asmLines.Add(asmLine);
            }

            File.WriteAllText(assemblyFile, string.Join(Environment.NewLine, asmLines));

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

