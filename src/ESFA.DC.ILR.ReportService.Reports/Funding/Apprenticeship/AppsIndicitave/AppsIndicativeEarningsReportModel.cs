using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.Abstract.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave
{
    public class AppsIndicativeEarningsReportModel : AbstractAppsReportModel
    {
        public PeriodisedValuesModel PeriodisedValues { get; set; }

        public ILearnerEmploymentStatus EmploymentStatus { get; set; }

        public string StandardNotionalEndLevel { get; set; }

        public int? EmpStatusMonitoringSmallEmployer { get; set; }

        public decimal TotalPRMPreviousFundingYear { get; set; }

        public decimal TotalPRMThisFundingYear { get; set; }
    }
}
