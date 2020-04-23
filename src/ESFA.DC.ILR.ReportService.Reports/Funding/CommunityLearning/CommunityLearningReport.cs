using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning
{
    public class CommunityLearningReport : AbstractExcelReport<CommunityLearningReportModel>, IReport
    {
        public CommunityLearningReport(
            IFileNameService fileNameService,
            IModelBuilder<CommunityLearningReportModel> modelBuilder,
            IExcelFileService excelService)
            : base(
                  fileNameService, 
                  modelBuilder,
                  excelService,
                  ReportTemplateConstants.CommunityLearningTemplateName,
                  ReportTemplateConstants.CommunityLearningDataSource,
                  ReportTaskNameConstants.CommunityLearningReport,
                  ReportNameConstants.CommunityLearning)
        {
        }

        public virtual IEnumerable<Type> DependsOn
            => new[] 
            {
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
            };

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var filenames = await GenerateExcelAsync(reportServiceContext, reportsDependentData, cancellationToken);

            return filenames;
        }
    }
}
