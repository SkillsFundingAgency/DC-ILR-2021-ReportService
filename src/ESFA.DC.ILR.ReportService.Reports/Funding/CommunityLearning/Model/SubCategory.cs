using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model
{
    public class SubCategory : ISubCategory
    {
        public string SubCategoryName { get; set; }

        public int Total { get; set; }
    }
}
