using System.Collections.Generic;
using System.Xml;
using JackCompiler.Exceptions;

namespace JackCompiler
{
    public class SyntaxAnalyzer
    {
        XmlNode _classNode;
        public List<Token> Tokens { get; set; } = new();
        public XmlDocument XmlDocument { get; set; } = new();
        public int Position { get; set; }
        public Token CurrentToken { get; set; }

        public void GetTokens(string source)
        {
            Tokens.Clear();
            var lexer = new Lexer(source);

            Token token = null;

            while (token?.Type != TokenType.EOF)
            {
                token = lexer.GetToken();
                Tokens.Add(token);
            }
        }

        public void AnalyzeClass()
        {
            CurrentToken = Tokens[Position];
            if (CurrentToken.Type == TokenType.Class)
            {
                _classNode = CreateNode("class");
                XmlDocument.AppendChild(_classNode);

                var keywordNode = CreateNode("keyword");
                keywordNode.InnerText = "class";
                _classNode.AppendChild(keywordNode);

                NextToken();

                if (CurrentToken.Type == TokenType.Identifier)
                {
                    var classIdentifierNode = CreateNode("identifier");
                    classIdentifierNode.InnerText = Tokens[Position].Value;
                    _classNode.AppendChild(classIdentifierNode);
                }
                else
                {
                    throw new JackLexerException($"The class needs and identifier at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }

                NextToken();


                ParseLeftCurlyBracket();

                NextToken();

                while (CurrentToken.Type is not (TokenType.RightCurlyBracket or TokenType.EOF))
                {
                    AnalyzeClassMembers();
                }

                ParseRightCurlyBracket();

                void ParseLeftCurlyBracket()
                {
                    if (CurrentToken.Type == TokenType.LeftCurlyBracket)
                    {
                        var symbolNode = CreateNode("symbol");
                        symbolNode.InnerText = "{";
                        _classNode.AppendChild(symbolNode);
                    }
                    else
                    {
                        throw new JackLexerException($"Expected left curly bracket at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                    }
                }

                void ParseRightCurlyBracket()
                {
                    if (CurrentToken.Type == TokenType.RightCurlyBracket)
                    {
                        var symbolNode = CreateNode("symbol");
                        symbolNode.InnerText = "}";
                        _classNode.AppendChild(symbolNode);
                    }
                    else
                    {
                        throw new JackLexerException($"Expected right curly bracket at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                    }
                }
            }
            else
            {
                throw new JackAnalyzerException($"The file must start with the 'class' token at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void AnalyzeClassMembers()
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Field or TokenType.Static:
                    do
                    {
                        AnalyzeClassVariableDeclarations();
                        NextToken();
                    } while (CurrentToken.Type != TokenType.Function &&
                             CurrentToken.Type != TokenType.Constructor &&
                             CurrentToken.Type != TokenType.Method &&
                             CurrentToken.Type != TokenType.EOF);
                    break;

                case TokenType.Function or TokenType.Method or TokenType.Constructor:
                    do
                    {
                        AnalyzeSubroutine();
                        NextToken();
                    } while (CurrentToken.Type != TokenType.RightCurlyBracket && CurrentToken.Type != TokenType.EOF);
                    break;

                default: throw new JackLexerException($"A class needs to have variables and/or subroutines at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void AnalyzeSubroutine()
        {
            var subRoutineNode = CreateNode("subroutineDec");
            _classNode.AppendChild(subRoutineNode);

            ParseSubroutineType();
            NextToken();
            ParseReturnType();
            NextToken();
            ParseSubroutineName();
            NextToken();
            ParseLeftParenthesis();
            NextToken();
            if (CurrentToken.Type != TokenType.RightParenthesis)
            {
                ParseParameterList();
            }
            else
            {
                ParseRightParenthesis();
            }

            do
            {
                NextToken();
                ParseLeftCurlyBracket();
                NextToken();
                var subroutineBodyNode = CreateNode("subroutineBody");
                _classNode.AppendChild(subRoutineNode);
                ParseSubroutineBody(subroutineBodyNode);
                subRoutineNode.AppendChild(subroutineBodyNode);

                NextToken();
                ParseRightCurlyBracket();
                NextToken();
            } while (CurrentToken.Type != TokenType.Function &&
                     CurrentToken.Type != TokenType.Constructor &&
                     CurrentToken.Type != TokenType.Method &&
                     CurrentToken.Type != TokenType.EOF);


            void ParseSubroutineBody(XmlNode parentNode)
            {
                var varDecNode = CreateNode("varDec");
                parentNode.AppendChild(varDecNode);
                do
                {
                    ParseKeyword(parentNode: varDecNode, "var");

                    NextToken();
                    ParseType(varDecNode);
                    NextToken();
                    do
                    {
                        if (CurrentToken.Type == TokenType.Comma)
                        {
                            ParseSymbol(parentNode: varDecNode, ",");
                        }

                        ParseIdentifier(varDecNode);
                        NextToken();
                    } while (CurrentToken.Type == TokenType.Comma);

                    ParseSemicolon(varDecNode);
                    NextToken();

                } while (CurrentToken.Type == TokenType.Var);

                var statementsNode = CreateNode("statements");
                ParseStatements(parentNode: statementsNode);
                parentNode.AppendChild(statementsNode);

                parentNode.AppendChild(varDecNode);
            }

            void ParseLeftCurlyBracket()
            {
                if (CurrentToken.Type == TokenType.LeftCurlyBracket)
                {
                    var symbolNode = CreateNode("symbol");
                    symbolNode.InnerText = "{";
                    subRoutineNode.AppendChild(symbolNode);
                }
                else
                {
                    throw new JackLexerException($"Expected left curly bracket at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }

            void ParseRightCurlyBracket()
            {
                if (CurrentToken.Type == TokenType.RightCurlyBracket)
                {
                    var symbolNode = CreateNode("symbol");
                    symbolNode.InnerText = "}";
                    subRoutineNode.AppendChild(symbolNode);
                }
                else
                {
                    throw new JackLexerException($"Expected right curly bracket at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }

            void ParseParameterList()
            {
                var parameterListNode = CreateNode("parameterList");
                subRoutineNode.AppendChild(parameterListNode);
                do
                {
                    if (CurrentToken.Type == TokenType.Comma)
                    {
                        ParseSymbol(parameterListNode, ",");
                        NextToken();
                    }

                    ParseType(parameterListNode);
                    NextToken();
                    ParseIdentifier(parameterListNode);
                    NextToken();
                } while (CurrentToken.Type == TokenType.Comma);

                subRoutineNode.AppendChild(parameterListNode.Clone());
                ParseRightParenthesis();
            }

            void ParseType(XmlNode parentNode)
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Int:
                        ParseKeyword(parentNode, "int");
                        break;

                    case TokenType.Boolean:
                        ParseKeyword(parentNode, "boolean");
                        break;

                    case TokenType.Char:
                        ParseKeyword(parentNode, "char");
                        break;

                    case TokenType.Identifier:
                        ParseIdentifier(parentNode);
                        break;

                    default:
                        throw new JackAnalyzerException($"Type expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }

            void ParseLeftParenthesis()
            {
                if (CurrentToken.Type == TokenType.LeftParenthesis)
                {
                    ParseSymbol(subRoutineNode, "(");
                }
                else
                {
                    throw new JackAnalyzerException($"Left parenthesis expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }

            void ParseRightParenthesis()
            {
                if (CurrentToken.Type == TokenType.RightParenthesis)
                {
                    ParseSymbol(subRoutineNode, ")");
                }
                else
                {
                    throw new JackAnalyzerException($"Right parenthesis expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }

            void ParseSubroutineName()
            {
                if (CurrentToken.Type == TokenType.Identifier)
                {
                    ParseIdentifier(subRoutineNode);
                }
                else
                {
                    throw new JackAnalyzerException($"Subroutine identifier expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }

            void ParseReturnType()
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Void:
                        ParseKeyword(subRoutineNode, "void");
                        break;

                    case TokenType.Identifier:
                        ParseIdentifier(subRoutineNode);
                        break;

                    case TokenType.Int:
                        ParseKeyword(subRoutineNode, "int");
                        break;

                    case TokenType.Boolean:
                        ParseKeyword(subRoutineNode, "boolean");
                        break;

                    case TokenType.Char:
                        ParseKeyword(subRoutineNode, "char");
                        break;

                    default:
                        throw new JackAnalyzerException($"Return type expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }

            void ParseSubroutineType()
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Function:
                        ParseKeyword(subRoutineNode, "function");
                        break;

                    case TokenType.Method:
                        ParseKeyword(subRoutineNode, "method");
                        break;

                    case TokenType.Constructor:
                        ParseKeyword(subRoutineNode, "constructor");
                        break;

                    default:
                        throw new JackAnalyzerException($"Subroutine type expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }
        }

        void ParseStatements(XmlNode parentNode)
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Let:
                    ParseLetStatement(parentNode: parentNode);
                    break;

                case TokenType.If:
                    ParseIfStatement(parentNode: parentNode);
                    break;

                case TokenType.Else:
                    ParseElseStatement(parentNode: parentNode);
                    break;

                case TokenType.While:
                    ParseWhileStatement(parentNode: parentNode);
                    break;

                case TokenType.Do:
                    ParseDoStatement(parentNode: parentNode);
                    break;

                case TokenType.Return:
                    ParseReturnStatement(parentNode: parentNode);
                    break;
            }
        }

        void ParseReturnStatement(XmlNode parentNode)
        {
            throw new System.NotImplementedException();
        }

        void ParseDoStatement(XmlNode parentNode)
        {
            throw new System.NotImplementedException();
        }

        void ParseWhileStatement(XmlNode parentNode)
        {
            throw new System.NotImplementedException();
        }

        void ParseElseStatement(XmlNode parentNode)
        {
            throw new System.NotImplementedException();
        }

        void ParseIfStatement(XmlNode parentNode)
        {
            throw new System.NotImplementedException();
        }

        void ParseLetStatement(XmlNode parentNode)
        {
            var keyword = CreateNode("keyword");
            keyword.InnerText = "let";
            parentNode.AppendChild(keyword);
            NextToken();
            ParseIdentifier(parentNode); // varName
            NextToken();
            ParseEqualSign(parentNode);
            NextToken();
            ParseExpression(parentNode);
        }


        void ParseExpression(XmlNode parentNode)
        {
            var expressionNode = CreateNode("expression");
            parentNode.AppendChild(expressionNode);
            ParseTerm();


            void ParseTerm()
            {
                var termNode = CreateNode("term");
                expressionNode.AppendChild(termNode);

                switch (CurrentToken.Type)
                {
                    case TokenType.Identifier:
                        ParseVariableName(termNode);
                        break;

                    case TokenType.IntegerConstant:
                        ParseIntegerConstant(parentNode: termNode);
                        break;

                    case TokenType.StringConstant:
                        ParseStringConstant(parentNode: termNode);

                        break;

                    case TokenType.True:
                        ParseKeyword(parentNode: termNode, "true");
                        break;

                    case TokenType.False:
                        ParseKeyword(parentNode: termNode, "false");
                        break;

                    case TokenType.Null:
                        ParseKeyword(parentNode: termNode, "null");
                        break;

                    case TokenType.This:
                        ParseKeyword(parentNode: termNode, "this");
                        break;

                    case TokenType.Minus:
                        ParseSymbol(parentNode: termNode, "-");
                        break;

                    case TokenType.Not:
                        ParseSymbol(parentNode: termNode, "~");
                        break;
                }
            }
        }

        void AnalyzeClassVariableDeclarations()
        {
            var classVarDecNode = CreateNode("classVarDec");
            var keywordNode = CreateNode("keyword");
            var identifierNode = CreateNode("identifier");
            var symbolNode = CreateNode("symbol");
            _classNode.AppendChild(classVarDecNode);
            ParseStaticOrField();
            NextToken();
            ParseType();
            NextToken();
            do
            {
                if (CurrentToken.Type == TokenType.Comma)
                {
                    symbolNode.RemoveAll();
                    symbolNode.InnerText = ",";
                    classVarDecNode.AppendChild(symbolNode.Clone());
                }

                ParseIdentifier(parentNode: classVarDecNode);
                NextToken();
            } while (CurrentToken.Type == TokenType.Comma);

            ParseSemicolon(parentNode: classVarDecNode);



            void ParseStaticOrField()
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Field:
                        keywordNode.RemoveAll();
                        keywordNode.InnerText = "field";
                        classVarDecNode.AppendChild(keywordNode.Clone());
                        break;

                    case TokenType.Static:
                        keywordNode.RemoveAll();
                        keywordNode.InnerText = "static";
                        classVarDecNode.AppendChild(keywordNode.Clone());
                        break;
                }
            }

            void ParseType()
            {
                switch (CurrentToken.Type)
                {
                    case TokenType.Int:
                        keywordNode.RemoveAll();
                        keywordNode.InnerText = "int";
                        classVarDecNode.AppendChild(keywordNode.Clone());
                        break;

                    case TokenType.Boolean:
                        keywordNode.RemoveAll();
                        keywordNode.InnerText = "boolean";
                        classVarDecNode.AppendChild(keywordNode.Clone());
                        break;

                    case TokenType.Char:
                        keywordNode.RemoveAll();
                        keywordNode.InnerText = "char";
                        classVarDecNode.AppendChild(keywordNode.Clone());
                        break;

                    case TokenType.Identifier:
                        keywordNode.RemoveAll();
                        keywordNode.InnerText = CurrentToken.Value;
                        classVarDecNode.AppendChild(keywordNode.Clone());
                        break;

                    default:
                        throw new JackAnalyzerException($"Type expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
                }
            }
        }

        void ParseVariableName(XmlNode parentNode)
        {
            NextToken();
            switch (CurrentToken.Type)
            {
                case TokenType.LeftSquareBracket:
                    ParseArrayExpression();
                    break;

                case TokenType.Period:
                    ParseSubroutineCallOnAnObject();
                    break;

                case TokenType.LeftParenthesis:
                    ParseSubroutineCall();
                    break;

                default:
                    ParseVariable();
                    break;
            }

            void ParseSubroutineCall()
            {
                throw new System.NotImplementedException();
            }

            void ParseSubroutineCallOnAnObject()
            {
                throw new System.NotImplementedException();
            }

            void ParseVariable()
            {
                PreviousToken();
                ParseIdentifier(parentNode: parentNode);
            }

            void ParseArrayExpression()
            {
                throw new System.NotImplementedException();
            }
        }

        void ParseSymbol(XmlNode parentNode, string symbol)
        {
            var symbolNode = CreateNode("keyword");
            symbolNode.InnerText = symbol;
            parentNode.AppendChild(symbolNode);
        }

        void ParseKeyword(XmlNode parentNode, string keyword)
        {
            var keywordNode = CreateNode("keyword");
            keywordNode.InnerText = keyword;
            parentNode.AppendChild(keywordNode);
        }

        void ParseStringConstant(XmlNode parentNode)
        {
            var stringConstantNode = CreateNode("stringConstant");
            stringConstantNode.InnerText = CurrentToken.Value;
            parentNode.AppendChild(stringConstantNode);
        }

        void ParseIntegerConstant(XmlNode parentNode)
        {
            var integerConstantNode = CreateNode("integerConstant");
            integerConstantNode.InnerText = CurrentToken.Value;
            parentNode.AppendChild(integerConstantNode);
        }

        void ParseEqualSign(XmlNode parentNode)
        {
            if (CurrentToken.Type == TokenType.Equal)
            {
                var symbol = CreateNode("symbol");
                symbol.InnerText = "=";
                parentNode.AppendChild(symbol);
            }
            else
            {
                throw new JackAnalyzerException($"Equal sign expected at  line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void ParseIdentifier(XmlNode parentNode)
        {
            if (CurrentToken.Type == TokenType.Identifier)
            {
                var identifierNode = CreateNode("identifier");
                identifierNode.InnerText = CurrentToken.Value;
                parentNode.AppendChild(identifierNode.Clone());
            }
            else
            {
                throw new JackAnalyzerException($"Identifier expected at  line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void ParseSemicolon(XmlNode parentNode)
        {
            if (CurrentToken.Type == TokenType.SemiColon)
            {
                var symbolNode = CreateNode("symbol");
                symbolNode.InnerText = ";";
                parentNode.AppendChild(symbolNode);
            }
            else
            {
                throw new JackAnalyzerException($"Semicolon expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }


        XmlNode CreateNode(string name)
        {
            return XmlDocument.CreateElement("", name, "");
        }

        void NextToken()
        {
            Position++;
            CurrentToken = Tokens[Position];
        }

        void PreviousToken()
        {
            Position--;
            CurrentToken = Tokens[Position];
        }
    }
}