using System.Collections.Generic;

namespace MailChemist.Entities;

public class MailChemistFluidResult
{
    public string Html { get; set; }

    public IEnumerable<string> Errors { get; set; }
}
