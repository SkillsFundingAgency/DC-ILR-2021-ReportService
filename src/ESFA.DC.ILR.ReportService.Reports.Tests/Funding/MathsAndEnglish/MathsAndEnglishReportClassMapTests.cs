using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.MathsAndEnglish;
using ESFA.DC.ILR.ReportService.Reports.Tests.Abstract;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.MathsAndEnglish
{
    public class MathsAndEnglishReportClassMapTests : AbstractCsvClassMapTests<MathsAndEnglishReportModel, MathsAndEnglishReportClassMap>
    {
        protected override IEnumerable<string> OrderedColumns => new[]
        {
            "Funding line type",
            "Learner reference number",
            "Family name",
            "Given names",
            "T Level Student",
            "Date of birth",
            "Campus identifier",
            "Maths GCSE status",
            "English GCSE status",
            "Funding band",
            "OFFICIAL-SENSITIVE",
        };
        
        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<MathsAndEnglishReportModel>()
            {
                new MathsAndEnglishReportModel()
                {
                    Learner = new TestLearner()
                    {
                        LearnRefNumber = "Test"
                    },
                    FM25Learner = new FM25Learner()
                }
            };

            var output = WriteAndReadModel(input).ToList();

            (output[0] as IDictionary<string, object>).Values.ToArray()[1].Should().Be("Test");
        }
    }
}
