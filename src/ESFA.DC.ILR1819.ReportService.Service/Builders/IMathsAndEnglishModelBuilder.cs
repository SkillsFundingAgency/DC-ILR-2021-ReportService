using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Service.Models;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public interface IMathsAndEnglishModelBuilder
    {
        MathsAndEnglishModel BuildModel(ILearner learner, Learner fm25Data);
    }
}