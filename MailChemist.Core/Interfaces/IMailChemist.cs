using System.Collections.Generic;
using System.Globalization;

namespace MailChemist.Core.Interfaces
{
    public interface IMailChemist
    {
        bool TryGenerate<T>(string content,
                            T model,
                            out string result,
                            out IList<string> errors,
                            bool registerType = false) where T : class;

        bool TryGenerateFluid<T>(string fluid,
                                T model,
                                out string result,
                                out IList<string> errors,
                                string modelName = "Model",
                                CultureInfo cultureInfo = null,
                                bool registerType = false) where T : class;

        bool TryGenerateContent(string content, out string result);

        
        string GenerateFluid<T>(string fluid,
                                T model,
                                string modelName = "Model",
                                CultureInfo cultureInfo = null,
                                bool registerType = false) where T : class;

        string GenerateContent(string content);
    }
}