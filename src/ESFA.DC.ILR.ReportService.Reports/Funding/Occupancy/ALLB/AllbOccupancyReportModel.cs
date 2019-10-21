using ESFA.DC.ILR.ReportService.Models.Fm99;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.ALLB
{
    public class AllbOccupancyReportModel : AbstractOccupancyReportModel
    {
        public LearningDeliveryValue Fm99LearningDelivery { get; set; }
    }
}
