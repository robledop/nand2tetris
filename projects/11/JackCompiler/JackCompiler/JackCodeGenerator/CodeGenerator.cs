using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        int _whileCount;
        int _ifCount;

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
                        if (childNode.NextSibling.Name == "identifier" && varIdentifiers.Count == 0)
                        {
                            varIdentifiers.Add(childNode.NextSibling.InnerText.Trim());
                        }
                    }

                    //if (childNode.Name == "identifier" && varIdentifiers.Count == 0)
                    //{
                    //    varIdentifiers.Add(innerText);
                    //}
                }

                foreach (var varIdentifier in varIdentifiers)
                {
                    var existingVarsOfSameKind = ClassSymbolTable.Count(v => v.Kind == kind);
                    var symbol = new Symbol(varIdentifier, type, (SymbolKind)kind!, existingVarsOfSameKind);
                    ClassSymbolTable.Add(symbol);
                }
            }
        }

        internal string CompileStatements(XmlNode statementsNode)
        {
            var sb = new StringBuilder();

            foreach (XmlNode statement in statementsNode.ChildNodes)
            {
                switch (statement.Name)
                {
                    case "letStatement":
                        sb.Append(CompileLetStatement(statement));
                        break;

                    case "returnStatement":
                        sb.Append(CompileReturnStatement(statement));
                        break;

                    case "ifStatement":
                        sb.Append(CompileIfStatement(statement));
                        break;

                    case "whileStatement":
                        sb.Append(CompileWhileStatement(statement));
                        break;

                    case "doStatement":
                        sb.Append(CompileDoStatement(statement));
                        break;
                }
            }

            return sb.ToString();
        }

        internal string CompileDoStatement(XmlNode statement)
        {
            var sb = new StringBuilder();
            var expressionList = statement.SelectSingleNode(".//expressionList");
            var expressions = expressionList.SelectNodes(".//expression");
            sb.Append(CompileSubroutineCall(statement, expressions));

            return sb.ToString();
        }

        internal string CompileWhileStatement(XmlNode statement)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"label WHILE_EXP{_whileCount}");
            var whileExpression = statement.SelectSingleNode("expression");
            sb.Append(CompileExpression(whileExpression));
            sb.AppendLine("not");
            sb.AppendLine($"if-goto WHILE_END{_whileCount}");
            var statements = statement.SelectSingleNode("statements");
            sb.Append(CompileStatements(statements));

            sb.AppendLine($"goto WHILE_EXP{_whileCount}");
            sb.AppendLine($"label WHILE_END{_whileCount}");
            _whileCount++;
            return sb.ToString();
        }

        internal string CompileIfStatement(XmlNode statement)
        {
            var sb = new StringBuilder();
            var expression = statement.SelectSingleNode("expression");
            sb.Append(CompileExpression(expression));
            sb.AppendLine($"if-goto IF_TRUE{_ifCount}");
            sb.AppendLine($"goto IF_FALSE{_ifCount}");
            sb.AppendLine($"label IF_TRUE{_ifCount}");

            var ifTrueStatements = statement.SelectNodes(".//statements")?.Item(0);
            if (ifTrueStatements is not null)
            {
                sb.Append(CompileStatements(ifTrueStatements));
            }
            sb.AppendLine($"label IF_FALSE{_ifCount}");
            var ifFalseStatements = statement.SelectNodes(".//statements")?.Item(1);
            if (ifFalseStatements is not null)
            {
                sb.Append(CompileStatements(ifFalseStatements));
            }

            _ifCount++;
            return sb.ToString();
        }

        internal string CompileReturnStatement(XmlNode statement)
        {
            var sb = new StringBuilder();

            sb.AppendLine("return");
            return sb.ToString();
        }

        internal string CompileLetStatement(XmlNode letStatementNode)
        {
            var sb = new StringBuilder();

            var expression = letStatementNode.SelectSingleNode("expression");
            sb.Append(CompileExpression(expression));

            var identifierNode = letStatementNode.SelectSingleNode("identifier");

            var identifier = identifierNode.InnerText.Trim();
            var symbol = SubroutineSymbolTable.FirstOrDefault(s => s.Name == identifier) ??
                         ClassSymbolTable.FirstOrDefault(s => s.Name == identifier);
            sb.AppendLine($"pop {GetSegmentName(symbol!.Kind)} {symbol.Position}");


            return sb.ToString();
        }

        internal string CompileExpression(XmlNode expressionNode)
        {
            var sb = new StringBuilder();
            var terms = expressionNode.SelectNodes(".//term");

            // if exp is "f(exp1, exp2, ...)":
            //      codeWrite(exp1),
            //      codeWrite(exp2), ...,
            //      output "call f"
            var expressionList = expressionNode.SelectNodes(".//expressionList")?.Item(0);
            if (expressionList is not null)
            {
                var expressions = expressionList.SelectNodes("expression");

                var firstTerm = terms[0];

                sb.Append(CompileSubroutineCall(firstTerm, expressions));
            }

            // if exp is "op exp"
            //      codeWrite(exp),
            //      output "op"
            else if (terms.Count == 2 && terms[0]?.FirstChild?.Name == "symbol")
            {
                var firstTerm = terms[0];
                var secondTerm = terms[1];
                var expNode = CreateNode(secondTerm?.OwnerDocument, "expression");
                expNode.AppendChild(secondTerm.CloneNode(true));
                sb.Append(CompileExpression(expNode));
                sb.AppendLine(GetUnaryOp(firstTerm.FirstChild?.InnerText.Trim()));
            }

            // if exp is "exp1 op exp2":
            //      codeWrite(exp1),
            //      codeWrite(exp2),
            //      output "op"
            else if (terms.Count == 2)
            {
                var opTerms = expressionNode.SelectNodes("term");
                foreach (XmlNode opTerm in opTerms)
                {
                    var expNode1 = CreateNode(opTerm?.OwnerDocument, "expression");
                    expNode1.AppendChild(opTerm.CloneNode(true));
                    sb.Append(CompileExpression(expNode1));
                }

                var symbol = expressionNode.SelectNodes("symbol")?[0];
                sb.AppendLine(GetOp(symbol.InnerText.Trim()));
            }
            else if (terms!.Count == 1)
            {
                var term = terms[0];
                switch (term!.FirstChild!.Name)
                {
                    // if exp is number n:
                    //      output "push n"
                    case "integerConstant":
                        sb.AppendLine($"push constant {term.FirstChild.InnerText.Trim()}");
                        break;

                    // if exp is a variable var:
                    //      output "push var"
                    case "identifier":
                        var identifier = term.FirstChild.InnerText.Trim();
                        var symbol = SubroutineSymbolTable.FirstOrDefault(s => s.Name == identifier) ??
                                     ClassSymbolTable.FirstOrDefault(s => s.Name == identifier);
                        sb.AppendLine($"push {GetSegmentName(symbol!.Kind)} {symbol.Position}");
                        break;
                }
            }
            else
            {
                throw new JackCompilerException("Expression not recognized");
            }

            return sb.ToString();
        }

        internal string CompileSubroutineCall(XmlNode firstTerm, XmlNodeList expressions)
        {
            var sb = new StringBuilder();

            if (expressions is not null)
            {
                foreach (XmlNode expression in expressions)
                {
                    var expNode = CreateNode(expression?.OwnerDocument, "expression");
                    expNode.AppendChild(expression.CloneNode(true));
                    sb.Append(CompileExpression(expNode));
                }
            }

            var identifiers = firstTerm.SelectNodes("identifier");
            var firstIdentifierNode = identifiers[0];
            var firstIdentifier = firstIdentifierNode.InnerText.Trim();

            var symbol = SubroutineSymbolTable.FirstOrDefault(s => s.Name == firstIdentifier) ??
                         ClassSymbolTable.FirstOrDefault(s => s.Name == firstIdentifier);

            if (symbol is not null)
            {
                sb.AppendLine($"push {GetSegmentName(symbol!.Kind)} {symbol.Position}");
                sb.AppendLine("pop pointer 0");
                var secondIdentifier = identifiers[1];
                sb.AppendLine($"call {symbol.Type}.{secondIdentifier?.InnerText.Trim()} {expressions?.Count ?? 0}");
            }
            else if (firstIdentifierNode.NextSibling?.InnerText.Trim() == ".")
            {
                var secondIdentifier = identifiers[1];
                sb.AppendLine($"call {firstIdentifierNode.InnerText.Trim()}.{secondIdentifier?.InnerText.Trim()} {expressions?.Count ?? 0}");
            }
            else if (firstIdentifierNode.NextSibling?.InnerText.Trim() == "(")
            {
                sb.AppendLine($"call {firstIdentifierNode.InnerText.Trim()} {expressions?.Count ?? 0}");
            }

            return sb.ToString();
        }

        string GetSegmentName(SymbolKind kind)
        {
            return kind switch
            {
                SymbolKind.Argument => "argument",
                SymbolKind.Local => "local",
                SymbolKind.Field => "this",
                SymbolKind.Static => "static",
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
        }

        string GetUnaryOp(string op)
        {
            return op switch
            {
                "-" => "neg",
                "~" => "not",
                _ => throw new NotSupportedException(op)
            };
        }

        string GetOp(string op)
        {
            return op switch
            {
                "-" => "sub",
                "+" => "add",
                "&" => "and",
                "|" => "or",
                "=" => "eq",
                "<" => "lt",
                ">" => "gt",
                "*" => "call Math.multiply() 2",
                "/" => "call Math.divide() 2",
                _ => throw new NotSupportedException(op)
            };
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

        XmlNode CreateNode(XmlDocument xmlDoc, string name)
        {
            var node = xmlDoc.CreateElement("", name, "");
            node.IsEmpty = false;
            return node;
        }
    }
}