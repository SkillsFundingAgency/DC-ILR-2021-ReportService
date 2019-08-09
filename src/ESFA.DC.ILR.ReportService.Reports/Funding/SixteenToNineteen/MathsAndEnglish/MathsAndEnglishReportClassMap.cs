using CsvHelper.Configuration;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.MathsAndEnglish
{
    public class MathsAndEnglishReportClassMap : ClassMap<MathsAndEnglishReportModel>
    {
        public MathsAndEnglishReportClassMap()
        {
            var index = 0;

            Map(m => m.FM25Learner.FundLine).Name(@"Funding line type").Index(++index);
            Map(m => m.Learner.LearnRefNumber).Name(@"Learner reference number").Index(++index);
            Map(m => m.Learner.FamilyName).Name(@"Family name").Index(++index);
            Map(m => m.Learner.GivenNames).Name(@"Given names").Index(++index);
            Map(m => m.Learner.DateOfBirthNullable).Name(@"Date of birth").Index(++index);
            Map(m => m.Learner.CampId).Name(@"Campus identifier").Index(++index);
            Map(m => m.FM25Learner.ConditionOfFundingMaths).Name(@"Maths GCSE status").Index(++index);
            Map(m => m.FM25Learner.ConditionOfFundingEnglish).Name(@"English GCSE status").Index(++index);
            Map(m => m.FM25Learner.RateBand).Name(@"Funding band").Index(++index);
            Map().Name(@"OFFICIAL-SENSITIVE").Constant(string.Empty).Index(++index);
        }
    }
}
