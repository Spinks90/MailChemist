using Fluid;
using MailChemist.Core.Attributes;
using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;
using MailChemist.Providers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MailChemist
{
    /// <summary>
    /// 
    /// </summary>
    public class MailChemist : IMailChemist
    {
        private readonly FluidParser _fluidParser = new FluidParser();
        private readonly IEmailContentProvider _emailContentProvider;

        private static List<Type> _globalTypes = new List<Type>();
        private static FilterCollection _globalFilters = new FilterCollection();

        private string _result = string.Empty;

        /// <summary>
        /// If you don't require functionality of <see cref="IEmailContentProvider" />. Otherwise, use one of the other constructors.
        /// </summary>
        public MailChemist()
        {
            _emailContentProvider = new EmptyEmailContentProvider();
        }

        /// <summary>
        /// Use MJML as the <see cref="IEmailContentProvider" />
        /// </summary>
        /// <param name="mjmlApiApplicationId"></param>
        /// <param name="mjmlApiSecretKey"></param>
        /// <param name="mjmlApiUrl">Use this to mock or change to future version if the scheme hasn't changed</param>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public MailChemist(string mjmlApiApplicationId,
                            string mjmlApiSecretKey,
                            string mjmlApiUrl = "https://api.mjml.io/v1")
        {
            var baseUri = new Uri(mjmlApiUrl);
            _emailContentProvider = new MjmlEmailContentProvider(baseUri, mjmlApiApplicationId, mjmlApiSecretKey);
        }

        /// <summary> 
        /// Implement a custom <see cref="IEmailContentProvider" />
        /// </summary>
        /// <param name="emailContentProvider"></param>
        /// <exception cref="ArgumentNullException"><paramref name="emailContentProvider"/> is null</exception>
        public MailChemist(IEmailContentProvider emailContentProvider)
        {
            _emailContentProvider = emailContentProvider ?? throw new ArgumentNullException(nameof(emailContentProvider));
        }

        /// <summary>
        /// Uses reflection to automatically register any <see cref="MailChemistModelAttribute" /> types.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AppDomainUnloadedException"></exception>
        public void RegisterGlobalTypesByAttribute()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                         .Where(m => m.GetCustomAttributes(typeof(MailChemistModelAttribute), false).Length > 0)
                         .ToList();
        }

        public void RegisterGlobalType(Type type)
        {

        }

        /// <summary>
        /// Allows you to use MailChemist defined filters. 
        /// </summary>
        public void RegisterGlobalFilter(string name, FilterDelegate filterDelegate)
        {
            _globalFilters.AddFilter(name, filterDelegate);
        }

        /// <summary>
        /// Executes the <see cref="IEmailContentProvider.GenerateEmailContent" /> then executes <see cref="TryGenerateFluid" /> using
        /// <see cref="CultureInfo.CurrentCulture" /> as the CultureInfo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="model"></param>
        /// <param name="registerType">If you have not registered the type, set this flag to true</param>
        /// <param name="result"></param>
        /// <param name="error">Error with getting the content or generating the fluid</param>
        /// <returns></returns>
        public bool TryGenerate<T>(string content,
                                  T model,
                                  out string result,
                                  out string error,
                                  bool registerType = false) where T : class
        {
            result = string.Empty;
            error = string.Empty;

            EmailContentData contentResult = new EmailContentData();

            try
            {
                contentResult = _emailContentProvider.GenerateEmailContent(content);

                if (string.IsNullOrEmpty(contentResult.Error) == false)
                    error = contentResult.Error;
            }
            catch(Exception ex)
            {
                error = ex.Message;
            }
            
            if(string.IsNullOrEmpty(error) == false)
                return false;

            if(TryGenerateFluid(contentResult.Content, model, out var fluidResult, out var fluidError, registerType: registerType) == false)
            {
                result = string.Empty;
                error = fluidError;
                return false;
            }

            result = fluidResult;

            return true;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fluid"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        /// <param name="error"></param>
        /// <param name="modelName"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="registerType"></param>
        /// <returns></returns>
        public bool TryGenerateFluid<T>(string fluid,
                                      T model,
                                      out string result,
                                      out string error,
                                      string modelName = "Model",
                                      CultureInfo cultureInfo = null,
                                      bool registerType = false) where T : class
        {
            error = string.Empty;
            result = string.Empty;

            var options = new TemplateOptions();

            options.CultureInfo = cultureInfo is null ? CultureInfo.CurrentCulture : cultureInfo;

            foreach (var filter in _globalFilters)
                options.Filters.AddFilter(filter.Key, filter.Value);

            if (registerType)
                RegisterTypes(options, model);

            if (_fluidParser.TryParse(fluid, out var template, out var fluidError) == false)
            {
                error = fluidError;
                return false;
            }

            var context = new TemplateContext(model, options);

            context.SetValue(modelName, model);

            result = template.Render(context);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGenerateContent(string content, out string result)
        {
            result = string.Empty;

            EmailContentData retVal;
         
            try
            {
                retVal = _emailContentProvider.GenerateEmailContent(content);
            }
            catch
            {
                return false;
            }

            if(string.IsNullOrEmpty(retVal.Error) == false)
                return false;

            result = retVal.Content ?? string.Empty;

            return true;
        }

        public string GenerateFluid<T>(string fluid,
                                       T model,
                                       string modelName = "Model",
                                       CultureInfo cultureInfo = null,
                                       bool registerType = false) where T : class
        {


            var template = _fluidParser.Parse(fluid);

            var options = new TemplateOptions();

            foreach (var filter in _globalFilters)
                options.Filters.AddFilter(filter.Key, filter.Value);

            if (registerType)
                RegisterTypes(options, model);

            options.CultureInfo = cultureInfo is null ? CultureInfo.CurrentCulture : cultureInfo;

            var context = new TemplateContext(model, options);

            context.SetValue(modelName, model);

            return template.Render(context);
        }

        private void RegisterTypes<T>(TemplateOptions templateOptions, T t)
        {
            var nestedTypes = typeof(T)
                .GetProperties()
                .Select(s => s.PropertyType)
                .Where(m => m.GetCustomAttributes(typeof(MailChemistModelAttribute), false).Any())
                .ToList();

            foreach (var nestedType in nestedTypes)
                templateOptions.MemberAccessStrategy.Register(nestedType);

            templateOptions.MemberAccessStrategy.Register<T>();
        }

        public string GenerateContent(string content)
        {
            return _emailContentProvider.GenerateEmailContent(content).Content ?? string.Empty;        
        }
    }
}