using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;

namespace MailChemist.Providers;

public class MemoryContentProvider : IContentProvider
{
    public MjmlContentData GetContent(string content)
    {
        return new MjmlContentData()
        {
            Success = true,
            Content = content,
            Error = string.Empty
        };
    }
}