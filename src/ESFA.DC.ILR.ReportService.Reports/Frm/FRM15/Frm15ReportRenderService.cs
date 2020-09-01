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
                "Previous Organisation Name",
                "Pre-Merger UKPRN",
                "Pre-Merger Organisation Name",
                "Unique Learner Number",
                "Learner Reference Number",
                "Software Supplier Aim Identifier",
                "Learning Aim Reference",
                "Learning Aim Title",
                "Aim Sequence Number",
                "Aim Type Code",
                "Standard Code",
                "Framework Code",
                "Pathway Code",
                "Programme Type Code",
                "Learning Start Date",
                "Original Learning Start Date",
                "Learning Planned End Date",
                "Learning Actual End Date",
                "Completion Status Code",
                "Learning Outcome Code",
                "Funding Model",
                "Source Of Funding Code",
                "Advanced Learner Loans Indicator",
                "Restart Indicator",
                "Provider Specified Delivery Monitoring",
                "Provider Specified Learner Monitoring",
                "Prior Learning Funding Adjustment",
                "Other Funding Adjustment",
                "End Point Assessment Organisation Identifier",
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
                model.PrevOrgName,
                model.PMUKPRN,
                model.PMOrgName,
                model.ULN,
                model.LearnRefNumber,
                model.SWSupAimId,
                model.LearnAimRef,
                model.LearnAimTitle,
                model.AimSeqNumber,
                model.AimTypeCode,
                model.StdCode,
                model.FworkCode,
                model.PwayCode,
                model.ProgType,
                model.LearnStartDate,
                model.OrigLearnStartDate,
                model.LearnPlanEndDate,
                model.LearnActEndDate,
                model.CompStatus,
                model.Outcome,
                model.FundModel,
                model.SOFCode,
                model.AdvancedLoansIndicator,
                model.ResIndicator,
                model.ProvSpecLearnDelMon,
                model.ProvSpecDelMon,
                model.PriorLearnFundAdj,
                model.OtherFundAdj,
                model.EPAOrgId,
                model.TotalNegotiatedAssessmentPrice,
                model.AssessmentPaymentReceived
            }, row, 0, false);

            return worksheet;
        }
    }
}
