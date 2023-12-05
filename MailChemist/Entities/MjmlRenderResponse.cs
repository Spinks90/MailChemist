using Newtonsoft.Json;
using System.Collections.Generic;

namespace MailChemist.Entities;

internal sealed class MjmlRenderResponse
{
    [JsonProperty("errors")]
    public IEnumerable<MjmlRenderResponseError> Errors { get; set; }

    [JsonProperty("html")]
    public string Html { get; set; }

    [JsonProperty("mjml")]
    public string Mjml { get; set; }

    [JsonProperty("mjml_version")]
    public string Version { get; set; }
}
