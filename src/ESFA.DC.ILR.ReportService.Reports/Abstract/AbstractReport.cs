using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.ILR.ReportService.Service.Model.Generation;
using ESFA.DC.ILR.ReportService.Service.Model.Styling;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Reports.Abstract
{
    public abstract class AbstractReport
    {
        private readonly IValueProvider _valueProvider;
        private readonly Dictionary<Worksheet, int> _currentRow = new Dictionary<Worksheet, int>();

        protected AbstractReport(IValueProvider valueProvider)
        {
            _valueProvider = valueProvider;
        }

        /// <summary>
        /// Builds an Excel report using the specified mapper as the list of column names.
        /// </summary>
        /// <typeparam name="TMapper">The mapper.</typeparam>
        /// <typeparam name="TModel">The model.</typeparam>
        /// <param name="worksheet">The worksheet to operate on.</param>
        /// <param name="classMap">The class mapper to use to write the headers, and to get the properties references from.</param>
        /// <param name="records">The records to write.</param>
        /// <param name="headerStyle">The style to apply to the header.</param>
        /// <param name="recordStyle">The style to apply to the records.</param>
        /// <param name="pivot">Whether to write the data vertically, rather than horizontally.</param>
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
                    _valueProvider.GetFormattedValue(values, modelProperty.MethodInfo.GetValue(record), classMap, modelProperty);

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

        protected void WriteCsvRecords<TMapper, TModel>(CsvWriter csvWriter, IEnumerable<TModel> records)
            where TMapper : ClassMap
            where TModel : class
        {
            csvWriter.Configuration.RegisterClassMap<TMapper>();

            csvWriter.WriteHeader<TModel>();
            csvWriter.NextRecord();

            csvWriter.WriteRecords(records);

            csvWriter.Configuration.UnregisterClassMap();
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

        

    }
}
