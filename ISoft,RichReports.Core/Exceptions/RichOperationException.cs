using System;

namespace ISoft.RichReports.Core.Exceptions
{
    public class RichOperationException : Exception
    {
        public RichOperationException(string message, Exception inner) : base(message, inner) { }
    }
}
