using Fluid;
using MailChemist.Core;
using MailChemist.Core.Attributes;
using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;
using MailChemist.Entities;
using MailChemist.Exceptions;
using MailChemist.Providers;
using Mjml.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace MailChemist;

/// <summary>
/// 
/// </summary>
public class MailChemistEngine : IMailChemistEngine
{
    private readonly FluidParser _fluidParser = new FluidParser();
    private readonly MailChemistMjmlRenderer _mailChemistMjmlRenderer;
    private readonly HashSet<Type> _globalTypes = new HashSet<Type>();
    private readonly FilterCollection _globalFilters = new FilterCollection();
    private readonly IContentProvider _contentProvider;

    /// <summary>
    /// If you don't require functionality of <see cref="IContentProvider" />. Otherwise, use one of the other constructors.
    /// </summary>
    public MailChemistEngine(MailChemistMjmlRenderer mailChemistEngine)
    {
        _contentProvider = new MemoryContentProvider();
        _mailChemistMjmlRenderer = mailChemistEngine;

        // PS: Prevent 
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MaxDepth = 10 };
    }


    /// <summary> 
    /// Implement a custom <see cref="IContentProvider" />
    /// </summary>
    /// <param name="contentProvider"></param>
    /// <param name="mailChemistEngine"></param>
    /// <exception cref="ArgumentNullException"><paramref name="contentProvider"/> is null</exception>
    public MailChemistEngine(MailChemistMjmlRenderer mailChemistEngine, IContentProvider contentProvider)
    {
        _contentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
        _mailChemistMjmlRenderer = mailChemistEngine;
    }

    /// <summary>
    /// Uses reflection to automatically register any <see cref="MailChemistModelAttribute" /> types.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="AppDomainUnloadedException"></exception>
    public void RegisterGlobalTypesByAttribute()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetExportedTypes())
                     .Where(m => m.GetCustomAttributes(typeof(MailChemistModelAttribute), false).Length > 0)
                     .ToList();

        _globalTypes.UnionWith(types);
    }

    /// <summary>
    /// Allows you to register a type to be used in the fluid template.
    /// </summary>
    /// <param name="type"></param>
    public void RegisterGlobalType(Type type)
    {
        _globalTypes.Add(type);
    }

    /// <summary>
    /// Allows you to use MailChemist defined filters. 
    /// </summary>
    public void RegisterGlobalFilter(string name, FilterDelegate filterDelegate)
    {
        _globalFilters.AddFilter(name, filterDelegate);
    }

    /// <summary>
    /// Executes the provided content provider then renders the fluid and then renders the MJML using the provided engine.
    /// <see cref="CultureInfo.CurrentCulture" /> as the CultureInfo.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="model"></param>
    /// <param name="options"></param>
    /// <param name="registerType">If you have not registered the type, set this flag to true</param>
    /// <returns>
    /// 
    /// </returns>
    /// <exception cref="MailChemistGetContentException">The IContentProvider threw an error. Read the InnerException</exception>
    /// <exception cref="ArgumentOutOfRangeException">The provided MailChemistEngine doesn't exist</exception>
    public MjmlResult Render<T>(string content,
                                  T model,
                                  RenderOptions options = null,
                                  bool registerType = false) where T : class
    {
        var mcEngine = options is null ? _mailChemistMjmlRenderer : options.MailChemistMjmlRenderer;

        var mjmlContent = GetContent(content);

        if (!mjmlContent.Success)
            return CreateFailedMjmlResult(mjmlContent.Error);

        // PS:
        var mjmlResult = RenderFluid(mjmlContent.Content, model, registerType: registerType);

        if (!mjmlResult.Success)
            return CreateFailedMjmlResult(mjmlResult.Error);

        // PS: 
        return RenderMjmlEngine(mcEngine, mjmlResult.Html);
    }

    public MjmlResult RenderMjmlEngine(MailChemistMjmlRenderer mcEngine, string mjml)
    {
        var mjmlResult = new MjmlResult();

        switch (mcEngine)
        {
            case MailChemistMjmlRenderer.MjmlDotNet:
                var renderResult = GenerateMjmlDotNet(mjml);

                if (renderResult.Errors.Count > 0)
                {
                    mjmlResult.Success = false;
                    // mjmlResult.Error = renderResult.Errors[0].Message;
                    return mjmlResult;
                }

                mjmlResult.Html = renderResult.Html;
                mjmlResult.Success = true;

                break;
            case MailChemistMjmlRenderer.MjmlApi:
                var apiResult = GenerateMjmlApi(mjml);

                mjmlResult.Success = apiResult.Success;
                mjmlResult.Html = apiResult.Content;

                break;
            case MailChemistMjmlRenderer.Html:
                // Skip MJML Engine and return the HTML
                mjmlResult.Html = mjml;
                mjmlResult.Success = true;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mcEngine), mcEngine, "Unknown MailChemist engine");
        }

        return mjmlResult;
    }

    private MjmlContentData GenerateMjmlApi(string mjml)
    {
        var _baseUri = new Uri("https://api.mjml.io/v1");
        var _httpClient = new HttpClient();

        var mjmlContentData = new MjmlContentData();

        // Prepare the request
        var mjmlRequest = new MjmlRenderRequest
        {
            mjml = mjml
        };

        var renderMjmlPayload = JsonConvert.SerializeObject(mjmlRequest);

        var webRequest = new HttpRequestMessage(HttpMethod.Post, _baseUri)
        {
            Content = new StringContent(renderMjmlPayload, Encoding.UTF8, "application/json")
        };

        var response = _httpClient.Send(webRequest);

        using var reader = new StreamReader(response.Content.ReadAsStream());
        string result = reader.ReadToEnd();

        if (response.IsSuccessStatusCode)
        {
            mjmlContentData.Success = true;
            mjmlContentData.Content = result;
            return mjmlContentData;
        }

        mjmlContentData.Error = "Failed to render MJML content";

        return mjmlContentData;
    }

    public RenderResult GenerateMjmlDotNet(string mjml)
    {
        var mjmlRenderer = new MjmlRenderer();

        var options = new MjmlOptions
        {
            Beautify = false
        };

        return mjmlRenderer.Render(mjml, options);
    }

    private MjmlResult CreateFailedMjmlResult(string errorMessage)
    {
        return new MjmlResult { Success = false, Error = errorMessage };
    }

    public MjmlContentData GetContent(string content)
    {
        try
        {
            return _contentProvider.GetContent(content);
        }
        catch (Exception ex)
        {
            throw new MailChemistGetContentException("Failed to get MJML content", ex);
        }
    }

    private void RegisterGlobalTypesToTemplate(TemplateOptions templateOptions)
    {
        if (_globalTypes.Count > 0)
        {
            foreach (var type in _globalTypes)
                templateOptions.MemberAccessStrategy.Register(type);
        }
    }

    private void RegisterGlobalFiltersToTemplate(TemplateOptions templateOptions)
    {
        if (_globalFilters.Count > 0)
        {
            foreach (var filter in _globalFilters)
                templateOptions.Filters.AddFilter(filter.Key, filter.Value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fluid"></param>
    /// <param name="model"></param>
    /// <param name="result"></param>
    /// <param name="error"></param>
    /// <param name="modelName"></param>
    /// <param name="cultureInfo"></param>
    /// <param name="registerType"></param>
    /// <returns>
    /// 
    /// </returns>
    public MjmlResult RenderFluid<T>(string fluid,
                                  T model,
                                  string modelName = "Model",
                                  CultureInfo cultureInfo = null,
                                  bool registerType = false) where T : class
    {
        var mjmlResult = new MjmlResult();

        var options = new TemplateOptions();

        options.CultureInfo = cultureInfo is null ? CultureInfo.CurrentCulture : cultureInfo;

        RegisterGlobalTypesToTemplate(options);
        RegisterGlobalFiltersToTemplate(options);

        if (registerType)
            RegisterTypes<T>(options);

        if (_fluidParser.TryParse(fluid, out var template, out var fluidError) == false)
        {
            mjmlResult.Error = fluidError;
            return mjmlResult;
        }

        var context = new TemplateContext(model, options);
        context.SetValue(modelName, model);

        var result = template.Render(context);

        mjmlResult.Success = true;
        mjmlResult.Html = result;

        return mjmlResult;
    }

    private void RegisterTypes<T>(TemplateOptions templateOptions) where T : class
    {
        var types = typeof(T)
                        .GetProperties()
                        .SelectMany(GetTypesFromProperty)
                        .Distinct()
                        .ToList();

        foreach (var type in types)
            templateOptions.MemberAccessStrategy.Register(type);

        templateOptions.MemberAccessStrategy.Register<T>();
    }

    private static IEnumerable<Type> GetTypesFromProperty(PropertyInfo propInfo)
    {
        Type propType = propInfo.PropertyType;

        // If it's a generic type, extract the generic type arguments that have the MailChemistModelAttribute.
        if (propType.IsGenericType)
        {
            foreach (var arg in propType.GetGenericArguments())
            {
                if (arg.GetCustomAttributes(typeof(MailChemistModelAttribute), false).Any())
                {
                    yield return arg;
                }
            }
        }
        // For non-generic properties, check for the custom attribute.
        else if (propType.GetCustomAttributes(typeof(MailChemistModelAttribute), false).Any())
        {
            yield return propType;
        }
    }
}