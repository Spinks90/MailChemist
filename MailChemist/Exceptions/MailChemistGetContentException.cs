using System;

namespace MailChemist.Exceptions;

internal class MailChemistGetContentException : Exception
{
    public MailChemistGetContentException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
