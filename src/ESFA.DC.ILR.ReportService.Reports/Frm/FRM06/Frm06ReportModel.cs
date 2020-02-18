using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM06
{
    public class Frm06ReportModel
    {
        public string Return { get; set; }

        public long UKPRN { get; set; }

        public string OrgName { get; set; }

        public string PartnerUKPRN { get; set; } 

        public string PartnerOrgName { get; set; }

        public string PrevUKPRN { get; set; }

        public string PMUKPRN { get; set; }

        public long ULN { get; set; }

        public string LearnRefNumber { get; set; }

        public string PrevLearnRefNumber { get; set; }
        
        public string LearnAimRef { get; set; }

        public int AimSeqNumber { get; set; }

        public string LearnAimTitle { get; set; }

        public string StdCode { get; set; }

        public string FworkCode { get; set; }

        public string PwayCode { get; set; }

        public string ProgType { get; set; }

        public string AdvancedLoansIndicator { get; set; }

        public string SWSupAimId { get; set; }

        public string ProvSpecDelMon { get; set; }

        public string ProvSpecLearnDelMon { get; set; }
        
        public DateTime LearnStartDate { get; set; }

        public DateTime LearnPlanEndDate { get; set; }

        public DateTime? LearnActEndDate { get; set; }

        public string ResIndicator { get; set; }

        public string PriorLearnFundAdj { get; set; }

        public string OtherFundAdj { get; set; }

        public int CompStatus { get; set; }

        public string Outcome { get; set; }
        
        public string FundingStream { get; set; }
    }
}
