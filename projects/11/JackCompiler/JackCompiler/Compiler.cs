using System.IO;
using JackCompiler.JackAnalyzer;
using JackCompiler.JackCodeGenerator;

namespace JackCompiler
{
    public class Compiler
    {
        public void Compile(string filePath)
        {
            var source = File.ReadAllText(filePath);
            var parser = new Parser();
            parser.GetTokens(source);
            var parseTree = parser.ParseClass();
            var codeGenerator = new CodeGenerator(parseTree);

            codeGenerator.CompileClass();
        }
    }
}