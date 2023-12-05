using MailChemist.Core.Attributes;

namespace MailChemist.UnitTests.Models;

[MailChemistModel]
public class Person
{
    public int Id { get; set; }

    public string Name { get; set; }
}