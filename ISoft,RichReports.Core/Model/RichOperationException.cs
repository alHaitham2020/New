using System;

namespace ISoft.RichReports.Core.Model
{
    public class RichOperationException : Exception
    {
        public RichOperationException(string message, Exception inner) : base(message, inner) { }
    }
}
