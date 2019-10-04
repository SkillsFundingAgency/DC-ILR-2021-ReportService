using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface
{
    public interface ICategory
    {
        string CategoryName { get; set; }

        List<ISubCategory> SubCategories { get; set; }

        int Total { get; }
    }
}