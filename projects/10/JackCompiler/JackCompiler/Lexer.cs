using System.Globalization;
using System.Linq;
using System.Text;

namespace JackCompiler
{
    public class Lexer
    {
        readonly string _source;
        Marker _sourceMarker;
        Marker _tokenMarker;
        char _lastChar;
        string _lastToken;
        Constant _constant;

        public Lexer(string source)
        {
            _source = source;
            _sourceMarker = new Marker(0, 1, 1);
            _lastChar = _source.First();


        }

        public void GoTo(Marker marker)
        {
            _sourceMarker = marker;
        }

        public string GetLine(Marker marker)
        {
            var sb = new StringBuilder();
            Marker oldMarker = _sourceMarker;
            marker.Pointer--;
            GoTo(marker);

            do
            {
                sb.Append(GetChar());
            } while (_lastChar != '\n' && _lastChar != (char)0);

            sb.Remove(sb.Length - 1, sb.Length);


            GoTo(oldMarker);

            return sb.ToString();
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

        public TokenType GetToken()
        {
            var sb = new StringBuilder();

            GetChar();

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

                switch (_lastToken)
                {
                    case "class": return TokenType.Class;
                    case "constructor": return TokenType.Constructor;
                    case "function": return TokenType.Function;
                    case "method": return TokenType.Method;
                    case "field": return TokenType.Field;
                    case "static": return TokenType.Static;
                    case "var": return TokenType.Var;
                    case "int": return TokenType.Int;
                    case "char": return TokenType.Char;
                    case "boolean": return TokenType.Boolean;
                    case "void": return TokenType.Void;
                    case "true": return TokenType.True;
                    case "false": return TokenType.False;
                    case "this": return TokenType.This;
                    case "let": return TokenType.Let;
                    case "do": return TokenType.Do;
                    case "if": return TokenType.If;
                    case "else": return TokenType.Else;
                    case "while": return TokenType.While;
                    case "return": return TokenType.Return;

                    default: return TokenType.Identifier;
                }
            }

            if (char.IsDigit(_lastChar))
            {
                sb.Clear();
                do
                {
                    sb.Append(_lastChar); 
                } 
                while (char.IsDigit(GetChar()));

                var number = sb.ToString();

                if (!int.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out int integer))
                {
                    throw new JackLexerException($"ERROR while parsing number on line {_tokenMarker.Line}, position {_tokenMarker.Column}");
                }

                _constant = new Constant(number);
                return TokenType.IntegerConstant;
            }

            switch (_lastChar)
            {
                case '{': return TokenType.LeftCurlyBracket;
                case '}': return TokenType.RightCurlyBracket;
                case '(': return TokenType.LeftParenthesis;
                case ')': return TokenType.RightParenthesis;
                case '[': return TokenType.LeftSquareBracket;
                case ']': return TokenType.RightSquareBracket;
                case '.': return TokenType.Period;
                case ',': return TokenType.Comma;
                case ';': return TokenType.SemiColon;
                case '+': return TokenType.Plus;
                case '-': return TokenType.Minus;
                case '*': return TokenType.Multiply;
                case '/': return TokenType.Divide;
                case '&': return TokenType.And;
                case '|': return TokenType.Or;
                case '<': return TokenType.LesserThan;
                case '>': return TokenType.GreaterThan;
                case '=': return TokenType.Equal;
                case '~': return TokenType.Not;
            }

            if (_lastChar == '"')
            {
                sb.Clear();
                do
                {
                    sb.Append(GetChar());
                } while (_lastChar != '"');

                _constant = new Constant(sb.ToString());
                return TokenType.StringConstant;
            }

            if (_sourceMarker.Pointer >= _source.Length)
            {
                return TokenType.EOF;
            }

            throw new JackLexerException($"Token not recognized at line {_sourceMarker.Line}, position {_sourceMarker.Column}");
        }
    }
}
