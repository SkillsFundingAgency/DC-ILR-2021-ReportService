using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.SummaryOfFM35Funding
{
    public class SummaryOfFM35FundingReportClassMapTests
    {
        [Fact]
        public void Map_Columns()
        {
            var orderedColumns = new string[]
            {
                "Funding Line Type",
                "Period",
                "On-programme (£)",
                "Balancing (£)",
                "Job Outcome Achievement (£)",
                "Aim Achievement (£)",
                "Total Achievement (£)",
                "Learning Support (£)",
                "Total (£)",
                "OFFICIAL - SENSITIVE",           
            };

            var input = new List<SummaryOfFM35FundingReportModel>();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<SummaryOfFM35FundingReportClassMap>();

                        csvWriter.WriteRecords(input);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        csvReader.Read();
                        csvReader.ReadHeader();
                        var header = csvReader.Context.HeaderRecord;
                        
                        header.Should().ContainInOrder(orderedColumns);
                        
                        header.Should().HaveCount(10);
                    }
                }
            }
        }

        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<SummaryOfFM35FundingReportModel>()
            {
                new SummaryOfFM35FundingReportModel()
                {
                    FundingLineType = "FundLine1",
                    Period = 1,
                    OnProgramme = 1m,
                    Balancing= 1m,
                    JobOutcomeAchievement = 1m,
                    AimAchievement = 1m,
                    LearningSupport = 1m,
                },
                new SummaryOfFM35FundingReportModel()
                {
                    FundingLineType = "FundLine2",
                    Period = 2,
                    OnProgramme = 2m,
                    Balancing= 2m,
                    JobOutcomeAchievement = 2m,
                    AimAchievement = 2m,
                    LearningSupport = 2m,
                }
            };

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<SummaryOfFM35FundingReportClassMap>();

                        csvWriter.WriteRecords(input);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        var output = csvReader.GetRecords<dynamic>().ToList();

                        (output[0] as IDictionary<string, object>).Values.ToArray()[0].Should().Be("FundLine1");
                    }
                }
            }
        }
    }
}
