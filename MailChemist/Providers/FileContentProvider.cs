using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;
using System;
using System.IO;

namespace MailChemist.Providers;

public class FileContentProvider : IContentProvider
{
    private readonly string _baseTemplateDirectory;

    public FileContentProvider()
    {
        _baseTemplateDirectory = string.Empty;
    }

    public FileContentProvider(string baseTemplateDirectory)
    {
        _baseTemplateDirectory = baseTemplateDirectory ?? throw new ArgumentNullException(nameof(baseTemplateDirectory));
    }

    public MjmlContentData GetContent(string baseTemplateDirectory)
    {
        string templatePath;

        if(string.IsNullOrWhiteSpace(_baseTemplateDirectory))
            templatePath = baseTemplateDirectory;
        else
            templatePath = Path.Combine(_baseTemplateDirectory, baseTemplateDirectory);

        var emailContentData = new MjmlContentData
        {
            Success = true,
            Content = File.ReadAllText(templatePath)
        };

        return emailContentData;
    }

//#if NETSTANDARD2_1
//            public async Task<EmailContentData> GenerateEmailContentAsync(string baseTemplateDirectory, CancellationToken cancellationToken = default)
//            {
//                string templatePath = string.IsNullOrWhiteSpace(_baseTemplateDirectory) ? baseTemplateDirectory : Path.Combine(_baseTemplateDirectory, baseTemplateDirectory);

//                var emailContentData = new EmailContentData();
//                emailContentData.Content = await File.ReadAllTextAsync(templatePath, cancellationToken);
 
//                return emailContentData;
//            }
//#endif
}