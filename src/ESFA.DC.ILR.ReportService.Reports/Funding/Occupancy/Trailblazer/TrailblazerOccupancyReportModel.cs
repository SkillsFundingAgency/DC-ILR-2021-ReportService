using ESFA.DC.ILR.ReportService.Models.Fm81;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;
using ESFA.DC.ILR.ReportService.Reports.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer
{
    public class TrailblazerOccupancyReportModel : AbstractOccupancyReportModel
    {
        public LearningDeliveryValue Fm81LearningDelivery { get; set; }

        public AppFinRecordModel AppFinRecord { get; set; }
    }
}
