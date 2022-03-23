using Fluid;
using Fluid.Values;
using System.Threading.Tasks;

namespace MailChemist.Filters
{
    public static class MailChemistFilters
    {
        public static ValueTask<FluidValue> IsLessThanZeroAddClass(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var className = arguments.At(0).ToStringValue();

            var d = input.ToNumberValue();

            if (d < 0)
                return new ValueTask<FluidValue>(new StringValue(className));

            return new ValueTask<FluidValue>(new StringValue(string.Empty));
        }

        public static ValueTask<FluidValue> MoneyWithCurrency(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (decimal.TryParse(input.ToStringValue(), out decimal result) == false)
                return NilValue.Instance;

            return new StringValue(result.ToString("C", context.CultureInfo));
        }
    }
}