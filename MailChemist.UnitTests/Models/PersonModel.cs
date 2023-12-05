using MailChemist.Core.Attributes;
using System.Runtime.CompilerServices;

namespace MailChemist.UnitTests.Models;

[MailChemistModel]
public class PersonModel : DefaultModel
{

    public PersonModel(string firstName, [CallerMemberName] string lastName = null) : base(firstName, lastName)
    {
    }
}