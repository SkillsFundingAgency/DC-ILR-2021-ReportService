using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests
{
    public class FileNameServiceTests
    {
        [InlineData(OutputTypes.Csv, "csv")]
        [InlineData(OutputTypes.Excel, "xlsx")]
        [InlineData(OutputTypes.Json, "json")]
        [Theory]
        public void GetExtensions(OutputTypes outputType, string extension)
        {
            NewService().GetExtension(outputType).Should().Be(extension);
        }

        private FileNameService NewService(IDateTimeProvider dateTimeProvider = null)
        {
            return new FileNameService(dateTimeProvider);
        }
    }
}
