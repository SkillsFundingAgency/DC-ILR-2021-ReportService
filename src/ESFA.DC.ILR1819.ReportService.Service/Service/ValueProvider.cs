using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Generation;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class ValueProvider : IValueProvider
    {
        public void GetFormattedValue(List<object> values, object value, ClassMap mapper, ModelProperty modelProperty)
        {
            if (value == null)
            {
                values.Add(string.Empty);
                return;
            }

            if (value is bool b)
            {
                values.Add(b ? "Yes" : "No");
                return;
            }

            if (value is decimal d1)
            {
                values.Add(decimal.Round(d1, GetDecimalPoints(mapper, modelProperty)));
                return;
            }

            if (IsOfNullableType<decimal>(value))
            {
                decimal? d = (decimal?)value;
                values.Add(decimal.Round(d.Value, GetDecimalPoints(mapper, modelProperty)));
                return;
            }

            values.Add(value);
        }

        private bool IsOfNullableType<T>(object o)
        {
            return Nullable.GetUnderlyingType(o.GetType()) != null && o is T;
        }

        private int GetDecimalPoints(ClassMap mapper, ModelProperty modelProperty)
        {
            if (mapper == null || modelProperty == null)
            {
                return 2;
            }

            MemberMap memberMap = mapper.MemberMaps.SingleOrDefault(x => x.Data.Names.Names.Intersect(modelProperty.Names).Any());
            string[] format = memberMap?.Data?.TypeConverterOptions?.Formats ?? new[] { "0.00" };
            if (format.Length > 0)
            {
                string[] decimals = format[0].Split('.');
                if (decimals.Length == 2)
                {
                    return decimals[1].Length;
                }
            }

            return 2;
        }
    }
}
