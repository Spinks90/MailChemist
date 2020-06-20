using Fluid;
using MailChemist.Core.Attributes;
using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;
using MailChemist.Extensions;
using MailChemist.Filters;
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
        private readonly IEmailContentProvider _emailContentProvider;

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
        public static bool RegisterGlobalTypes()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                         .Where(m => m.GetCustomAttributes(typeof(MailChemistModelAttribute), false).Length > 0)
                         .ToList();

            if (types == null)
                return false;

            foreach (var type in types)
                TemplateContext.GlobalMemberAccessStrategy.Register(type);

            return true;
        }

        /// <summary>
        /// Allows you to use MailChemist defined filters. 
        /// </summary>
        public static void RegisterGlobalFilters()
        {
            TemplateContext.GlobalFilters.AddFilter("IsLessThanZeroAddClass", MailChemistFilters.IsLessThanZeroAddClass);
            TemplateContext.GlobalFilters.AddFilter("MoneyWithCurrency", MailChemistFilters.MoneyWithCurrency);
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
        /// <param name="errors">Errors with getting the content or generating the fluid</param>
        /// <returns></returns>
        public bool TryGenerate<T>(string content,
                                  T model,
                                  out string result,
                                  out IList<string> errors,
                                  bool registerType = false) where T : class
        {
            errors = new List<string>();

            EmailContentData contentResult = new EmailContentData(); ;

            try
            {
                contentResult = _emailContentProvider.GenerateEmailContent(content);
            }
            catch(Exception ex)
            {
                errors.Add(ex.Message);
            }
            finally
            {
                errors.AddRange(contentResult.Errors ?? Enumerable.Empty<string>());
            }
            
            if(errors.Any())
            {
                result = string.Empty;
                return false;
            }

            if(TryGenerateFluid(contentResult.Content, model, out var fluidResult, out var fluidErrors, registerType: registerType) == false)
            {
                result = string.Empty;
                errors = fluidErrors;
                return false;
            }

            result = fluidResult;
            errors = Array.Empty<string>();

            return true;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fluid"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        /// <param name="errors"></param>
        /// <param name="modelName"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="registerType"></param>
        /// <returns></returns>
        public bool TryGenerateFluid<T>(string fluid,
                                      T model,
                                      out string result,
                                      out IList<string> errors,
                                      string modelName = "Model",
                                      CultureInfo cultureInfo = null,
                                      bool registerType = false) where T : class
        {
            errors = Array.Empty<string>();
            result = string.Empty;

            if (FluidTemplate.TryParse(fluid, out var template, out var fluidErrors) == false)
            {
                errors = new List<string>();
                errors.AddRange(fluidErrors);
                return false;
            }
        
            var context = new TemplateContext();

            if (cultureInfo != null)
                context.CultureInfo = cultureInfo;
            else
                context.CultureInfo = CultureInfo.CurrentCulture;

            if (registerType)
                context.MemberAccessStrategy.Register<T>();

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

            if(retVal.Errors?.Any() ?? false)
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
            var template = FluidTemplate.Parse(fluid);

            var context = new TemplateContext();

            if (cultureInfo != null)
                context.CultureInfo = cultureInfo;
            else
                context.CultureInfo = CultureInfo.CurrentCulture;

            if (registerType)
                context.MemberAccessStrategy.Register<T>();

            context.SetValue(modelName, model);

            return template.Render(context);
        }

        public string GenerateContent(string content)
        {
            return _emailContentProvider.GenerateEmailContent(content).Content ?? string.Empty;
        }
    }
}