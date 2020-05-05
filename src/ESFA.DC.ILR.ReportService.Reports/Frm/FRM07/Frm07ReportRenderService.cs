using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM07
{
    public class Frm07ReportRenderService : FrmBaseRenderService<Frm07ReportModel>
    {
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
                "Devolved Authority UKPRN",
                "Devolved Authority Name",
                "Unique Learner Number",
                "Learner Reference Number",
                "Previous Learner Reference Number",
                "Learning Aim Reference",
                "Aim Sequence Number",
                "Aim Type Code",
                "Learning Aim Title",
                "Learning Aim Type",
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
                "Fund Model",
                "Source Of Funding Code",
                "Advanced Learner Loans Indicator",
                "Funding Stream",
                "Restart Indicator",
                "Software Supplier Aim Identifier",
                "Provider Specified Delivery Monitoring",
                "Provider Specified Learner Monitoring",
                "Prior Learning Funding Adjustment",
                "Other Funding Adjustment"
            };

        public Frm07ReportRenderService()
            : base("FRM07")
        {
        }

        protected override Worksheet RenderReportRow(Worksheet worksheet, int row, Frm07ReportModel model)
        {
            worksheet.Cells.ImportObjectArray(new object[]
            {
                _reportID,
                model.Return,
                model.UKPRN,
                model.OrgName,
                model.PartnerUKPRN,
                model.PartnerOrgName,
                model.PrevUKPRN,
                model.PrevOrgName,
                model.PMUKPRN,
                model.PMOrgName,
                model.DevolvedUKPRN,
                model.DevolvedOrgName,
                model.ULN,
                model.LearnRefNumber,
                model.PrevLearnRefNumber,
                model.LearnAimRef,
                model.AimSeqNumber,
                model.AimTypeCode,
                model.LearnAimTitle,
                model.LearnAimType,
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
                model.FundingStream,
                model.ResIndicator,
                model.SWSupAimId,
                model.ProvSpecDelMon,
                model.ProvSpecLearnDelMon,
                model.PriorLearnFundAdj,
                model.OtherFundAdj
            }, row, 0, false);

            return worksheet;
        }
    }
}
