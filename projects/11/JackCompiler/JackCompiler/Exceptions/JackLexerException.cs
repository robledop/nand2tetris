using System;
using System.Runtime.Serialization;

namespace JackCompiler.Exceptions
{
    public class JackLexerException : Exception
    {
        public JackLexerException() : base()
        {
        }

        public JackLexerException(string message) : base(message)
        {
        }

        public JackLexerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}