using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JackCompiler.Exceptions;

namespace JackCompiler.JackCodeGenerator
{
    public class CodeGenerator
    {
        XmlDocument ParseTree { get; }
        internal List<Symbol> ClassSymbolTable { get; set; } = new();
        internal List<Symbol> SubroutineSymbolTable { get; set; } = new();
        internal XmlNode ClassNode { get; set; }

        public CodeGenerator(XmlDocument parseTree)
        {
            ParseTree = parseTree;
            ClassNode = ParseTree.FirstChild;
        }

        public void CompileClass()
        {
            GenerateClassSymbolTable();
            CompileSubroutines();
        }

        internal void GenerateClassSymbolTable()
        {
            var classVarDecs = ClassNode!.SelectNodes("classVarDec");

            foreach (XmlNode classVarDec in classVarDecs!)
            {
                SymbolKind? kind = null;
                string type = null;
                List<string> varIdentifiers = new();

                bool multiVariableDeclaration = false;
                foreach (XmlNode childNode in classVarDec.ChildNodes)
                {
                    var innerText = childNode.InnerText.Trim();
                    if (innerText.Contains(","))
                    {
                        multiVariableDeclaration = true;
                        continue;
                    }

                    if (multiVariableDeclaration)
                    {
                        varIdentifiers.Add(innerText);
                        multiVariableDeclaration = false;
                    }

                    if (kind is null)
                    {
                        kind = innerText switch
                        {
                            "field" => SymbolKind.Field,
                            "static" => SymbolKind.Static,
                            _ => throw new JackCompilerException($"Keyword '{innerText}' not supported as a class variable kind")
                        };

                        continue;
                    }

                    if (childNode.Name is "identifier" or "keyword")
                    {
                        type ??= innerText;
                    }

                    if (childNode.Name == "identifier" && varIdentifiers.Count == 0)
                    {
                        varIdentifiers.Add(innerText);
                    }
                }

                foreach (var varIdentifier in varIdentifiers)
                {
                    var existingVarsOfSameKind = ClassSymbolTable.Count(v => v.Kind == kind);
                    var symbol = new Symbol(varIdentifier, type, (SymbolKind)kind!, existingVarsOfSameKind);
                    ClassSymbolTable.Add(symbol);
                }
            }
        }

        internal void CompileSubroutines()
        {
            var subroutines = ClassNode!.SelectNodes("subroutineDec");

            foreach (XmlNode subroutine in subroutines!)
            {
                CompileSubroutine(subroutine);
            }
        }

        internal void CompileSubroutine(XmlNode subroutine)
        {
            SubroutineSymbolTable.Clear();
            AddArgumentsToTheSubroutineSymbolTable(subroutine);

            var body = subroutine.SelectSingleNode("subroutineBody");
            AddLocalVariablesToSubroutineSymbolTable(body);
        }

        internal void AddArgumentsToTheSubroutineSymbolTable(XmlNode subroutine)
        {
            // add 'this' to the argument list
            var classIdentifierNode = ClassNode.SelectSingleNode("identifier");
            var className = classIdentifierNode!.InnerText.Trim();
            var @this = new Symbol("this", className, SymbolKind.Argument, 0);
            SubroutineSymbolTable.Add(@this);

            // add the parameters list to the argument list
            var parameterListNode = subroutine.SelectSingleNode("parameterList");
            var paramNodes = parameterListNode!.ChildNodes;
            var node = paramNodes.Item(0);

            while (node is not null)
            {
                if (node.InnerText.Trim() == ",")
                {
                    node = node.NextSibling;
                }

                var type = node!.InnerText.Trim();
                node = node.NextSibling;
                var name = node!.InnerText.Trim();

                var argumentCount = SubroutineSymbolTable.Count(x => x.Kind == SymbolKind.Argument);
                var argument = new Symbol(name, type, SymbolKind.Argument, argumentCount);
                SubroutineSymbolTable.Add(argument);

                node = node.NextSibling;
            }
        }

        internal void AddLocalVariablesToSubroutineSymbolTable(XmlNode body)
        {
            var varDecs = body.SelectNodes("varDec");

            foreach (XmlNode varDec in varDecs!)
            {
                string type = null;
                List<string> varIdentifiers = new();

                bool multiVariableDeclaration = false;
                foreach (XmlNode childNode in varDec.ChildNodes)
                {
                    var innerText = childNode.InnerText.Trim();
                    if (innerText == "var")
                    {
                        continue;
                    }

                    if (innerText.Contains(","))
                    {
                        multiVariableDeclaration = true;
                        continue;
                    }

                    if (multiVariableDeclaration)
                    {
                        varIdentifiers.Add(innerText);
                        multiVariableDeclaration = false;
                    }

                    if (childNode.Name is "identifier" or "keyword" && type is null)
                    {
                        type = innerText;
                        continue;
                    }

                    if (childNode.Name == "identifier" && varIdentifiers.Count == 0)
                    {
                        varIdentifiers.Add(innerText);
                    }
                }

                foreach (var varIdentifier in varIdentifiers)
                {
                    var existingVarsOfSameKindCount = SubroutineSymbolTable.Count(v => v.Kind == SymbolKind.Local);
                    var symbol = new Symbol(varIdentifier, type, SymbolKind.Local, existingVarsOfSameKindCount);
                    SubroutineSymbolTable.Add(symbol);
                }
            }
        }
    }
}