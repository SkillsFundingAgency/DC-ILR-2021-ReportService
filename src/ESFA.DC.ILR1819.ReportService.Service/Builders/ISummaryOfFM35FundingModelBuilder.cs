using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Service.Models;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public interface ISummaryOfFM35FundingModelBuilder
    {
        IList<SummaryOfFm35FundingModel> BuildModel(LearningDeliveryAttribute fundLineData);
    }
}