using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim
{
    public class FundingClaimReport : AbstractExcelReport<FundingClaimReportModel>, IReport, IFilteredReport
    {
        public FundingClaimReport(
            IFileNameService fileNameService,
            IModelBuilder<FundingClaimReportModel> modelBuilder,
            IExcelFileService excelService)
            : base(
                  fileNameService,
                  modelBuilder,
                  excelService,
                  ReportTemplateConstants.FundingClaimTemplateName,
                  ReportTemplateConstants.FundingClaimDataSource,
                  ReportTaskNameConstants.FundingClaim1619Report,
                  ReportNameConstants.SixteenNineteenFundingClaim)
        {
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var filenames = await GenerateExcelAsync(reportServiceContext, reportsDependentData, cancellationToken);

            return filenames;
        }


        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm25,
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
            };

        public IReportFilterDefinition Filter => new ReportFilterDefinition()
        {
            ReportName = ReportName,
            Properties = new IReportFilterPropertyDefinition[]
            {
                new ReportFilterPropertyDefinition<DateTime?>(ReportingConstants.ReferenceDateFilterPropertyName),
            }
        };
    }
}
