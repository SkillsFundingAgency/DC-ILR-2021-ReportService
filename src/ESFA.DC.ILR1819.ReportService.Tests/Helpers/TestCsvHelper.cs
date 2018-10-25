using System;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;
using ESFA.DC.ILR1819.ReportService.Tests.Models;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Helpers
{
    public static class TestCsvHelper
    {
        /// <summary>
        /// Checks the csv data matches the expected structure.
        /// </summary>
        /// <param name="csv">The csv data.</param>
        /// <param name="csvEntries">The entries to verify.</param>
        public static void CheckCsv(string csv, params CsvEntry[] csvEntries)
        {
            try
            {
                using (TextReader textReader = new StringReader(csv))
                {
                    using (TextFieldParser textFieldParser = new TextFieldParser(textReader))
                    {
                        textFieldParser.TextFieldType = FieldType.Delimited;
                        textFieldParser.Delimiters = new[] { "," };
                        long lastKnownRow = 0;
                        foreach (CsvEntry csvEntry in csvEntries)
                        {
                            string[] currentRow = textFieldParser.ReadFields();

                            if (csvEntry.BlankRowsBefore > 0)
                            {
                                // Reader will automatically skip the blank row, so the line number will be 1 too far.
                                Assert.Equal(csvEntry.BlankRowsBefore, (textFieldParser.LineNumber - 1) - lastKnownRow);
                            }

                            CheckHeader(currentRow, csvEntry.Mapper);
                            for (int i = 0; i < csvEntry.DataRows; i++)
                            {
                                currentRow = textFieldParser.ReadFields();
                                Assert.NotNull(currentRow);
                                CheckRow(currentRow, csvEntry.Mapper);
                            }

                            lastKnownRow = textFieldParser.LineNumber;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Null(ex);
            }
        }

        private static void CheckRow(string[] currentRow, ClassMap classMap)
        {
            currentRow.Length.Should().Be(classMap.MemberMaps.Count);
        }

        private static void CheckHeader(string[] currentRow, ClassMap classMap)
        {
            string[] names = classMap.MemberMaps.OrderBy(x => x.Data.Index).Select(x => x.Data.Names[0]).ToArray();
            currentRow.Should().BeEquivalentTo(names);
        }
    }
}
