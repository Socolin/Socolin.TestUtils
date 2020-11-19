using System;

namespace Socolin.TestUtils.JsonComparer.Exceptions
{
    public class InvalidJsonException : Exception
    {
        public InvalidJsonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
