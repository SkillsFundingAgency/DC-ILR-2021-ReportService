using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Tests.Stubs;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Service
{
    public class CsvServiceTests
    {
        [Fact]
        public async void WriteAsync()
        {
            var fileName = "CsvServiceTest.csv";
            var container = "Output";

            var records = new List<TestModel>()
            {
                new TestModel()
                {
                    Id = 1,
                    Amount = 10.00m,
                    Name = "Name",
                    DateTime = new DateTime(2019, 01, 02),
                    DateTimeNull = new DateTime(2019, 02, 03)
                }
            };

            var fileService = new FileServiceStub();

            await NewService(fileService).WriteAsync<TestModel, TestMap>(records, fileName, container, CancellationToken.None);

            using (var stream = await fileService.OpenReadStreamAsync(fileName, container, CancellationToken.None))
            {
                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        var output = csvReader.GetRecords<dynamic>().ToList();

                        ((IDictionary<string, object>)output[0]).Values.ToArray()[0].Should().Be("1");
                        ((IDictionary<string, object>)output[0]).Values.ToArray()[1].Should().Be("10.00");
                        ((IDictionary<string, object>)output[0]).Values.ToArray()[2].Should().Be("Name");
                        ((IDictionary<string, object>)output[0]).Values.ToArray()[3].Should().Be("02/01/2019");
                        ((IDictionary<string, object>)output[0]).Values.ToArray()[4].Should().Be("03/02/2019");
                    }
                }
            }
        }

        private CsvService NewService(IFileService fileService = null)
        {
            return new CsvService(fileService);
        }

        private class TestModel
        {
            public int Id { get; set; }
            public decimal Amount { get; set; }
            public string Name { get; set; }
            public DateTime? DateTime { get; set; }
            public DateTime? DateTimeNull { get; set; }
        }

        private sealed class TestMap : ClassMap<TestModel>
        {
            public TestMap()
            {
                Map(m => m.Id);
                Map(m => m.Amount);
                Map(m => m.Name);
                Map(m => m.DateTime);
                Map(m => m.DateTimeNull);
            }
        }
    }
}
