using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using JackCompiler.Exceptions;

namespace JackCompiler
{
    public class Parser
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

        public void ParseClass()
        {
            CurrentToken = Tokens[Position];
            if (CurrentToken.Type == TokenType.Class)
            {
                _classNode = CreateNode("class");
                XmlDocument.AppendChild(_classNode);

                ParseKeyword(_classNode, "class");

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

                ParseSymbol(_classNode, "{");

                NextToken();

                while (CurrentToken.Type is not (TokenType.RightCurlyBracket or TokenType.EOF))
                {
                    ParseClassMembers();
                }

                ParseSymbol(_classNode, "}");
            }
            else
            {
                throw new JackAnalyzerException($"The file must start with the 'class' token at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void ParseClassMembers()
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Field or TokenType.Static:
                    do
                    {
                        ParseClassVariableDeclarations();
                        NextToken();
                    } while (CurrentToken.Type != TokenType.Function &&
                             CurrentToken.Type != TokenType.Constructor &&
                             CurrentToken.Type != TokenType.Method &&
                             CurrentToken.Type != TokenType.EOF);
                    break;

                case TokenType.Function or TokenType.Method or TokenType.Constructor:
                    do
                    {
                        ParseSubroutine();
                        NextToken();
                    } while (CurrentToken.Type != TokenType.RightCurlyBracket && CurrentToken.Type != TokenType.EOF);
                    break;

                default: throw new JackLexerException($"A class needs to have variables and/or subroutines at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void ParseSubroutine()
        {
            var subRoutineNode = CreateNode("subroutineDec");
            _classNode.AppendChild(subRoutineNode);

            ParseSubroutineType(subRoutineNode);
            NextToken();
            ParseReturnType(subRoutineNode);
            NextToken();
            ParseSubroutineName();
            NextToken();
            ParseSymbol(subRoutineNode, "(");
            NextToken();
            if (CurrentToken.Type != TokenType.RightParenthesis)
            {
                ParseParameterList();
            }
            else
            {
                ParseSymbol(subRoutineNode, ")");
            }

            do
            {
                NextToken();
                ParseSymbol(subRoutineNode, "{");
                NextToken();
                var subroutineBodyNode = CreateNode("subroutineBody");
                _classNode.AppendChild(subRoutineNode);
                subRoutineNode.AppendChild(subroutineBodyNode);
                ParseSubroutineBody(subroutineBodyNode);
                subRoutineNode.AppendChild(subroutineBodyNode);

                NextToken();
                ParseSymbol(subRoutineNode, "}");
                NextToken();
            } while (CurrentToken.Type != TokenType.Function &&
                     CurrentToken.Type != TokenType.Constructor &&
                     CurrentToken.Type != TokenType.Method &&
                     CurrentToken.Type != TokenType.EOF);

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

                ParseSymbol(subRoutineNode, ")");
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
        }

        void ParseSubroutineType(XmlNode parentNode)
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Function:
                    ParseKeyword(parentNode, "function");
                    break;

                case TokenType.Method:
                    ParseKeyword(parentNode, "method");
                    break;

                case TokenType.Constructor:
                    ParseKeyword(parentNode, "constructor");
                    break;

                default:
                    throw new JackAnalyzerException($"Subroutine type expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void ParseReturnType(XmlNode parentNode)
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Void:
                    ParseKeyword(parentNode, "void");
                    break;

                case TokenType.Identifier:
                    ParseIdentifier(parentNode);
                    break;

                case TokenType.Int:
                    ParseKeyword(parentNode, "int");
                    break;

                case TokenType.Boolean:
                    ParseKeyword(parentNode, "boolean");
                    break;

                case TokenType.Char:
                    ParseKeyword(parentNode, "char");
                    break;

                default:
                    throw new JackAnalyzerException($"Return type expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
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
            var returnStatementNode = CreateNode("doStatement");
            parentNode.AppendChild(returnStatementNode);
            ParseKeyword(returnStatementNode, "return");

            Debugger.Break();
        }

        void ParseDoStatement(XmlNode parentNode)
        {
            var doStatementNode = CreateNode("doStatement");
            parentNode.AppendChild(doStatementNode);
            ParseKeyword(doStatementNode, "do");

            Debugger.Break();
        }

        void ParseWhileStatement(XmlNode parentNode)
        {
            var whileStatementNode = CreateNode("whileStatement");
            parentNode.AppendChild(whileStatementNode);
            ParseKeyword(whileStatementNode, "while");
            NextToken();
            ParseSymbol(whileStatementNode, "(");
            NextToken();
            ParseExpression(whileStatementNode);
            NextToken();
            ParseSymbol(whileStatementNode, ")");
            NextToken();
            ParseSymbol(whileStatementNode, "{");
            NextToken();

            while (CurrentToken.Type is TokenType.Let or
                   TokenType.Do or
                   TokenType.If or
                   TokenType.Else or
                   TokenType.While or
                   TokenType.Return)
            {
                ParseStatements(parentNode: whileStatementNode);
                NextToken();
            }

            ParseSymbol(whileStatementNode, "}");
        }

        void ParseIfStatement(XmlNode parentNode)
        {
            var ifStatementNode = CreateNode("ifStatement");
            parentNode.AppendChild(ifStatementNode);
            ParseKeyword(ifStatementNode, "if");

            Debugger.Break();
        }

        void ParseLetStatement(XmlNode parentNode)
        {
            var letStatementNode = CreateNode("letStatement");
            parentNode.AppendChild(letStatementNode);
            ParseKeyword(letStatementNode, "let");
            NextToken();
            ParseIdentifier(letStatementNode); // varName
            NextToken();

            if (CurrentToken.Type == TokenType.Equal)
            {
                ParseSymbol(letStatementNode, "=");
                NextToken();
                ParseExpression(letStatementNode);
            }
            else
            {
                ParseArrayExpression(letStatementNode);
                NextToken();
                ParseSymbol(letStatementNode, "=");
                NextToken();
                ParseExpression(letStatementNode);
            }

            NextToken();
            ParseSemicolon(letStatementNode);
        }

        void ParseExpression(XmlNode parentNode)
        {
            var expressionNode = CreateNode("expression");
            parentNode.AppendChild(expressionNode);
            ParseExpressionTerm(expressionNode);

            var isOp = CheckForOperator(expressionNode);
            while (isOp)
            {
                NextToken();
                ParseExpressionTerm(expressionNode);
                isOp = CheckForOperator(expressionNode);
            }
        }

        bool CheckForOperator(XmlNode parentNode)
        {
            NextToken();
            if (CurrentToken.Type is TokenType.Equal or TokenType.LesserThan or TokenType.GreaterThan or TokenType.And or TokenType.Or or TokenType.Plus or TokenType.Minus)
            {
                ParseSymbol(parentNode, CurrentToken.Value);
                return true;
            }
            else
            {
                PreviousToken();
                return false;
            }
        }

        void ParseClassVariableDeclarations()
        {
            var classVarDecNode = CreateNode("classVarDec");
            _classNode.AppendChild(classVarDecNode);
            ParseStaticOrField();
            NextToken();
            ParseType(classVarDecNode);
            NextToken();
            do
            {
                if (CurrentToken.Type == TokenType.Comma)
                {
                    ParseSymbol(classVarDecNode, ",");
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
                        ParseKeyword(classVarDecNode, "field");
                        break;

                    case TokenType.Static:
                        ParseKeyword(classVarDecNode, "static");
                        break;
                }
            }
        }

        void ParseExpressionTerm(XmlNode parentNode)
        {
            var termNode = CreateNode("term");
            parentNode.AppendChild(termNode);

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

        void ParseVariableName(XmlNode parentNode)
        {
            NextToken(); // look one step ahead to see the kind of expression we are dealing with. We need to go back before continuing parsing
            switch (CurrentToken.Type)
            {
                case TokenType.LeftSquareBracket:
                    ParseArrayExpression(parentNode);
                    break;

                case TokenType.Period:
                    ParseSubroutineCallOnAnObject(parentNode);
                    break;

                case TokenType.LeftParenthesis:
                    ParseSubroutineCall();
                    break;

                default:
                    ParseVariable(parentNode);
                    break;
            }
        }

        void ParseVariable(XmlNode parentNode)
        {
            PreviousToken();
            ParseIdentifier(parentNode: parentNode);
        }

        void ParseSubroutineCallOnAnObject(XmlNode parentNode)
        {
            PreviousToken();
            ParseIdentifier(parentNode: parentNode);
            NextToken();
            ParseSymbol(parentNode, ".");
            NextToken();
            ParseIdentifier(parentNode);
            NextToken();
            ParseSymbol(parentNode, "(");
            NextToken();
            var expressionListNode = CreateNode("expressionList");
            parentNode.AppendChild(expressionListNode);

            if (CurrentToken.Type != TokenType.RightParenthesis)
            {
                do
                {
                    ParseExpression(expressionListNode);
                    NextToken();
                } while (CurrentToken.Type == TokenType.Comma);
            }

            ParseSymbol(parentNode, ")");
        }

        void ParseSubroutineCall()
        {
            Debugger.Break();
        }

        void ParseSymbol(XmlNode parentNode, string symbol)
        {
            if (CurrentToken.Value == symbol)
            {
                var symbolNode = CreateNode("symbol");
                symbolNode.InnerText = symbol;
                parentNode.AppendChild(symbolNode);
            }
            else
            {
                throw new JackAnalyzerException($"Symbol '{symbol}' expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void ParseKeyword(XmlNode parentNode, string keyword)
        {
            if (CurrentToken.Value == keyword)
            {
                var keywordNode = CreateNode("keyword");
                keywordNode.InnerText = keyword;
                parentNode.AppendChild(keywordNode);
            }
            else
            {
                throw new JackAnalyzerException($"Keyword '{keyword}' expected at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
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

        void ParseIdentifier(XmlNode parentNode)
        {
            if (CurrentToken.Type == TokenType.Identifier)
            {
                var identifierNode = CreateNode("identifier");
                identifierNode.InnerText = CurrentToken.Value;
                parentNode.AppendChild(identifierNode);
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

        void ParseSubroutineBody(XmlNode parentNode)
        {
            var varDecNode = CreateNode("varDec");
            parentNode.AppendChild(varDecNode);
            while (CurrentToken.Type == TokenType.Var)
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
                        NextToken();
                    }

                    ParseIdentifier(varDecNode);
                    NextToken();
                } while (CurrentToken.Type == TokenType.Comma);

                ParseSemicolon(varDecNode);
                NextToken();
            }

            var statementsNode = CreateNode("statements");
            parentNode.AppendChild(statementsNode);

            while (CurrentToken.Type is TokenType.Let or
                   TokenType.Do or
                   TokenType.If or
                   TokenType.Else or
                   TokenType.While or
                   TokenType.Return)
            {
                ParseStatements(parentNode: statementsNode);
                NextToken();
            }
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

        void ParseArrayExpression(XmlNode parentNode)
        {
            ParseSymbol(parentNode, "[");
            NextToken();
            ParseExpression(parentNode);
            NextToken();
            ParseSymbol(parentNode, "]");
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