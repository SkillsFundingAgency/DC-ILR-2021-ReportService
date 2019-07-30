using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved.Model
{
    public class DevolvedAdultEducationOccupancyReportModel : AbstractOccupancyReportModel
    {
        public LearningDeliveryValue Fm35LearningDelivery { get; set; }

        public LearningDeliveryPeriodisedValuesModel PeriodisedValues { get; set; }

        public string McaGlaShortCode { get; set; }
    }
}
