using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model
{
    public class SubCategory : ISubCategory
    {
        public int TotalLearners { get; set; }

        public int TotalStartedInFundingYear { get; set; }

        public int TotalEnrolmentsInFundingYear { get; set; }
    }
}
