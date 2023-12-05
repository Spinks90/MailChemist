using MailChemist.Core.Attributes;

namespace MailChemist.UnitTests.Models;

[MailChemistModel]
public class PersonX
{
    public int Id { get; set; }

    public string Name { get; set; }
}