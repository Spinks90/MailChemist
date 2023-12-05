using MailChemist.Core.Attributes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MailChemist.UnitTests.Models;

/// <summary>
/// PS: An example of IEnumerable<> and 
/// </summary>
[MailChemistModel]
public class PersonsModel : DefaultModel
{
    public IEnumerable<Person> EnumPeople { get; set; }

    public List<Person> ListPeople { get; set; }

    public IList<Person> IListPeople { get; set; }

    public PersonX PersonX { get; set; }

    public PersonsModel([CallerMemberName] string name = null) : base("", "")
    {
    }
}