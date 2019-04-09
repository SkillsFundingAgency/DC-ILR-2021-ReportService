﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Service;
using ESFA.DC.ILR.ReportService.Tests.AutoFac;
using ESFA.DC.ILR1819.ReportService.Stateless.Context;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Tests.General
{
    public sealed class TestEntryPoint
    {
        [Fact]
        public async Task TestEntryPointZipGeneration()
        {
            string csv = $"A,B,C,D{Environment.NewLine}1,2,3,4";
            byte[] zipBytes = null;

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IStreamableKeyValuePersistenceService> streamableKeyValuePersistenceService =
                new Mock<IStreamableKeyValuePersistenceService>();
            streamableKeyValuePersistenceService.Setup(x =>
                    x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<string, Stream, CancellationToken>((s, m, c) =>
                {
                    zipBytes = new byte[m.Length];
                    m.Seek(0, SeekOrigin.Begin);
                    m.Read(zipBytes, 0, (int)m.Length);
                })
                .Returns(Task.CompletedTask);
            Mock<IReport> report = new Mock<IReport>();
            report.SetupGet(x => x.ReportTaskName).Returns(ReportTaskNameConstants.AllbOccupancyReport);
            report.Setup(x =>
                    x.GenerateReport(It.IsAny<IReportServiceContext>(), It.IsAny<ZipArchive>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Callback<IReportServiceContext, ZipArchive, bool, CancellationToken>((j, z, b, c) =>
                {
                    ZipArchiveEntry archivedFile = z.CreateEntry("ExampleReport.csv", CompressionLevel.Optimal);
                    using (var sw = new StreamWriter(archivedFile.Open()))
                    {
                        sw.Write(csv);
                    }
                })
                .Returns(Task.CompletedTask);
            report.Setup(x => x.IsMatch(It.IsAny<string>())).Returns(true);

            JobContextMessage jobContextMessage =
                new JobContextMessage(
                    1,
                    new ITopicItem[] { new TopicItem("SubscriptionName", new List<ITaskItem> { new TaskItem(new List<string> { ReportTaskNameConstants.AllbOccupancyReport }, false) }) },
                    0,
                    DateTime.UtcNow);

            jobContextMessage.KeyValuePairs.Add(JobContextMessageKey.UkPrn, 1234);

            EntryPoint entryPoint = new EntryPoint(
                logger.Object,
                streamableKeyValuePersistenceService.Object,
                new[] { report.Object });

            await entryPoint.Callback(new ReportServiceJobContextMessageContext(jobContextMessage), CancellationToken.None);

            string zipContents;
            using (MemoryStream ms = new MemoryStream(zipBytes))
            {
                using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry entry = archive.GetEntry("ExampleReport.csv");
                    entry.Should().NotBeNull();
                    using (StreamReader reader = new StreamReader(entry.Open()))
                    {
                        zipContents = reader.ReadToEnd();
                    }
                }
            }

            zipContents.Should().NotBeNullOrEmpty();
            zipContents.Should().Be(csv);
        }
    }
}