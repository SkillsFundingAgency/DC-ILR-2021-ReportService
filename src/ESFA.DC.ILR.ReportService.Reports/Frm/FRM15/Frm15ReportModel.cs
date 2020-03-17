namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM15
{
    public class Frm15ReportModel : FrmBaseReportModel
    {
        public string EPAOrgId { get; set; }

        public int? TotalNegotiatedAssessmentPrice { get; set; }

        public int? AssessmentPaymentReceived { get; set; }
    }
}
