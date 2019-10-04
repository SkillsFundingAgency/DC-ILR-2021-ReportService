using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning
{
    public class CommunityLearningReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<ICommunityLearningReport> _communityLearningReportModelBuilder;
        private readonly IExcelService _excelService;
        private readonly IRenderService<ICommunityLearningReport> _communityLearningReportRenderService;

        public CommunityLearningReport(
            IFileNameService fileNameService,
            IModelBuilder<ICommunityLearningReport> communityLearningReportModelBuilder,
            IExcelService excelService,
            IRenderService<ICommunityLearningReport> communityLearningReportRenderService)
            : base(ReportTaskNameConstants.CommunityLearningReport, "CL Summary of Learners by Non-Single Budget Category Report")
        {
            _fileNameService = fileNameService;
            _communityLearningReportModelBuilder = communityLearningReportModelBuilder;
            _excelService = excelService;
            _communityLearningReportRenderService = communityLearningReportRenderService;
        }

        public virtual IEnumerable<Type> DependsOn
            => new[] 
            {
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
            };

        public async Task<IEnumerable<string>> GenerateAsync(
            IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var communityLearningReportModel = _communityLearningReportModelBuilder.Build(reportServiceContext, reportsDependentData);

            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);

            using (var workbook = _excelService.NewWorkbook())
            {
                workbook.Worksheets.Clear();

                var worksheet = _excelService.GetWorksheetFromWorkbook(workbook, "CommunityLearningReport");

                _communityLearningReportRenderService.Render(communityLearningReportModel, worksheet);

                await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
            }

            return new[] { fileName };
        }
    }
}
