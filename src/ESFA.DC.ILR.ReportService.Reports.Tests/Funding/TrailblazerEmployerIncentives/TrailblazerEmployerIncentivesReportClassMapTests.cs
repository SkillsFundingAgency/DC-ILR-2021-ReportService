using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.TrailblazerEmployerIncentives
{
    public class TrailblazerEmployerIncentivesReportClassMapTests
    {
        [Fact]
        public void Map_Columns()
        {
            var orderedColumns = new string[]
            {
                "Employer identifier",
                @"August small employer incentive (£)",
                @"August 16-18 year-old apprentice incentive (£)",
                @"August achievement incentive (£)",
                @"August total (£)",
                @"September small employer incentive (£)",
                @"September 16-18 year-old apprentice incentive (£)",
                @"September achievement incentive (£)",
                @"September total (£)",
                @"October small employer incentive (£)",
                @"October 16-18 year-old apprentice incentive (£)",
                @"October achievement incentive (£)",
                @"October total (£)",
                @"November small employer incentive (£)",
                @"November 16-18 year-old apprentice incentive (£)",
                @"November achievement incentive (£)",
                @"November total (£)",
                @"December small employer incentive (£)",
                @"December 16-18 year-old apprentice incentive (£)",
                @"December achievement incentive (£)",
                @"December total (£)",
                @"January small employer incentive (£)",
                @"January 16-18 year-old apprentice incentive (£)",
                @"January achievement incentive (£)",
                @"January total (£)",
                @"February small employer incentive (£)",
                @"February 16-18 year-old apprentice incentive (£)",
                @"February achievement incentive (£)",
                @"February total (£)",
                @"March small employer incentive (£)",
                @"March 16-18 year-old apprentice incentive (£)",
                @"March achievement incentive (£)",
                @"March total (£)",
                @"April small employer incentive (£)",
                @"April 16-18 year-old apprentice incentive (£)",
                @"April achievement incentive (£)",
                @"April total (£)",
                @"May small employer incentive (£)",
                @"May 16-18 year-old apprentice incentive (£)",
                @"May achievement incentive (£)",
                @"May total (£)",
                @"June small employer incentive (£)",
                @"June 16-18 year-old apprentice incentive (£)",
                @"June achievement incentive (£)",
                @"June total (£)",
                @"July small employer incentive (£)",
                @"July 16-18 year-old apprentice incentive (£)",
                @"July achievement incentive (£)",
                @"July total (£)",
                @"Total small employer incentive (£)",
                @"Total 16-18 year-old apprentice incentive (£)",
                @"Total achievement incentive (£)",
                @"Grand total (£)",
            };

            var input = new List<TrailblazerEmployerIncentivesReportModel>();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<TrailblazerEmployerIncentivesReportClassMap>();

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
                        
                        header.Should().HaveCount(53);
                    }
                }
            }
        }

        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<TrailblazerEmployerIncentivesReportModel>()
            {
                new TrailblazerEmployerIncentivesReportModel()
                {
                    EmployerIdentifier = 111,
                    AugustSmallEmployerIncentive = 10.0m,
                    August1618ApprenticeIncentive = 10.1m,
                    AugustAchievementPayment = 10.2m,
                }
            };

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<TrailblazerEmployerIncentivesReportClassMap>();

                        csvWriter.WriteRecords(input);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        var output = csvReader.GetRecords<dynamic>().ToList();

                        (output[0] as IDictionary<string, object>).Values.ToArray()[0].Should().Be("111");
                    }
                }
            }
        }
    }
}
