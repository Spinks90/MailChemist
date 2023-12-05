using MailChemist.Core.Entities;
using System;
using System.Globalization;

namespace MailChemist.Core.Interfaces;

public interface IMailChemistEngine
{
    MjmlContentData GetContent(string content);

    void RegisterGlobalType(Type type);

    void RegisterGlobalTypesByAttribute();

    MjmlResult Render<T>(string content, T model, RenderOptions options = null, bool registerType = false) where T : class;

    MjmlResult RenderFluid<T>(string fluid, T model, string modelName = "Model", CultureInfo cultureInfo = null, bool registerType = false) where T : class;

    MjmlResult RenderMjmlEngine(MailChemistMjmlRenderer mcEngine, string mjml);
}