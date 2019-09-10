using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReportClassMapTests
    {
        [Fact]
        public void Map_Columns()
        {
            var orderedColumns = new string[]
            {
                "Learner reference number",
                "Unique learner number",
                "Date of birth",
                "Campus identifier",
                "Provider specified learner monitoring (A)",
                "Provider specified learner monitoring (B)",
                "Aim sequence number",
                "Learning aim reference",
                "Learning aim title",
                "Software supplier aim identifier",
                "Programme type",
                "Standard code",
                "Framework code",
                "Apprenticeship pathway",
                "Aim type",
                "Original learning start date",
                "Learning start date",
                "Learning planned end date",
                "Learning actual end date",
                "Achievement date",
                "Learning delivery funding and monitoring type - eligibility for enhanced apprenticeship funding",
                "Provider specified delivery monitoring (A)",
                "Provider specified delivery monitoring (B)",
                "Provider specified delivery monitoring (C)",
                "Provider specified delivery monitoring (D)",
                "Price episode start date",
                "Price episode actual end date",
                "Apprenticeship adjusted learning start date",
                "Age at programme start",
                "Funding line type",
                "Learning delivery funding and monitoring type - apprenticeship contract type",
                "Learning delivery funding and monitoring type - ACT date applies from",
                "Learning delivery funding and monitoring type - ACT date applies to",
                "August total earnings",
                "September total earnings",
                "October total earnings",
                "November total earnings",
                "December total earnings",
                "January total earnings",
                "February total earnings",
                "March total earnings",
                "April total earnings",
                "May total earnings",
                "June total earnings",
                "July total earnings",
                "Total earnings",
                "OFFICIAL - SENSITIVE",
            };

            var input = new List<NonContractedAppsActivityReportModel>();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<NonContractedAppsActivityReportClassMap>();

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
                        header.Should().HaveCount(47);
                    }
                }
            }
        }

        //[Fact]
        //public void ClassMap_Model()
        //{
        //    var input = new List<NonContractedAppsActivityReportModel>()
        //    {
        //        new NonContractedAppsActivityReportModel()
        //        {
        //            Learner = new TestLearner()
        //            {
        //                LearnRefNumber = "Test",
        //                DateOfBirthNullable = new DateTime(2019,07,13)
        //            },
        //            LearningDelivery = new TestLearningDelivery()
        //            {
        //                OrigLearnStartDateNullable = null
        //            },
        //            LarsLearningDelivery = new LARSLearningDelivery(),
        //            ProviderSpecLearnerMonitoring = new ProviderSpecLearnerMonitoringModel(),
        //            ProviderSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel(),
        //        }
        //    };

        //    using (var stream = new MemoryStream())
        //    {
        //        using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
        //        {
        //            using (var csvWriter = new CsvWriter(streamWriter))
        //            {
        //                csvWriter.Configuration.RegisterClassMap<NonContractedAppsActivityReportClassMap>();
        //                csvWriter.Configuration.TypeConverterOptionsCache.GetOptions<DateTime?>().Formats = new[] { "dd/MM/yyyy" };

        //                csvWriter.WriteRecords(input);
        //            }
        //        }

        //        stream.Position = 0;

        //        using (var streamReader = new StreamReader(stream))
        //        {
        //            using (var csvReader = new CsvReader(streamReader))
        //            {
        //                var output = csvReader.GetRecords<dynamic>().ToList();

        //                (output[0] as IDictionary<string, object>).Values.ToArray()[0].Should().Be("Test");
        //                (output[0] as IDictionary<string, object>).Values.ToArray()[2].Should().Be("13/07/2019");
        //                (output[0] as IDictionary<string, object>).Values.ToArray()[22].Should().Be(""); // OriginalLearnStartDate
        //                (output[0] as IDictionary<string, object>).Values.ToArray()[74].Should().Be("1.10000");
        //            }
        //        }
        //    }
        // }
    }
}
