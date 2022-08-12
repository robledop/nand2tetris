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

                NextToken();

                //while (CurrentToken.Type is not (TokenType.RightCurlyBracket or TokenType.EOF))
                //{
                    AnalyzeClassMembers();
                //}

               


                NextToken();

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
            else
            {
                throw new JackAnalyzerException($"The file must start with the 'class' token at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void AnalyzeClassMembers()
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Function or TokenType.Method or TokenType.Constructor:
                    AnalyzeSubroutine();
                    break;
                case TokenType.Field or TokenType.Static:
                    AnalyzeClassVariableDeclarations();
                    break;
                default: throw new JackLexerException($"A class needs to have variables and/or subroutines at line {CurrentToken.Marker.Line}, position {CurrentToken.Marker.Column}");
            }
        }

        void AnalyzeSubroutine()
        {
            XmlNode subRoutineNode;
            XmlNode keywordNode;
            switch (CurrentToken.Type)
            {
                case TokenType.Function:
                    subRoutineNode = CreateNode("subroutineDec");
                    keywordNode = CreateNode("keyword");
                    keywordNode.InnerText = "function";
                    subRoutineNode.AppendChild(keywordNode);
                    _classNode.AppendChild(subRoutineNode);
                    break;

                case TokenType.Method:
                    subRoutineNode = CreateNode("subroutineDec");
                    keywordNode = CreateNode("keyword");
                    keywordNode.InnerText = "method";
                    subRoutineNode.AppendChild(keywordNode);
                    _classNode.AppendChild(subRoutineNode);
                    break;

                case TokenType.Constructor:
                    subRoutineNode = CreateNode("subroutineDec");
                    keywordNode = CreateNode("keyword");
                    keywordNode.InnerText = "constructor";
                    subRoutineNode.AppendChild(keywordNode);
                    _classNode.AppendChild(subRoutineNode);
                    break;
            }
        }

        void AnalyzeClassVariableDeclarations()
        {

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
    }
}