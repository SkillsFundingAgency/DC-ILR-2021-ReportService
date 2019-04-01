using System.Collections.Generic;
using ESFA.DC.ILR1819.ReportService.Model.Eas;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR.ReportService.Interface.Builders
{
    public interface IEasBuilder
    {
        FundingSummaryModel BuildWithEasSubValueLine(
            string title,
            List<EasSubmissionValues> easSubmissionValues,
            string paymentTypeName,
            int period);
    }
}
