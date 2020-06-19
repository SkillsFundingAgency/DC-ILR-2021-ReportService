using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.Ilr;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved
{
    public class DevolvedAdultEducationOccupancyReportModel : AbstractOccupancyReportModel
    {
        public LearningDeliveryValue Fm35LearningDelivery { get; set; }

        public MessageLearnerLearnerEmploymentStatus LearnerEmploymentStatus { get; set; }

        public string McaGlaShortCode { get; set; }

        public string EntitlementCategoryLevel2Or3 { get; set; }

        public string PartnershipProviderName { get; set; }

        public string LocalAuthorityCode { get; set; }
    }
}
