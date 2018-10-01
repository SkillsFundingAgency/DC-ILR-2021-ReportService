using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IAppsAdditionalPaymentsModelBuilder
    {
        AppsAdditionalPaymentsModel BuildModel(ILearner learner, FM36Learner learnerData);
    }
}