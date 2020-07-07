using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.Ilr;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;
using ESFA.DC.ILR.ReportService.Reports.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.NonContractDevolved
{
    public class NonContractDevolvedAdultEducationOccupancyReportModel : AbstractOccupancyReportModel
    {
        public LearningDeliveryValue Fm35LearningDelivery { get; set; }

        public ILearnerEmploymentStatus LearnerEmploymentStatus { get; set; }

        public EmploymentStatusMonitoringModel EmploymentStatusMonitorings { get; set; }

        public string McaGlaShortCode { get; set; }

        public string EntitlementCategoryLevel2Or3 { get; set; }

        public string LocalAuthorityCode { get; set; }

        public string PartnershipProviderName { get; set; }
    }
}
