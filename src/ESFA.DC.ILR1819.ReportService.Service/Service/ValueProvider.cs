using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Generation;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class ValueProvider : IValueProvider
    {
        private readonly string DateTimeMin = DateTime.MinValue.ToString("dd/MM/yyyy");

        public void GetFormattedValue(List<object> values, object value, ClassMap mapper, ModelProperty modelProperty)
        {
            Type propertyType = modelProperty.MethodInfo.PropertyType;

            if (value == null)
            {
                if (IsNullable(propertyType) && propertyType == typeof(decimal?))
                {
                    int decimalPoints = GetDecimalPoints(mapper, modelProperty);
                    values.Add(PadDecimal(0, decimalPoints));
                    return;
                }

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
                int decimalPoints = GetDecimalPoints(mapper, modelProperty);
                decimal rounded = decimal.Round(d1, decimalPoints);
                values.Add(PadDecimal(rounded, decimalPoints));
                return;
            }

            if (IsOfNullableType<decimal>(propertyType))
            {
                int decimalPoints = GetDecimalPoints(mapper, modelProperty);
                decimal? d = (decimal?)value;
                decimal rounded = decimal.Round(d.GetValueOrDefault(0), decimalPoints);
                values.Add(PadDecimal(rounded, decimalPoints));
                return;
            }

            if (value is string str)
            {
                if (str == DateTimeMin)
                {
                    values.Add(string.Empty);
                    return;
                }
            }

            values.Add(value);
        }

        private bool IsOfNullableType<T>(object o)
        {
            return Nullable.GetUnderlyingType(o.GetType()) != null && o is T;
        }

        private bool IsNullable(Type propertyType)
        {
            return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
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

        private string PadDecimal(decimal value, int decimalPoints)
        {
            string valueStr = value.ToString(CultureInfo.InvariantCulture);
            int decimalPointPos = valueStr.IndexOf('.');
            int actualDecimalPoints = 0;
            if (decimalPointPos > -1)
            {
                actualDecimalPoints = valueStr.Length - (decimalPointPos + 1);
            }
            else
            {
                valueStr += ".";
            }

            return valueStr + new string('0', decimalPoints - actualDecimalPoints);
        }
    }
}
