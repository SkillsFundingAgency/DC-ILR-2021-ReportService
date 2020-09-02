using System;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM09
{
    public class Frm09ReportModel : FrmBaseReportModel
    {
        public string PMOrgName { get; set; }

        public int? DevolvedUKPRN { get; set; }

        public string DevolvedOrgName { get; set; }

        public string SOFCode { get; set; }

        public string PrevOrgName { get; set; }

        public int AimTypeCode { get; set; }

        public DateTime? OrigLearnStartDate { get; set; }

        public int? WithdrawalCode { get; set; }

        public int FundModel { get; set; }

        public string LearnAimType { get; set; }
    }
}
