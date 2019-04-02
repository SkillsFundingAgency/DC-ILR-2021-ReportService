﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Generation;
using ESFA.DC.ILR.ReportService.Model.Styling;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public abstract class AbstractReportBuilder
    {
        protected string ReportFileName;

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValueProvider _valueProvider;

        private readonly Dictionary<Worksheet, int> _currentRow;

        protected AbstractReportBuilder(IDateTimeProvider dateTimeProvider, IValueProvider valueProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _valueProvider = valueProvider;

            _currentRow = new Dictionary<Worksheet, int>();
        }

        public string ReportTaskName { get; set; }

        public bool IsMatch(string reportTaskName)
        {
            return string.Equals(reportTaskName, ReportTaskName, StringComparison.OrdinalIgnoreCase);
        }

        public string GetExternalFilename(string ukPrn, long jobId, DateTime submissionDateTime)
        {
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(submissionDateTime);
            return $"{ukPrn}_{jobId.ToString()}_{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
        }

        public string GetFilename(string ukPrn, long jobId, DateTime submissionDateTime)
        {
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(submissionDateTime);
            return $"{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
        }

        /// <summary>
        /// Builds a CSV report using the specified mapper as the list of column names.
        /// </summary>
        /// <typeparam name="TMapper">The mapper.</typeparam>
        /// <typeparam name="TModel">The model.</typeparam>
        /// <param name="csvWriter">The memory stream to write to.</param>
        /// <param name="records">The records to persist.</param>
        /// <param name="mapperOverride">Optional override of the TMapper, for example, when needing to specify constructor parameters.</param>
        protected void WriteCsvRecords<TMapper, TModel>(CsvWriter csvWriter, IEnumerable<TModel> records, TMapper mapperOverride = null)
            where TMapper : ClassMap
            where TModel : class
        {
            if (mapperOverride == null)
            {
                csvWriter.Configuration.RegisterClassMap<TMapper>();
            }
            else
            {
                csvWriter.Configuration.RegisterClassMap(mapperOverride);
            }

            csvWriter.WriteHeader<TModel>();
            csvWriter.NextRecord();

            if (mapperOverride == null)
            {
                csvWriter.WriteRecords(records);
            }
            else
            {
                ModelProperty[] names = mapperOverride.MemberMaps.OrderBy(x => x.Data.Index).Select(x => new ModelProperty(x.Data.Names.Names.ToArray(), (PropertyInfo)x.Data.Member)).ToArray();
                foreach (TModel record in records)
                {
                    WriteCsvRecords(csvWriter, mapperOverride, names, record);
                }
            }

            csvWriter.Configuration.UnregisterClassMap();
        }

        protected void WriteCsvRecords<TMapper>(CsvWriter csvWriter, TMapper mapper)
            where TMapper : ClassMap
        {
            object[] names = mapper.MemberMaps.OrderBy(x => x.Data.Index).SelectMany(x => x.Data.Names.Names).Select(x => (object)x).ToArray();
            WriteCsvRecords(csvWriter, names);
        }

        protected void WriteCsvRecords<TMapper, TModel>(CsvWriter csvWriter, TMapper mapper, TModel record)
            where TMapper : ClassMap
            where TModel : class
        {
            ModelProperty[] names = mapper.MemberMaps.OrderBy(x => x.Data.Index).Select(x => new ModelProperty(x.Data.Names.Names.ToArray(), (PropertyInfo)x.Data.Member)).ToArray();
            WriteCsvRecords(csvWriter, mapper, names, record);
        }

        protected void WriteCsvRecords<TMapper, TModel>(CsvWriter csvWriter, TMapper mapper, ModelProperty[] modelProperties, TModel record)
            where TMapper : ClassMap
        {
            List<object> values = new List<object>();
            foreach (var modelProperty in modelProperties)
            {
                _valueProvider.GetFormattedValue(values, modelProperty.MethodInfo.GetValue(record), mapper, modelProperty);
            }

            WriteCsvRecords(csvWriter, values.ToArray());
        }

        /// <summary>
        /// Builds a CSV report using the specified mapper as the list of column names.
        /// </summary>
        /// <typeparam name="TMapper">The mapper.</typeparam>
        /// <typeparam name="TModel">The model.</typeparam>
        /// <param name="writer">The memory stream to write to.</param>
        /// <param name="record">The record to persist.</param>
        protected void WriteCsvRecords<TMapper, TModel>(CsvWriter writer, TModel record)
            where TMapper : ClassMap
            where TModel : class
        {
            WriteCsvRecords<TMapper, TModel>(writer, new[] { record });
        }

        /// <summary>
        /// Writes a blank row to the csv file.
        /// </summary>
        /// <param name="writer">The memory stream to write to.</param>
        /// <param name="numberOfBlankRows">The optional number of blank rows to create.</param>
        protected void WriteCsvRecords(CsvWriter writer, int numberOfBlankRows = 1)
        {
            for (int i = 0; i < numberOfBlankRows; i++)
            {
                writer.NextRecord();
            }
        }

        /// <summary>
        /// Writes the items as individual tokens to the CSV.
        /// </summary>
        /// <param name="writer">The writer target.</param>
        /// <param name="items">The strings to write.</param>
        protected void WriteCsvRecords(CsvWriter writer, params object[] items)
        {
            foreach (object item in items)
            {
                writer.WriteField(item);
            }

            writer.NextRecord();
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

        protected void WriteExcelRecords<TMapper>(Worksheet worksheet, TMapper classMap, CellStyle headerStyle, bool pivot = false)
            where TMapper : ClassMap
        {
            string[] names = classMap.MemberMaps.OrderBy(x => x.Data.Index).Select(x => x.Data.Names[0]).ToArray();
            WriteExcelRecords(worksheet, classMap, names, headerStyle, pivot);
        }

        protected void WriteExcelRecords<TMapper>(Worksheet worksheet, TMapper classMap, string[] names, CellStyle headerStyle, bool pivot = false)
            where TMapper : ClassMap
        {
            int currentRow = GetCurrentRow(worksheet);

            worksheet.Cells.ImportObjectArray(names, currentRow, 0, pivot);
            if (headerStyle != null)
            {
                worksheet.Cells.CreateRange(currentRow, 0, pivot ? names.Length : 1, pivot ? 1 : names.Length).ApplyStyle(headerStyle.Style, headerStyle.StyleFlag);
            }

            if (pivot)
            {
                currentRow += names.Length;
            }
            else
            {
                currentRow++;
            }

            SetCurrentRow(worksheet, currentRow);
        }

        protected void WriteExcelRecords<TMapper, TModel>(Worksheet worksheet, TMapper classMap, TModel record, CellStyle recordStyle, bool pivot = false)
            where TMapper : ClassMap
            where TModel : class
        {
            ModelProperty[] names = classMap.MemberMaps.OrderBy(x => x.Data.Index).Select(x => new ModelProperty(x.Data.Names.Names.ToArray(), (PropertyInfo)x.Data.Member)).ToArray();
            WriteExcelRecords(worksheet, classMap, names, record, recordStyle, pivot);
        }

        protected void WriteExcelRecords<TMapper, TModel>(Worksheet worksheet, TMapper classMap, ModelProperty[] modelProperties, TModel record, CellStyle recordStyle, bool pivot = false)
            where TMapper : ClassMap
            where TModel : class
        {
            int currentRow = GetCurrentRow(worksheet);

            int column = 0;
            if (pivot)
            {
                // If we have pivoted then we need to move one column in, as the header is in column 1.
                column = 1;
            }

            List<object> values = new List<object>();
            foreach (ModelProperty modelProperty in modelProperties)
            {
                _valueProvider.GetFormattedValue(values, modelProperty.MethodInfo.GetValue(record), classMap, modelProperty);
            }

            worksheet.Cells.ImportObjectArray(values.ToArray(), currentRow, column, pivot);
            if (recordStyle != null)
            {
                worksheet.Cells.CreateRange(currentRow, column, pivot ? values.Count : 1, pivot ? 1 : values.Count).ApplyStyle(recordStyle.Style, recordStyle.StyleFlag);
            }

            if (pivot)
            {
                currentRow += values.Count;
            }
            else
            {
                currentRow++;
            }

            SetCurrentRow(worksheet, currentRow);
        }

        /// <summary>
        /// Writes a blank row to the worksheet (increments the current row number)
        /// </summary>
        /// <param name="worksheet">The current worksheet.</param>
        /// <param name="numberOfBlankRows">The optional number of blank rows to create.</param>
        protected void WriteExcelRecords(Worksheet worksheet, int numberOfBlankRows = 1)
        {
            int currentRow = GetCurrentRow(worksheet);
            currentRow += numberOfBlankRows;
            SetCurrentRow(worksheet, currentRow);
        }

        /// <summary>
        /// Writes a new heading row in column 1, optionally extends across a number of columns
        /// </summary>
        /// <param name="worksheet">The current worksheet,</param>
        /// <param name="heading">The heading text to write out.</param>
        /// <param name="headerStyle">The optional header style.</param>
        /// <param name="numberOfColumns">The optional number of columns.</param>
        protected void WriteExcelRecords(Worksheet worksheet, string heading, CellStyle headerStyle = null, int numberOfColumns = 1)
        {
            int currentRow = GetCurrentRow(worksheet);
            worksheet.Cells[currentRow, 0].PutValue(heading);
            if (headerStyle != null)
            {
                worksheet.Cells.CreateRange(currentRow, 0, 1, numberOfColumns).ApplyStyle(headerStyle.Style, headerStyle.StyleFlag);
            }

            currentRow++;
            SetCurrentRow(worksheet, currentRow);
        }

        /// <summary>
        /// Writes the data to the zip file with the specified filename.
        /// </summary>
        /// <param name="archive">Archive to write to.</param>
        /// <param name="filename">Filename to use in zip file.</param>
        /// <param name="data">Data to write.</param>
        /// <returns>Awaitable task.</returns>
        protected async Task WriteZipEntry(ZipArchive archive, string filename, string data)
        {
            if (archive == null)
            {
                return;
            }

            ZipArchiveEntry entry = archive.GetEntry(filename);
            entry?.Delete();

            ZipArchiveEntry archivedFile = archive.CreateEntry(filename, CompressionLevel.Optimal);
            using (StreamWriter sw = new StreamWriter(archivedFile.Open()))
            {
                await sw.WriteAsync(data);
            }
        }

        /// <summary>
        /// Writes the stream to the zip file with the specified filename.
        /// </summary>
        /// <param name="archive">Archive to write to.</param>
        /// <param name="filename">Filename to use in zip file.</param>
        /// <param name="data">Data to write.</param>
        /// <param name="cancellationToken">Cancellation token for cancelling copy operation.</param>
        /// <returns>Awaitable task.</returns>
        protected async Task WriteZipEntry(ZipArchive archive, string filename, Stream data, CancellationToken cancellationToken)
        {
            if (archive == null)
            {
                return;
            }

            ZipArchiveEntry entry = archive.GetEntry(filename);
            entry?.Delete();

            ZipArchiveEntry archivedFile = archive.CreateEntry(filename, CompressionLevel.Optimal);
            using (Stream sw = archivedFile.Open())
            {
                data.Seek(0, SeekOrigin.Begin);
                await data.CopyToAsync(sw, 81920, cancellationToken);
            }
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
