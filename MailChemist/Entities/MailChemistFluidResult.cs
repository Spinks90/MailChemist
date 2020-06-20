using System;
using System.Collections.Generic;
using System.Text;

namespace MailChemist.Entities
{
    public class MailChemistFluidResult
    {
        public string Html { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
