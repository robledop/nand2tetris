using System.Text.RegularExpressions;
using HackAssembler;

if (args.Length == 0)
{
    Console.WriteLine("Provide a source file");
    return;
}
var file = args[0];
//var file = "Rect.asm";
var binaryFile = $"{file.Split(".")[0]}.hack";

var lines = (await File.ReadAllLinesAsync(file)).ToList();

var code = RemoveWhiteSpaceAndComments(lines);

var binaryCode = Compiler.Compile(code);

File.WriteAllText(binaryFile, string.Join(Environment.NewLine, binaryCode));

List<string> RemoveWhiteSpaceAndComments(List<string> list)
{
    for (int i = 0; i < list.Count; i++)
    {
        list[i] = list[i].Replace(" ", "");
        list[i] = StripComments(list[i].ToUpper());
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