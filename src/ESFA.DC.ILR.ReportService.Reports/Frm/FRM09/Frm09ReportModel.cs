using System;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM09
{
    public class Frm09ReportModel : FrmBaseReportModel
    {
        public string PMOrgName;
        public int? DAUKPRN;
        public string DAOrgName;
        public string SOFCode;
        public string PrevOrgName { get; set; }
        public int AimTypeCode { get; set; }
        public DateTime? OrigLearnStartDate { get; set; }
        public string WithdrawalCode { get; set; }
        public int FundModel { get; set; }
        public string LearnAimType { get; set; }
    }
}
