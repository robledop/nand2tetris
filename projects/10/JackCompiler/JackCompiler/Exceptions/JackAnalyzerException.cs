using System;

namespace JackCompiler.Exceptions
{
    public class JackAnalyzerException : Exception
    {
        public JackAnalyzerException() : base()
        {
        }

        public JackAnalyzerException(string message) : base(message)
        {
        }

        public JackAnalyzerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
