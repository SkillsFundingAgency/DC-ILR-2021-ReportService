using System;

namespace ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd
{
    public class AppsMonthlyPaymentModel
    {
        public string LearnerReferenceNumber { get; set; }

        public long UniqueLearnerNumber { get; set; }

        public string OfficialSensitive { get; }
    }
}