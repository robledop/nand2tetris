using System.Text.RegularExpressions;
using VMTranslator;

if (args.Length == 0)
{
    Console.WriteLine("Provide a source file");
    return;
}
var file = args[0];
var outputFile = args[1];

var assemblyFile = $"{file.Split(".")[0]}.asm";
var asmLines = new List<string>();

var lines = (await File.ReadAllLinesAsync(file)).ToList();
var code = RemoveComments(lines);

foreach (var line in code)
{
    var cmd = Parser.Parse(line);
    var asmLine = cmd.Type switch
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
        CommandType.Push => CodeWriter.WritePush(cmd, line),
        CommandType.Pop => CodeWriter.WritePop(cmd, line),
        _ => throw new ArgumentOutOfRangeException()
    };

    asmLines.Add(asmLine);
}

File.WriteAllText(outputFile, string.Join(Environment.NewLine, asmLines));

List<string> RemoveComments(List<string> list)
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

    static string StripComments(string line)
    {
        var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
        return Regex.Replace(line, re, "$1");
    }
}