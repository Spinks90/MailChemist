using MailChemist.Core.Entities;

namespace MailChemist.Core.Interfaces;

public interface IContentProvider
{
    MjmlContentData GetContent(string content);
}