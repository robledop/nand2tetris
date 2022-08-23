using System;
using System.IO;
using JackCompiler.JackAnalyzer;
using JackCompiler.JackCodeGenerator;

if (args.Length == 0)
{
    Console.WriteLine("Provide a source file or a directory");
    return;
}
var path = args[0];


FileAttributes attr = File.GetAttributes(path);
var isDirectory = (attr & FileAttributes.Directory) == FileAttributes.Directory;

if (isDirectory)
{
    var endsWithDirSeparator = Path.EndsInDirectorySeparator(path);
    path = endsWithDirSeparator ? path.TrimEnd(Path.DirectorySeparatorChar) : path;

    var jackFiles = Directory.EnumerateFiles(path, "*.jack");
    foreach (var file in jackFiles)
    {
        Console.WriteLine($"Compiling file '{file}'");
        var vmFilePath = $"{path}{Path.DirectorySeparatorChar}{Path.GetFileName(file).Replace(".jack", ".vm")}";
        var vmCode = CompileFile(file);
        File.WriteAllText(vmFilePath, vmCode);
    }
}
else
{
    var vmFilePath = path.Replace(".jack", ".vm", StringComparison.OrdinalIgnoreCase);
    var vmCode = CompileFile(path);
    File.WriteAllText(vmFilePath, vmCode);
}

static string CompileFile(string path)
{
    try
    {
        var source = File.ReadAllText(path);
        var parser = new Parser();
        parser.GetTokens(source);
        var parseTree = parser.ParseClass();
        var codeGenerator = new CodeGenerator(parseTree);
        var compiledClass = codeGenerator.CompileClass();
        return compiledClass;
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        return null;
    }
}