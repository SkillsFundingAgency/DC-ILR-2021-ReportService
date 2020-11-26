using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM15;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM08
{
    public class Frm08ReportRenderService : FrmBaseRenderService<Frm08ReportModel>
    {
        public Frm08ReportRenderService() 
            : base("FRM08")
        {
        }

        protected override object[] ColumnNames => new object[]
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
            "Funding Model",
            "Source of Funding Code",
            "Advanced Learner Loans Indicator",
            "Restart Indicator",
            "Provider Specified Delivery Monitoring",
            "Provider Specified Learner Monitoring",
            "Prior Learning Funding Adjustment",
            "Other Funding Adjustment"
        };

        protected override Worksheet RenderReportRow(Worksheet worksheet, int row, Frm08ReportModel model)
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
                model.ULN,
                model.LearnRefNumber,
                model.SWSupAimId,
                model.LearnAimRef,
                model.LearnAimTitle,
                model.AimSeqNumber,
                model.AimTypeCode,
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
                model.ResIndicator,
                model.ProvSpecLearnDelMon,
                model.ProvSpecDelMon,
                model.PriorLearnFundAdj,
                model.OtherFundAdj
            }, row, 0, false);

            return worksheet;
        }
    }
}
