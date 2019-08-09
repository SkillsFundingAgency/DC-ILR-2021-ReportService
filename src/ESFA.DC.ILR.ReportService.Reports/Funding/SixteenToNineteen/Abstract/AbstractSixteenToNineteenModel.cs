using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen
{
    public abstract class AbstractSixteenToNineteenModel
    {
        public ILearner Learner { get; set; }

        public FM25Learner FM25Learner { get; set; }
    }
}
