using Fluid;
using Fluid.Values;
using MailChemist.Core.Attributes;

namespace MailChemist.Filters
{
    public static class MailChemistFilters
    {
        public static FluidValue IsLessThanZeroAddClass(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var className = arguments.At(0).ToStringValue();

            var d = input.ToNumberValue();

            if (d < 0)
                return new StringValue(className);

            return new StringValue(string.Empty);
        }

        public static FluidValue MoneyWithCurrency(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (decimal.TryParse(input.ToStringValue(), out decimal result) == false)
                return NilValue.Instance;

            return new StringValue(result.ToString("C", context.CultureInfo));
        }
    }
}