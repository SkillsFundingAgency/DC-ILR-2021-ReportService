using System;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM07
{
    public class Frm07ReportModel : FrmBaseReportModel
    {
        public string PrevOrgName { get; set; }

        public string PMOrgName { get; set; }

        public int? DevolvedUKPRN { get; set; }

        public string DevolvedOrgName { get; set; }

        public int AimTypeCode { get; set; }

        public string LearnAimType { get; set; }

        public int FundModel { get; set; }

        public DateTime? OrigLearnStartDate { get; set; }

        public string SOFCode { get; set; }
    }
}
