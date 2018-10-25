using System;
using System.IO;
using System.Linq;
using Aspose.Cells;
using ESFA.DC.ILR1819.ReportService.Tests.Models;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Helpers
{
    public static class TestXlsxHelper
    {
        /// <summary>
        /// Checks the Excel XLSX file matches the expected structure.
        /// </summary>
        /// <param name="xlsx">The xlsx file as a byte array.</param>
        /// <param name="xlsxEntries">The entries to verify.</param>
        public static void CheckXlsx(byte[] xlsx, params XlsxEntry[] xlsxEntries)
        {
            try
            {
                LoadOptions loadOptions = new LoadOptions(LoadFormat.Xlsx);

                Workbook workbook;
                using (MemoryStream ms = new MemoryStream(xlsx))
                {
                    workbook = new Workbook(ms, loadOptions);
                }

                Worksheet worksheet = workbook.Worksheets[0];

                int currentRow = 0;

                foreach (XlsxEntry xlsxEntry in xlsxEntries)
                {
                    object[] names = xlsxEntry.Mapper.MemberMaps.OrderBy(x => x.Data.Index)
                        .Select(x => (object)x.Data.Names[0]).ToArray();
                    if (xlsxEntry.Pivot)
                    {
                        CheckPivot(currentRow, names, worksheet, xlsxEntry.DataRows);
                        currentRow += names.Length;
                    }
                    else
                    {
                        CheckHeader(currentRow, names, worksheet);
                        currentRow++;
                        CheckRow(currentRow, names, worksheet, xlsxEntry.DataRows);
                        currentRow += xlsxEntry.DataRows;
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Null(ex);
            }
        }

        private static void CheckPivot(int row, object[] names, Worksheet worksheet, int numberOfColumnsOfData)
        {
            numberOfColumnsOfData = numberOfColumnsOfData * 2;
            object[,] cells = worksheet.Cells.ExportArray(row, 0, names.Length, 2);
            int pointer = 0;
            foreach (object cell in cells)
            {
                cell.Should().NotBeNull();
                if (pointer++ == numberOfColumnsOfData)
                {
                    // End of mandatory data
                    break;
                }
            }
        }

        private static void CheckRow(int row, object[] names, Worksheet worksheet, int numberOfColumnsOfData)
        {
            object[,] cells = worksheet.Cells.ExportArray(row, 0, 1, names.Length);
            int pointer = 0;
            foreach (object cell in cells)
            {
                cell.Should().NotBeNull();
                if (pointer++ == numberOfColumnsOfData)
                {
                    // End of mandatory data
                    break;
                }
            }
        }

        private static void CheckHeader(int row, object[] names, Worksheet worksheet)
        {
            object[,] cells = worksheet.Cells.ExportArray(row, 0, 1, names.Length);
            int column = 0;
            foreach (object cell in cells)
            {
                cell.Should().Be(names[column++]);
            }
        }
    }
}
