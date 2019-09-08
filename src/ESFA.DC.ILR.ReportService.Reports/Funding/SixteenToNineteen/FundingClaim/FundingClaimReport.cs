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
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim
{
    public class FundingClaimReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<FundingClaimReportModel> _modelBuilder;
        private readonly IExcelService _excelService;

        public FundingClaimReport(
            IFileNameService fileNameService,
            IModelBuilder<FundingClaimReportModel> modelBuilder,
            IExcelService excelService
           )
            : base(ReportTaskNameConstants.FundingClaim1619Report, "16-19 Funding Claim Report")
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


            model.ProviderName = "Provider XYZ";
            model.Ukprn = 987654321;
            model.IlrFile = "ILR-12345678-1920-20191005-151322-01.xml";
            model.Year = "2019/20";
            model.FundingFactor = new FundingFactorModel()
            {
                AreaCostFact1618Hist = "0.12345",
                ProgWeightHist = "1.61",
                PrvDisadvPropnHist = "0.222",
                PrvHistLrgProgPropn = "0.354",
                PrvRetentFactHist = "1.0152"
            };

            model.DirectFundingStudents = new FundingLineReportingBandModel()
            {
                Band5StudentNumbers = 1,
                Band4aStudentNumbers = 2,
                Band4bStudentNumbers = 3,
                Band3StudentNumbers = 4,
                Band2StudentNumbers = 5,
                Band1StudentNumbers = 6,
                Band5TotalFunding = 1,
                Band4aTotalFunding = 2,
                Band4bTotalFunding = 3,
                Band3TotalFunding = 4,
                Band2TotalFunding = 5,
                Band1TotalFunding = 6
            };

            model.StudentsIncludingHNS = new FundingLineReportingBandModel()
            {
                Band5StudentNumbers = 11,
                Band4aStudentNumbers = 12,
                Band4bStudentNumbers = 13,
                Band3StudentNumbers = 14,
                Band2StudentNumbers = 15,
                Band1StudentNumbers = 16,
                Band5TotalFunding = 11,
                Band4aTotalFunding =12,
                Band4bTotalFunding = 13,
                Band3TotalFunding = 14,
                Band2TotalFunding = 15,
                Band1TotalFunding = 16
            };

            model.StudentsWithEHCPlan = new FundingLineReportingBandModel()
            {
                Band5StudentNumbers = 21,
                Band4aStudentNumbers = 22,
                Band4bStudentNumbers = 23,
                Band3StudentNumbers = 24,
                Band2StudentNumbers = 25,
                Band1StudentNumbers = 26,
                Band5TotalFunding = 21,
                Band4aTotalFunding = 22,
                Band4bTotalFunding = 23,
                Band3TotalFunding = 24,
                Band2TotalFunding = 25,
                Band1TotalFunding = 26
            };


            model.ContinuingStudentsExcludingEHCPlan = new FundingLineReportingBandModel()
            {
                Band5StudentNumbers = 31,
                Band4aStudentNumbers = 32,
                Band4bStudentNumbers = 33,
                Band3StudentNumbers = 34,
                Band2StudentNumbers = 35,
                Band1StudentNumbers = 36,
                Band5TotalFunding = 31,
                Band4aTotalFunding = 32,
                Band4bTotalFunding = 33,
                Band3TotalFunding = 34,
                Band2TotalFunding = 35,
                Band1TotalFunding = 36
            };

            model.ComponentSetVersion = "12";
            model.ApplicationVersion = "11.22.3300.4321";
            model.FilePreparationDate = "06/11/2019";
            model.LarsData = "LVersion 3.0.0: 17 Sep 2019 08:13:35:643";
            model.OrganisationData = "OVersion 3.0.0: 17 Sep 2019 08:13:35:643";
            model.PostcodeData = "PVersion 3.0.0: 17 Sep 2019 08:13:35:643";
            model.LargeEmployerData = "LEVersion 3.0.0: 17 Sep 2019 08:13:35:643";
            model.CofRemovalData = "CLEVersion 3.0.0: 17 Sep 2019 08:13:35:643";
            model.CofRemoval = (decimal) -10.22;

            var workbook = _excelService.BindExcelTemplateToWorkbook(model, "FundingClaim1619ReportTemplate.xlsx", "FundingClaim");
            await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
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
