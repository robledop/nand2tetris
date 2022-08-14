using JackCompiler;
using System;
using System.IO;
using System.Xml;

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
        var xmlFilePath = $"{path}{Path.DirectorySeparatorChar}{Path.GetFileName(file).Replace(".jack", ".xml")}";
        Console.WriteLine($"Parsing file '{xmlFilePath}'");
        var xml = ProcessFile(file);
        if (xml is not null)
        {
            xml.Save(xmlFilePath);
        }
    }
}
else
{
    var xmlFilePath = path.Replace(".jack", ".xml", StringComparison.OrdinalIgnoreCase);
    var xml = ProcessFile(path);
    if (xml is not null)
    {
        xml.Save(xmlFilePath);
    }
}

static XmlDocument ProcessFile(string path)
{
    try
    {
        var source = File.ReadAllText(path);
        var parser = new Parser();
        parser.GetTokens(source);
        return parser.ParseClass();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        return null;
    }
}