using Newtonsoft.Json;
using System.Collections.Generic;

namespace MailChemist.Entities
{
    internal sealed class MjmlRenderResponse
    {
        [JsonProperty("errors")]
        public IEnumerable<MjmlError> Errors { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("mjml")]
        public string Mjml { get; set; }

        [JsonProperty("mjml_version")]
        public string Version { get; set; }
    }

    internal sealed class MjmlError
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("tagName")]
        public string TagName { get; set; }

        [JsonProperty("formattedMessage")]
        public string FormattedMessage { get; set; }

        [JsonProperty("line")]
        public int LineNumber { get; set; }
    }
}