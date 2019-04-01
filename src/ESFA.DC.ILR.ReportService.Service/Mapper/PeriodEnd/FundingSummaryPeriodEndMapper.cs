using CsvHelper.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR1819.ReportService.Service.Mapper.PeriodEnd
{
    public class FundingSummaryPeriodEndMapper : ClassMap<FundingSummaryPeriodEndModel>, IClassMapper
    {
        public FundingSummaryPeriodEndMapper()
        {
            int i = 0;
            Map(m => m.OfficialSensitive).Index(i++).Name("OFFICIAL - SENSITIVE");
        }
    }
}
