using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MailChemist.Providers
{
    public class MemoryEmailContentProvider : IEmailContentProvider
    {
        private Dictionary<string, string> _cacheTemplates = new Dictionary<string, string>();

        public EmailContentData GenerateEmailContent(string content)
        {
            throw new NotImplementedException();
        }

        public bool AddCacheTemplateByFile(string fileName)
        {
            return true;
        }
    }
}