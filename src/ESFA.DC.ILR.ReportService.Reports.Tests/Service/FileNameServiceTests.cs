﻿using System;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Service
{
    public class FileNameServiceTests
    {
        [InlineData(OutputTypes.Csv, "csv")]
        [InlineData(OutputTypes.Excel, "xlsx")]
        [InlineData(OutputTypes.Json, "json")]
        [InlineData(OutputTypes.Zip, "zip")]
        [Theory]
        public void GetFileName(OutputTypes outputType, string extension)
        {
            var submissionDateTime = new DateTime(2019, 1, 1, 1, 1, 1);
            var ukDateTime = new DateTime(2020, 1, 1, 1, 1, 1);
            var ukprn = 1234;
            var jobId = 5678;
            var fileName = "FileName";

            var dateTimeProvider = new Mock<IDateTimeProvider>();

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.SetupGet(c => c.Ukprn).Returns(ukprn);
            reportServiceContextMock.SetupGet(c => c.JobId).Returns(jobId);
            reportServiceContextMock.SetupGet(c => c.SubmissionDateTimeUtc).Returns(submissionDateTime);

            dateTimeProvider.Setup(p => p.ConvertUtcToUk(submissionDateTime)).Returns(ukDateTime);

            var result = NewService(dateTimeProvider.Object).GetFilename(reportServiceContextMock.Object, fileName, outputType);

            result.Should().Be($"{ukprn}/{jobId}/{fileName} 20200101-010101.{extension}");
        }

        [InlineData(OutputTypes.Csv, "csv")]
        [InlineData(OutputTypes.Excel, "xlsx")]
        [InlineData(OutputTypes.Json, "json")]
        [InlineData(OutputTypes.Zip, "zip")]
        [Theory]
        public void GetFileName_NoDate(OutputTypes outputType, string extension)
        {
            var ukprn = 1234;
            var jobId = 5678;
            var fileName = "FileName";

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.SetupGet(c => c.Ukprn).Returns(ukprn);
            reportServiceContextMock.SetupGet(c => c.JobId).Returns(jobId);
            
            var result = NewService(null).GetFilename(reportServiceContextMock.Object, fileName, outputType, false);

            result.Should().Be($"{ukprn}/{jobId}/{fileName}.{extension}");
        }

        private FileNameService NewService(IDateTimeProvider dateTimeProvider = null)
        {
            return new FileNameService(dateTimeProvider);
        }
    }
}
