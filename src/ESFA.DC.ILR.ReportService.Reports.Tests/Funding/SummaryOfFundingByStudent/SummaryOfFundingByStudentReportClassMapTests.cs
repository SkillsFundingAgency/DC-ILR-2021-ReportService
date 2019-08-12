using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.SummaryOfFundingByStudent;
using ESFA.DC.ILR.ReportService.Reports.Tests.Abstract;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.SummaryOfFundingByStudent
{
    public class SummaryOfFundingByStudentReportClassMapTests : AbstractCsvClassMapTests<SummaryOfFundingByStudentReportModel, SummaryOfFundingByStudentReportClassMap>
    {
        protected override IEnumerable<string> OrderedColumns => new[]
        {
            "Funding line type",
            "Learner reference number",
            "Family name",
            "Given names",
            "Date of birth",
            "Campus identifier",
            "Planned learning hours",
            "Planned employability, enrichment and pastoral hours",
            "Total planned hours",
            "Funding band",
            "Qualifies for funding",
            "Total funding",
            "OFFICIAL-SENSITIVE",
        };
        
        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<SummaryOfFundingByStudentReportModel>()
            {
                new SummaryOfFundingByStudentReportModel()
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
