using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim
{
    public class AdultFundingClaimReport : AbstractExcelReport<AdultFundingClaimReportModel>, IReport
    {
        private const int FisInfoRow = 12;
  
        public AdultFundingClaimReport(
           IFileNameService fileNameService,
           IModelBuilder<AdultFundingClaimReportModel> modelBuilder,
           IExcelFileService excelService,
           bool desktopMode = false
          )
           : base(
                 fileNameService,
                 modelBuilder,
                 excelService,
                 ReportTemplateConstants.AdultFundingClaimTemplateName,
                 ReportTemplateConstants.AdultFundingClaimDataSource,
                 ReportTaskNameConstants.AdultFundingClaimReport,
                 ReportNameConstants.AdultFundingClaim)
        {
            DesktopMode = desktopMode;
        }

        public bool DesktopMode { get; }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var filenames = await GenerateExcelAsync(reportServiceContext, reportsDependentData, cancellationToken, DesktopMode == false ? FisInfoRow : (int?)null);

            return filenames;
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm35,
                DependentDataCatalog.Fm99,
                DependentDataCatalog.Eas,
                DependentDataCatalog.ReferenceData
            };

    }
}
