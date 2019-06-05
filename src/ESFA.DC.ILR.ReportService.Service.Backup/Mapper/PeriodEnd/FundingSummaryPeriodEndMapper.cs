using CsvHelper.Configuration;
using ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR.ReportService.Service.Mapper.PeriodEnd
{
    public class FundingSummaryPeriodEndMapper : ClassMap<FundingSummaryPeriodEndModel>
    {
        public FundingSummaryPeriodEndMapper()
        {
            int i = 0;
            Map(m => m.OfficialSensitive).Index(i++).Name("OFFICIAL - SENSITIVE");
        }
    }
}
