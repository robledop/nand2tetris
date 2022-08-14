using System;

namespace JackCompiler.Exceptions
{
    public class JackCompilerException : Exception
    {
        public JackCompilerException() : base()
        {
        }

        public JackCompilerException(string message) : base(message)
        {
        }

        public JackCompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}