using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Model.Generation;
using ESFA.DC.ILR.ReportService.Service.Model.Styling;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Reports.Abstract
{
    public abstract class AbstractReport
    {
        private readonly Dictionary<Worksheet, int> _currentRow = new Dictionary<Worksheet, int>();

        protected AbstractReport(string taskName, string fileName)
        {
            TaskName = taskName;
            FileName = fileName;
        }

        public string TaskName { get; }

        public string FileName { get; }

        protected void WriteExcelRecords<TMapper, TModel>(Worksheet worksheet, TMapper classMap, IEnumerable<TModel> records, CellStyle headerStyle, CellStyle recordStyle, bool pivot = false)
            where TMapper : ClassMap
            where TModel : class
        {
            int currentRow = GetCurrentRow(worksheet);
            ModelProperty[] modelProperties = classMap.MemberMaps.OrderBy(x => x.Data.Index).Select(x => new ModelProperty(x.Data.Names.Names.ToArray(), (PropertyInfo)x.Data.Member)).ToArray();
            string[] names = modelProperties.SelectMany(x => x.Names).ToArray();

            worksheet.Cells.ImportObjectArray(names, currentRow, 0, pivot);
            if (headerStyle != null)
            {
                worksheet.Cells.CreateRange(currentRow, 0, pivot ? names.Length : 1, pivot ? 1 : names.Length).ApplyStyle(headerStyle.Style, headerStyle.StyleFlag);
            }

            int column = 0, localRow = currentRow;
            if (pivot)
            {
                // If we have pivoted then we need to move one column in, as the header is in column 1.
                column = 1;
            }
            else
            {
                currentRow++;
                localRow++;
            }

            foreach (TModel record in records)
            {
                int widestColumn = 1;

                foreach (ModelProperty modelProperty in modelProperties)
                {
                    List<object> values = new List<object>();
                    GetFormattedValue(values, modelProperty.MethodInfo.GetValue(record), classMap, modelProperty);

                    worksheet.Cells.ImportObjectArray(values.ToArray(), localRow, column, false);
                    if (recordStyle != null)
                    {
                        worksheet.Cells.CreateRange(localRow, column, 1, values.Count).ApplyStyle(recordStyle.Style, recordStyle.StyleFlag);
                    }

                    if (pivot)
                    {
                        localRow++;
                    }
                    else
                    {
                        column += values.Count;
                    }

                    if (values.Count > widestColumn)
                    {
                        widestColumn = values.Count;
                    }
                }

                if (pivot)
                {
                    column += widestColumn;
                    localRow = currentRow;
                }
                else
                {
                    column = 0;
                    localRow++;
                }
            }

            if (pivot)
            {
                currentRow += names.Length;
            }
            else
            {
                currentRow += records.Count();
            }

            SetCurrentRow(worksheet, currentRow);
        }

        private int GetCurrentRow(Worksheet worksheet)
        {
            if (!_currentRow.ContainsKey(worksheet))
            {
                _currentRow.Add(worksheet, 0);
            }

            return _currentRow[worksheet];
        }

        private void SetCurrentRow(Worksheet worksheet, int currentRow)
        {
            _currentRow[worksheet] = currentRow;
        }

        private void GetFormattedValue(List<object> values, object value, ClassMap mapper, ModelProperty modelProperty)
        {
            Type propertyType = modelProperty.MethodInfo.PropertyType;

            if (value == null)
            {
                if (IsNullable(propertyType) && propertyType == typeof(decimal?))
                {
                    if (IsNullableMapper(mapper, modelProperty))
                    {
                        values.Add(Constants.NotApplicable);
                        return;
                    }

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
                values.Add(rounded);
                return;
            }

            if (IsOfNullableType<decimal>(propertyType))
            {
                decimal? d = (decimal?)value;
                int decimalPoints = GetDecimalPoints(mapper, modelProperty);
                decimal rounded = decimal.Round(d.GetValueOrDefault(0), decimalPoints);
                values.Add(rounded);
                return;
            }

            if (value is int i)
            {
                if (i == 0)
                {
                    if (!CanAddZeroInt(mapper, modelProperty))
                    {
                        values.Add(string.Empty);
                        return;
                    }
                }
            }

            if (value is string str)
            {
                if (str == Constants.DateTimeMin)
                {
                    values.Add(string.Empty);
                    return;
                }
            }

            values.Add(value);
        }

        private bool IsNullableMapper(ClassMap mapper, ModelProperty modelProperty)
        {
            MemberMap memberMap = mapper.MemberMaps.SingleOrDefault(x => x.Data.Names.Names.Intersect(modelProperty.Names).Any());
            return memberMap?.Data?.TypeConverterOptions?.NullValues?.Contains(Constants.NotApplicable) ?? false;
        }

        private bool CanAddZeroInt(ClassMap mapper, ModelProperty modelProperty)
        {
            MemberMap memberMap = mapper.MemberMaps.SingleOrDefault(x => x.Data.Names.Names.Intersect(modelProperty.Names).Any());
            return !(memberMap?.Data?.TypeConverterOptions?.NullValues?.Contains(Constants.Zero) ?? false);
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
