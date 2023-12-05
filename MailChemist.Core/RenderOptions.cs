namespace MailChemist.Core;

public class RenderOptions
{
    public MailChemistMjmlRenderer MailChemistMjmlRenderer { get; set; } = MailChemistMjmlRenderer.MjmlDotNet;

    public string ModelName { get; set; } = "Model";
}