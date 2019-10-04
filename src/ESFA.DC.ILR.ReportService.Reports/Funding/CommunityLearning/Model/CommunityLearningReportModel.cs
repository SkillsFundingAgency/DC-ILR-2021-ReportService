using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model
{
    public class CommunityLearningReportModel : ICommunityLearningReport
    {
        public CommunityLearningReportModel(
          IDictionary<string, string> headerData,
          List<ICategory> categories,
          IDictionary<string, string> footerData)
        {
            Categories = categories ?? new List<ICategory>();
            HeaderData = headerData ?? new Dictionary<string, string>();
            FooterData = footerData ?? new Dictionary<string, string>();
        }

        public IDictionary<string, string> HeaderData { get; }

        public List<ICategory> Categories { get; }

        public IDictionary<string, string> FooterData { get; }
    }
}
