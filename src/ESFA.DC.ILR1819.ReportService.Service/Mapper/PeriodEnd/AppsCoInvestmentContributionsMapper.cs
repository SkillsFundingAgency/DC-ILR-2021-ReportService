using CsvHelper.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR1819.ReportService.Service.Mapper.PeriodEnd
{
    public sealed class AppsCoInvestmentContributionsMapper : ClassMap<AppsCoInvestmentContributionsModel>, IClassMapper
    {
        public AppsCoInvestmentContributionsMapper()
        {
            int i = 0;
            Map(m => m.LearnRefNumber).Index(i++).Name("Learner reference number");
            Map(m => m.UniqueLearnerNumber).Index(i++).Name("Unique learner number");
            Map(m => m.ProgType).Index(i++).Name("Programme type");
            Map(m => m.OfficialSensitive).Index(i).Name("OFFICIAL - SENSITIVE");
        }
    }
}
