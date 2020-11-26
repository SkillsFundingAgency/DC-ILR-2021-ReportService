using System;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public abstract class FrmBaseReportModel
    {
        public string Return { get; set; }

        public long UKPRN { get; set; }

        public string OrgName { get; set; }

        public long? PartnerUKPRN { get; set; }

        public string PartnerOrgName { get; set; }

        public long? PrevUKPRN { get; set; }

        public string PrevOrgName { get; set; }

        public long? PMUKPRN { get; set; }

        public string PMOrgName { get; set; }

        public long ULN { get; set; }

        public string LearnRefNumber { get; set; }

        public string PrevLearnRefNumber { get; set; }

        public string LearnAimRef { get; set; }

        public int AimSeqNumber { get; set; }

        public string LearnAimTitle { get; set; }

        public int? StdCode { get; set; }

        public int? FworkCode { get; set; }

        public int? PwayCode { get; set; }

        public int? ProgType { get; set; }

        public string AdvancedLoansIndicator { get; set; }

        public string SWSupAimId { get; set; }

        public string ProvSpecDelMon { get; set; }

        public string ProvSpecLearnDelMon { get; set; }

        public DateTime LearnStartDate { get; set; }

        public DateTime LearnPlanEndDate { get; set; }

        public DateTime? LearnActEndDate { get; set; }

        public string ResIndicator { get; set; }

        public int? PriorLearnFundAdj { get; set; }

        public int? OtherFundAdj { get; set; }

        public int? OtjActHours { get; set; }

        public int CompStatus { get; set; }

        public int? Outcome { get; set; }

        public string FundingStream { get; set; }
    }
}
