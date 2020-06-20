using MailChemist.Core.Entities;

namespace MailChemist.Core.Interfaces
{
    public interface IEmailContentProvider
    {
        EmailContentData GenerateEmailContent(string content);
    }
}