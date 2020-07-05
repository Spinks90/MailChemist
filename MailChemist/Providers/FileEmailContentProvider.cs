using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;
using System;
using System.IO;

namespace MailChemist.Providers
{
    public class FileEmailContentProvider : IEmailContentProvider
    {
        private readonly string _baseTemplateDirectory;

        public FileEmailContentProvider()
        {
            _baseTemplateDirectory = string.Empty;
        }

        public FileEmailContentProvider(string baseTemplateDirectory)
        {
            _baseTemplateDirectory = baseTemplateDirectory ?? throw new ArgumentNullException(nameof(baseTemplateDirectory));
        }

        public EmailContentData GenerateEmailContent(string baseTemplateDirectory)
        {
            string templatePath = string.IsNullOrWhiteSpace(_baseTemplateDirectory) ? baseTemplateDirectory : Path.Combine(_baseTemplateDirectory, baseTemplateDirectory);

            var emailContentData = new EmailContentData();
            emailContentData.Content = File.ReadAllText(templatePath);

            return emailContentData;
        }
    }
}