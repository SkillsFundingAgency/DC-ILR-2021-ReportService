using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<HighNeedsStudentSummaryReportModel> _modelBuilder;
        private readonly IExcelService _excelService;

        public HighNeedsStudentSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<HighNeedsStudentSummaryReportModel> modelBuilder,
            IExcelService excelService
           )
            : base(ReportTaskNameConstants.HNSSummaryReport, "High Needs Students Summary Report")
        {
            _fileNameService = fileNameService;
            _modelBuilder = modelBuilder;
            _excelService = excelService;
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, FileName, OutputTypes.Excel);
            var model = _modelBuilder.Build(reportServiceContext, reportsDependentData);

            //model.ProviderName = "Provider XYZ";
            //model.Ukprn = 987654321;
            //model.IlrFile = "ILR-12345678-1920-20191005-151322-01.xml";
            //model.Year = "2019/20";

            //model.TotalDirectFunded1416_WithEHCP = 1;
            //model.TotalDirectFunded1416_WithoutEHCP = 2;
            //model.TotalDirectFunded1416_HNSWithoutEHCP = 3;
            //model.TotalDirectFunded1416_HNSWithEHCP = 4;
            //model.TotalDirectFunded1416_EHCPWithoutHNS = 5;

            //model.Total1619IncludingHNS_WithEHCP = 6;
            //model.Total1619IncludingHNS_WithoutEHCP = 7;
            //model.Total1619IncludingHNS_HNSWithoutEHCP = 8;
            //model.Total1619IncludingHNS_HNSWithEHCP = 9;
            //model.Total1619IncludingHNS_EHCPWithoutHNS = 10;

            //model.Total1924WithEHCP_WithEHCP = 11;
            //model.Total1924WithEHCP_WithoutEHCP = 12;
            //model.Total1924WithEHCP_HNSWithoutEHCP = 13;
            //model.Total1924WithEHCP_HNSWithEHCP = 14;
            //model.Total1924WithEHCP_EHCPWithoutHNS = 15;

            //model.Total19PlusWithoutEHCP_WithEHCP = 16;
            //model.Total19PlusWithoutEHCP_WithoutEHCP = 17;
            //model.Total19PlusWithoutEHCP_HNSWithoutEHCP = 18;
            //model.Total19PlusWithoutEHCP_HNSWithEHCP = 19;
            //model.Total19PlusWithoutEHCP_EHCPWithoutHNS = 20;



            //model.ComponentSetVersion = "12";
            //model.ApplicationVersion = "11.22.3300.4321";
            //model.FilePreparationDate = "06/11/2019";
            //model.LarsData = "LVersion 3.0.0: 17 Sep 2019 08:13:35:643";
            //model.OrganisationData = "OVersion 3.0.0: 17 Sep 2019 08:13:35:643";
            //model.PostcodeData = "PVersion 3.0.0: 17 Sep 2019 08:13:35:643";
            //model.LargeEmployerData = "LEVersion 3.0.0: 17 Sep 2019 08:13:35:643";

            var designer = new WorkbookDesigner
            {
                Workbook = _excelService.GetWorkbookFromTemplate("HNSSummaryReportTemplate.xlsx")
            };
            designer.SetDataSource("HNSSummary", new List<HighNeedsStudentSummaryReportModel>{ model });
            designer.Process();
            await _excelService.SaveWorkbookAsync(designer.Workbook, fileName, reportServiceContext.Container, cancellationToken);
            return new[] { fileName };
        }

     
        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm25,
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
            };
    }
}
