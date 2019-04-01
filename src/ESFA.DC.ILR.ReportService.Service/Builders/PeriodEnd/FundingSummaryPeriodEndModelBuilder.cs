using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders.PeriodEnd
{
    public class FundingSummaryPeriodEndModelBuilder : IFundingSummaryPeriodEndModelBuilder
    {
        public FundingSummaryPeriodEndModel BuildModel(ILearner learner)
        {
            return new FundingSummaryPeriodEndModel()
            {
            };
        }
    }
}
