using MailChemist.Core.Attributes;
using MailChemist.UnitTests.Interfaces;
using System.Runtime.CompilerServices;

namespace MailChemist.UnitTests.Models;

[MailChemistModel]
public class DefaultModel : IDefaultModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DefaultModel(string firstName, [CallerMemberName] string lastName = null)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}