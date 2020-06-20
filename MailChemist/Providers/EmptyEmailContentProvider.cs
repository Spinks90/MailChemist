using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace MailChemist.Providers
{
    public class EmptyEmailContentProvider : IEmailContentProvider
    {
        public EmailContentData GenerateEmailContent(string content)
        {
            throw new NotSupportedException();
        }
    }
}