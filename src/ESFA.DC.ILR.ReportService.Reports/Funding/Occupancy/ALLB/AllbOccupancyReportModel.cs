using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.ALLB
{
    public class AllbOccupancyReportModel : AbstractOccupancyReportModel
    {
        public LearningDeliveryValue Fm99LearningDelivery { get; set; }
    }
}
