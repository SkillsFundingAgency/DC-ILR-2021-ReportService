using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model
{
    public class CommunityLearningReportModel : AbstractReportHeaderFooterModel
    {
        public ICategory TotalCommunityLearning { get; set; }

        public ICategory PersonalAndCommunityDevelopment { get; set;  }

        public ICategory NeigbourhoodLearning { get; set; }

        public ICategory FamilyEnglishMaths { get; set; }

        public ICategory WiderFamilyLearning { get; set; }
    }
}
