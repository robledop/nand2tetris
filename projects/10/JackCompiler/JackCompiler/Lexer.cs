using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JackCompiler.Exceptions;

namespace JackCompiler
{
    public class Lexer
    {
        readonly string _source;
        Marker _sourceMarker;
        Marker _tokenMarker;
        char _lastChar;
        string _lastToken;

        public Lexer(string source)
        {
            _source = StripComments(source);
            _sourceMarker = new Marker(0, 1, 1);
            _lastChar = _source.First();
        }

        static string StripComments(string source)
        {
            var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
            return Regex.Replace(source, re, "$1");
        }

        char GetChar()
        {
            _sourceMarker.Column++;
            _sourceMarker.Pointer++;

            if (_sourceMarker.Pointer >= _source.Length)
            {
                return _lastChar = (char)0;
            }

            _lastChar = _source[_sourceMarker.Pointer];
            if (_lastChar == '\n')
            {
                _sourceMarker.Column = 1;
                _sourceMarker.Line++;
            }
            return _lastChar;
        }

        public Token GetToken()
        {
            var sb = new StringBuilder();

            while (_lastChar is ' ' or '\t' or '\r' or '\n')
            {
                GetChar();
            }

            _tokenMarker = _sourceMarker;

            if (char.IsLetter(_lastChar) || _lastChar == '_')
            {
                _lastToken = _lastChar.ToString();
                GetChar();
                while (char.IsLetterOrDigit(_lastChar) || _lastChar == '_')
                {
                    sb.Append(_lastChar);
                    GetChar();
                }
                _lastToken += sb.ToString();

                return GetKeywordOrIdentifier();
            }

            if (char.IsDigit(_lastChar))
            {
                return GetIntegerConstant(sb);
            }

            // Get symbol
            switch (_lastChar)
            {
                case '{': GetChar(); return new Token("{", TokenType.LeftCurlyBracket, _tokenMarker);
                case '}': GetChar(); return new Token("}", TokenType.RightCurlyBracket, _tokenMarker);
                case '(': GetChar(); return new Token("(", TokenType.LeftParenthesis, _tokenMarker);
                case ')': GetChar(); return new Token(")", TokenType.RightParenthesis, _tokenMarker);
                case '[': GetChar(); return new Token("[", TokenType.LeftSquareBracket, _tokenMarker);
                case ']': GetChar(); return new Token("]", TokenType.RightSquareBracket, _tokenMarker);
                case '.': GetChar(); return new Token(".", TokenType.Period, _tokenMarker);
                case ',': GetChar(); return new Token(",", TokenType.Comma, _tokenMarker);
                case ';': GetChar(); return new Token(";", TokenType.SemiColon, _tokenMarker);
                case '+': GetChar(); return new Token("+", TokenType.Plus, _tokenMarker);
                case '-': GetChar(); return new Token("-", TokenType.Minus, _tokenMarker);
                case '*': GetChar(); return new Token("*", TokenType.Multiply, _tokenMarker);
                case '/': GetChar(); return new Token("/", TokenType.Divide, _tokenMarker);
                case '&': GetChar(); return new Token("&", TokenType.And, _tokenMarker);
                case '|': GetChar(); return new Token("|", TokenType.Or, _tokenMarker);
                case '<': GetChar(); return new Token("<", TokenType.LesserThan, _tokenMarker);
                case '>': GetChar(); return new Token(">", TokenType.GreaterThan, _tokenMarker);
                case '=': GetChar(); return new Token("=", TokenType.Equal, _tokenMarker);
                case '~': GetChar(); return new Token("~", TokenType.Not, _tokenMarker);
            }

            if (_lastChar == '"')
            {
                return GetStringConstant(sb);
            }

            if (_sourceMarker.Pointer >= _source.Length)
            {
                return new Token(null, TokenType.EOF, _tokenMarker);
            }

            throw new JackLexerException($"Token not recognized at line {_sourceMarker.Line}, position {_sourceMarker.Column}");
        }

        Token GetStringConstant(StringBuilder sb)
        {
            sb.Clear();

            do
            {
                if (_lastChar != '"')
                {
                    sb.Append(_lastChar);
                }

                GetChar();
            } while (_lastChar != '"');

            GetChar();
            return new Token(sb.ToString(), TokenType.StringConstant, _tokenMarker);
        }

        Token GetIntegerConstant(StringBuilder sb)
        {
            sb.Clear();
            do
            {
                sb.Append(_lastChar);
            } while (char.IsDigit(GetChar()));

            var number = sb.ToString();

            if (!int.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out int integer))
            {
                throw new JackLexerException($"ERROR while parsing number on line {_tokenMarker.Line}, position {_tokenMarker.Column}");
            }

            return new Token(integer.ToString(), TokenType.IntegerConstant, _tokenMarker);
        }

        Token GetKeywordOrIdentifier()
        {
            return _lastToken switch
            {
                "class" => new Token("class", TokenType.Class, _tokenMarker),
                "constructor" => new Token("constructor", TokenType.Constructor, _tokenMarker),
                "function" => new Token("function", TokenType.Function, _tokenMarker),
                "method" => new Token("method", TokenType.Method, _tokenMarker),
                "field" => new Token("field", TokenType.Field, _tokenMarker),
                "static" => new Token("static", TokenType.Static, _tokenMarker),
                "var" => new Token("var", TokenType.Var, _tokenMarker),
                "int" => new Token("int", TokenType.Int, _tokenMarker),
                "char" => new Token("char", TokenType.Char, _tokenMarker),
                "boolean" => new Token("boolean", TokenType.Boolean, _tokenMarker),
                "void" => new Token("void", TokenType.Void, _tokenMarker),
                "true" => new Token("true", TokenType.True, _tokenMarker),
                "false" => new Token("false", TokenType.False, _tokenMarker),
                "this" => new Token("this", TokenType.This, _tokenMarker),
                "let" => new Token("let", TokenType.Let, _tokenMarker),
                "do" => new Token("do", TokenType.Do, _tokenMarker),
                "if" => new Token("if", TokenType.If, _tokenMarker),
                "else" => new Token("else", TokenType.Else, _tokenMarker),
                "while" => new Token("while", TokenType.While, _tokenMarker),
                "return" => new Token("return", TokenType.Return, _tokenMarker),
                "null" => new Token("null", TokenType.Null, _tokenMarker),
                _ => new Token(_lastToken, TokenType.Identifier, _tokenMarker)
            };
        }
    }
}
