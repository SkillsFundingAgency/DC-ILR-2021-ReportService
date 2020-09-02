using System;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM06
{
    public class Frm06ReportModel : FrmBaseReportModel
    {
        public string PrevOrgName { get; internal set; }
        public string PMOrgName { get; internal set; }
        public int AimTypeCode { get; internal set; }
        public string LearningAimType { get; internal set; }
        public int FundingModel { get; internal set; }
        public DateTime? OrigLearnStartDate { get; internal set; }
        public string SOFCode { get; internal set; }
    }
}
