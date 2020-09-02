using System;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM08
{
    public class Frm08ReportModel : FrmBaseReportModel
    {
        public string LearnAimType { get; set; }

        public int FundModel { get; set; }

        public DateTime? OrigLearnStartDate { get; set; }

        public string SOFCode { get; set; }

        public int AimTypeCode { get; set; }
    }
}
