using System.Collections.Generic;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public abstract class FrmBaseRenderService<T> : IRenderService<IEnumerable<T>> where T : FrmBaseReportModel
    {
        private readonly Style _defaultStyle;
        private readonly Style _dateTimeStyle;

        protected readonly string _reportID;
        protected virtual object[] ColumnNames
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
                "Completion Status Code",
                "Learning Outcome Code",
                "Funding Stream"
            };

        protected FrmBaseRenderService(string reportId)
        {
            _reportID = reportId;

            var cellsFactory = new CellsFactory();

            _defaultStyle = cellsFactory.CreateStyle();
            _dateTimeStyle = cellsFactory.CreateStyle();

            ConfigureStyles();
        }

        public virtual Worksheet Render(IEnumerable<T> models, Worksheet worksheet)
        {
            worksheet.Workbook.DefaultStyle = _defaultStyle;

            RenderTitleRow(worksheet);

            foreach (var model in models)
            {
                var row = NextRow(worksheet);
                RenderReportRow(worksheet, row, model);
            }

            worksheet.AutoFitColumn(0);

            return worksheet;
        }

        protected virtual Worksheet RenderReportRow(Worksheet worksheet, int row, T model)
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
                model.CompStatus,
                model.Outcome,
                model.FundingStream
            }, row, 0, false);

            return worksheet;
        }

        private Worksheet RenderTitleRow(Worksheet worksheet)
        {
            var row = NextRow(worksheet);

            worksheet.Cells.ImportObjectArray(ColumnNames, row, 0, false);

            return worksheet;
        }

        private int NextRow(Worksheet worksheet)
        {
            return worksheet.Cells.MaxRow + 1;
        }

        private void ConfigureStyles()
        {
            _defaultStyle.Font.Size = 10;
            _defaultStyle.Font.Name = "Arial";

            _dateTimeStyle.Font.Size = 10;
            _dateTimeStyle.Font.Name = "Arial";
            _dateTimeStyle.SetCustom("yyyy-MM-dd", false);
        }
    }
}