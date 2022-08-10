using System;
using System.Runtime.Serialization;

namespace JackCompiler
{
    public class JackLexerException : Exception
    {
        public JackLexerException() : base()
        {
        }

        public JackLexerException(string? message) : base(message)
        {
        }

        public JackLexerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}