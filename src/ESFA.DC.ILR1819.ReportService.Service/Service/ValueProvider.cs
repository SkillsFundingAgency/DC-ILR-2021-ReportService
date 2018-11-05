using System;
using System.Collections.Generic;
using ESFA.DC.ILR1819.ReportService.Interface.Service;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class ValueProvider : IValueProvider
    {
        public void GetFormattedValue(List<object> values, object value)
        {
            if (value == null)
            {
                values.Add(string.Empty);
                return;
            }

            if (value is decimal d1)
            {
                values.Add(decimal.Round(d1, 2));
                return;
            }

            if (IsOfNullableType<decimal>(value))
            {
                decimal? d = (decimal?)value;
                values.Add(decimal.Round(d.Value, 2));
                return;
            }

            values.Add(value);
        }

        private bool IsOfNullableType<T>(object o)
        {
            return Nullable.GetUnderlyingType(o.GetType()) != null && o is T;
        }
    }
}
