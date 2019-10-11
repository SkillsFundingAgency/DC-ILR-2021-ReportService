using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model
{
    public class Category : ICategory
    {
        public string CategoryName { get; set; }

        public List<ISubCategory> SubCategories { get; set; }

        public int TotalLearners => SubCategories.Sum(x => x.TotalLearners);

        public int TotalStartedInFundingYear => SubCategories.Sum(x => x.TotalStartedInFundingYear);

        public int TotalEnrolmentsInFundingYear => SubCategories.Sum(x => x.TotalEnrolmentsInFundingYear);
    }
}
