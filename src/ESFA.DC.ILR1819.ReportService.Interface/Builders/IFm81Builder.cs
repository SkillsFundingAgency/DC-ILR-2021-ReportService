using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders
{
    public interface IFm81Builder
    {
        FundingSummaryModel BuildWithFundLine(
            string title,
            FM81Global fm81Global,
            List<string> validLearners,
            string fundLine,
            string[] attributes);
    }
}
