using System;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM06
{
    public class Frm06ReportModel : FrmBaseReportModel
    {
        public string PrevOrgName { get; set; }
        public string PMOrgName { get; set; }
        public int AimTypeCode { get; set; }
        public string LearningAimType { get; set; }
        public int FundingModel { get; set; }
        public DateTime? OrigLearnStartDate { get; set; }
        public string SOFCode { get; set; }
    }
}
