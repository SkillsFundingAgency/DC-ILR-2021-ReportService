using CsvHelper.Configuration;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding
{
    public class SummaryOfFM35FundingReportClassMap : ClassMap<SummaryOfFM35FundingReportModel>
    {
        public SummaryOfFM35FundingReportClassMap()
        {
            var index = 0;

            Map(m => m.FundingLineType).Name(@"Funding Line Type").Index(++index);
            Map(m => m.Period).Name(@"Period").Index(++index);
            Map(m => m.OnProgramme).Name(@"On-programme (£)").Index(++index);
            Map(m => m.Balancing).Name(@"Balancing (£)").Index(++index);
            Map(m => m.JobOutcomeAchievement).Name(@"Job Outcome Achievement (£)").Index(++index);
            Map(m => m.AimAchievement).Name(@"Aim Achievement (£)").Index(++index);
            Map(m => m.TotalAchievement).Name(@"Total Achievement (£)").Index(++index);
            Map(m => m.LearningSupport).Name(@"Learning Support (£)").Index(++index);
            Map(m => m.Total).Name(@"Total (£)").Index(++index);

            Map().Name(@"OFFICIAL - SENSITIVE").Constant(string.Empty).Index(++index);
        }
    }
}
