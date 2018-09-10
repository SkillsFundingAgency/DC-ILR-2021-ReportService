using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IAppsIndicativeCommand
    {
        void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute);
    }
}