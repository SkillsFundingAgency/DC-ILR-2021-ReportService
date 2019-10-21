using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract
{
    public abstract class AbstractSixteenToNineteenModel
    {
        public ILearner Learner { get; set; }

        public FM25Learner FM25Learner { get; set; }
    }
}
