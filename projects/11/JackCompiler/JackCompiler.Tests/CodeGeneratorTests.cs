using FluentAssertions;
using JackCompiler.JackAnalyzer;
using JackCompiler.JackCodeGenerator;

namespace JackCompiler.Tests;

public class CodeGeneratorTests
{


    [Fact]
    public void Square_GenerateClassSymbolTable()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var parseTree = parser.ParseClass();
        var codeGenerator = new CodeGenerator(parseTree);
        
        codeGenerator.GenerateClassSymbolTable();

        codeGenerator.ClassSymbolTable.Count.Should().Be(3);

        var compareTo = new List<Symbol>
        {
            new Symbol("x", "int", SymbolKind.Field, 0),
            new Symbol("y", "int", SymbolKind.Field, 1),
            new Symbol("size", "int", SymbolKind.Field, 2),
        };

        codeGenerator.ClassSymbolTable.Should().BeEquivalentTo(compareTo);
    }

    [Fact]
    public void Square_GenerateSubroutineSymbolTable()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var parseTree = parser.ParseClass();
        var codeGenerator = new CodeGenerator(parseTree);

        var constructor = parseTree.SelectSingleNode("//subroutineDec[identifier=' new ']");

        codeGenerator.CompileSubroutine(constructor);

        codeGenerator.SubroutineSymbolTable.Count.Should().Be(4);

        var compareTo = new List<Symbol>
        {
            new Symbol("this", "Square", SymbolKind.Argument, 0),
            new Symbol("Ax", "int", SymbolKind.Argument, 1),
            new Symbol("Ay", "int", SymbolKind.Argument, 2),
            new Symbol("Asize", "int", SymbolKind.Argument, 3)
        };

        codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
    }

    [Fact]
    public void SquareMain_GenerateClassSymbolTable()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Main.jack");

        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var parseTree = parser.ParseClass();
        var codeGenerator = new CodeGenerator(parseTree);

        codeGenerator.GenerateClassSymbolTable();

        codeGenerator.ClassSymbolTable.Count.Should().Be(1);

        var compareTo = new List<Symbol>
        {
            new Symbol("test", "boolean", SymbolKind.Static, 0)
        };

        codeGenerator.ClassSymbolTable.Should().BeEquivalentTo(compareTo);
    }

    [Fact]
    public void SquareMain_GenerateSubroutineSymbolTable_more()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Main.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var parseTree = parser.ParseClass();
        var codeGenerator = new CodeGenerator(parseTree);

        var functionMore = parseTree.SelectSingleNode("//subroutineDec[identifier=' more ']");

        codeGenerator.CompileSubroutine(functionMore);

        codeGenerator.SubroutineSymbolTable.Count.Should().Be(5);

        var compareTo = new List<Symbol>
        {
            new Symbol("this", "Main", SymbolKind.Argument, 0),
            new Symbol("i", "int", SymbolKind.Local, 0),
            new Symbol("j", "int", SymbolKind.Local, 1),
            new Symbol("s", "String", SymbolKind.Local, 2),
            new Symbol("a", "Array", SymbolKind.Local, 3),
        };

        codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
    }

    [Fact]
    public void Square_GenerateSubroutineSymbolTable_draw()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var parseTree = parser.ParseClass();
        var codeGenerator = new CodeGenerator(parseTree);

        var functionMore = parseTree.SelectSingleNode("//subroutineDec[identifier=' draw ']");

        codeGenerator.CompileSubroutine(functionMore);

        codeGenerator.SubroutineSymbolTable.Count.Should().Be(1);

        var compareTo = new List<Symbol>
        {
            new Symbol("this", "Square", SymbolKind.Argument, 0),
        };

        codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
    }

    [Fact]
    public void ArrayTestMain_GenerateClassSymbolTable()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");

        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var parseTree = parser.ParseClass();
        var codeGenerator = new CodeGenerator(parseTree);

        codeGenerator.GenerateClassSymbolTable();

        codeGenerator.ClassSymbolTable.Count.Should().Be(0);
    }

    [Fact]
    public void ArrayTestMain_GenerateSubroutineSymbolTable()
    {
        string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");

        var source = File.ReadAllText(path);

        var parser = new Parser();
        parser.GetTokens(source);
        var parseTree = parser.ParseClass();
        var codeGenerator = new CodeGenerator(parseTree);

        var functionMain = parseTree.SelectSingleNode("//subroutineDec[identifier=' main ']");

        codeGenerator.CompileSubroutine(functionMain);

        codeGenerator.SubroutineSymbolTable.Count.Should().Be(5);

        var compareTo = new List<Symbol>
        {
            new Symbol("this", "Main", SymbolKind.Argument, 0),
            new Symbol("a", "Array", SymbolKind.Local, 0),
            new Symbol("length", "int", SymbolKind.Local, 1),
            new Symbol("i", "int", SymbolKind.Local, 2),
            new Symbol("sum", "int", SymbolKind.Local, 3),
        };

        codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
    }
}