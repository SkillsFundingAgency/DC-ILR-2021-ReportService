using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM15
{
    public class Frm15ReportRenderService : FrmBaseRenderService<Frm15ReportModel>
    {
        private const string ReportId = "FRM15";

        public Frm15ReportRenderService() 
            : base(ReportId)
        {
        }

        protected override object[] ColumnNames
            => new object[]
            {
                "Report ID",
                "Return",
                "UK Provider Reference Number",
                "Organisation Name",
                "Subcontracted or Partnership UKPRN",
                "Subcontracted or Partnership Organisation Name",
                "Previous UKPRN",
                "Pre-Merger UKPRN",
                "Unique Learner Number",
                "Learner Reference Number",
                "Previous Learner Reference Number",
                "Learning Aim Reference",
                "Aim Sequence Number",
                "Learning Aim Title",
                "Standard Code",
                "Framework Code",
                "Pathway Code",
                "Programme Type Code",
                "Advanced Learner Loans Indicator",
                "Software Supplier Aim Identifier",
                "Provider Specified Delivery Monitoring",
                "Provider Specified Learner Monitoring",
                "Learning Start Date",
                "Learning Planned End Date",
                "Learning Actual End Date",
                "Restart Indicator",
                "Prior Learning Funding Adjustment",
                "Other Funding Adjustment",
                "End Point Assessment Organisation Identifier",
                "Completion Status Code",
                "Learning Outcome Code",
                "Funding Stream",
                "Total Negotiated Assessment Price",
                "Assessment Payments Received"
            };

        protected override Worksheet RenderReportRow(Worksheet worksheet, int row, Frm15ReportModel model)
        {
            worksheet.Cells.ImportObjectArray(new object[]
            {
                ReportId,
                model.Return,
                model.UKPRN,
                model.OrgName,
                model.PartnerUKPRN,
                model.PartnerOrgName,
                model.PrevUKPRN,
                model.PMUKPRN,
                model.ULN,
                model.LearnRefNumber,
                model.PrevLearnRefNumber,
                model.LearnAimRef,
                model.AimSeqNumber,
                model.LearnAimTitle,
                model.StdCode,
                model.FworkCode,
                model.PwayCode,
                model.ProgType,
                model.AdvancedLoansIndicator,
                model.SWSupAimId,
                model.ProvSpecDelMon,
                model.ProvSpecLearnDelMon,
                model.LearnStartDate,
                model.LearnPlanEndDate,
                model.LearnActEndDate,
                model.ResIndicator,
                model.PriorLearnFundAdj,
                model.OtherFundAdj,
                model.EPAOrgId,
                model.CompStatus,
                model.Outcome,
                model.FundingStream,
                model.TotalNegotiatedAssessmentPrice,
                model.AssessmentPaymentReceived
            }, row, 0, false);

            return worksheet;
        }
    }
}
