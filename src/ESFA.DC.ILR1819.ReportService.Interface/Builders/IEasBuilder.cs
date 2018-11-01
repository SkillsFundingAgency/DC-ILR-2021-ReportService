using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders
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
