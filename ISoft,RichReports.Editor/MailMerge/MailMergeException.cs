using System;

namespace ISoft.RichReports.Editor.MailMerge
{
    /// <summary>
    /// Exception for mail merge errors
    /// </summary>
    public class MailMergeException : Exception
    {
        public MailMergeException(string message) : base(message) { }
        public MailMergeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
