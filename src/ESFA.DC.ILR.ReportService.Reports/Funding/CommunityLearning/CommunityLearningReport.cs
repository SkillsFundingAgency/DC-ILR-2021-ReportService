using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning
{
    public class CommunityLearningReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<CommunityLearningReportModel> _communityLearningReportModelBuilder;
        private readonly IExcelService _excelService;

        public CommunityLearningReport(
            IFileNameService fileNameService,
            IModelBuilder<CommunityLearningReportModel> communityLearningReportModelBuilder,
            IExcelService excelService)
            : base(ReportTaskNameConstants.CommunityLearningReport, "CL Summary of Learners by Non-Single Budget Category Report")
        {
            _fileNameService = fileNameService;
            _communityLearningReportModelBuilder = communityLearningReportModelBuilder;
            _excelService = excelService;
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
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);
            var communityLearningReportModel = _communityLearningReportModelBuilder.Build(reportServiceContext, reportsDependentData);

            var workbook = _excelService.BindExcelTemplateToWorkbook(communityLearningReportModel, "CommunityLearningReportTemplate.xlsx", "CommunityLearning");

            await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);

            return new[] { fileName };
        }
    }
}
