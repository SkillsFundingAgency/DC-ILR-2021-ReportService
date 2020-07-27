using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM09
{
    public class Frm09ReportRenderService : FrmBaseRenderService<Frm09ReportModel>
    {
        private const string ReportId = "FRM09";

        public Frm09ReportRenderService() 
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
                "Previous Organisation Name",
                "Pre-Merger UKPRN",
                "Pre-Merger Organisation Name",
                "Devolved Authority UKPRN",
                "Devolved Authority Name",
                "Unique Learner Number",
                "Learner Reference Number",
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
                "Withdrawal Reason Code",
                "Learning Outcome Code",
                "Funding Model",
                "Source of Funding Code",
                "Advanced Learner Loans Indicator",
                "Funding Stream",
                "Restart Indicator",
                "Software Supplier Aim Identifier",
                "Provider Specified Delivery Monitoring",
                "Provider Specified Learner Monitoring",
                "Prior Learning Funding Adjustment",
                "Other Funding Adjustment"
            };

        protected override Worksheet RenderReportRow(Worksheet worksheet, int row, Frm09ReportModel model)
        {
            worksheet.Cells.ImportObjectArray(new object[]
            {
                ReportId,
                model.Return,
                model.UKPRN,
                model.OrgName,
                model.PrevOrgName,
                model.PMUKPRN,
                model.PMOrgName,
                model.DAUKPRN,
                model.DAOrgName,
                model.ULN,
                model.LearnRefNumber,
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
                model.WithdrawalCode,
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
                model.OtherFundAdj,
            }, row, 0, false);

            return worksheet;
        }
    }
}
