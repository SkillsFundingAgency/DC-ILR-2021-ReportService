using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface ISummaryOfFM35FundingModelBuilder
    {
        IList<SummaryOfFm35FundingModel> BuildModel(LearningDeliveryAttribute fundLineData);
    }
}