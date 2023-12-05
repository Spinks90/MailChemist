using Newtonsoft.Json;

namespace MailChemist.Entities;

internal sealed class MjmlRenderResponseError
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