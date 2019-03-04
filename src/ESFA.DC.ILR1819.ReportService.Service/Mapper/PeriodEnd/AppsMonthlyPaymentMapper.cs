using CsvHelper.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR1819.ReportService.Service.Mapper.PeriodEnd
{
    public class AppsMonthlyPaymentMapper : ClassMap<AppsMonthlyPaymentModel>, IClassMapper
    {
        public AppsMonthlyPaymentMapper()
        {
            int i = 0;
            Map(m => m.LearnerReferenceNumber).Index(i++).Name("Learner reference number");
            Map(m => m.UniqueLearnerNumber).Index(i++).Name("Unique learner number");
            Map(m => m.OfficialSensitive).Index(i++).Name("OFFICIAL - SENSITIVE");
        }
    }
}
