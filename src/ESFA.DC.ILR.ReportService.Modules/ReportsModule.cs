using System.Collections.Generic;
using Autofac;
using ESFA.DC.CsvService;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ExcelService;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Reports.Frm;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM06;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM07;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM08;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM15;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.ALLB;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.MathsAndEnglish;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.SummaryOfFundingByStudent;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding.Model;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Validation.Detail;
using ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Summary;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main.AEBSTFInitiativesOccupancy;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.NonContractDevolved;


namespace ESFA.DC.ILR.ReportService.Modules
{
    public class ReportsModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            RegisterValidationReports(containerBuilder);
          
            RegisterDevolvedAdultEducationOccupancyReport(containerBuilder);
            RegisterNonContractDevolvedAdultEducationOccupancyReport(containerBuilder);
            RegisterMainOccupancyReport(containerBuilder);
            RegisterAEBSTFInitiativesOccupancyReport(containerBuilder);
            RegisterAllbOccupancyReport(containerBuilder);

            RegisterAppsIndicativeEarningsReport(containerBuilder);
            RegisterNonContractsAppsActivityReport(containerBuilder);

            RegisterFundingSummaryReport(containerBuilder);

            RegisterSummaryOfFM35FundingReport(containerBuilder);

            RegisterTrailblazerEmployerIncentivesReport(containerBuilder);
            RegisterTrailblazerOccupancyReport(containerBuilder);

            RegisterDevolvedAdultEducationFundingSummaryReport(containerBuilder);

            RegisterCommunityLearningReport(containerBuilder);

            RegisterMathsAndEnglishReport(containerBuilder);
            RegisterHighNeedsStudentReport(containerBuilder);
            RegisterSummaryOfFundingByStudentReport(containerBuilder);
            RegisterFundingClaim1619Report(containerBuilder);
            RegisterAdultFundingClaimReport(containerBuilder);

            RegisterFrmReports(containerBuilder);

            containerBuilder.RegisterType<AcademicYearService>().As<IAcademicYearService>();
            containerBuilder.RegisterType<IlrModelMapper>().As<IIlrModelMapper>();

            containerBuilder.RegisterType<CsvFileService>().As<ICsvFileService>();
            containerBuilder.RegisterType<ExcelFileService>().As<IExcelFileService>();

            containerBuilder.RegisterType<ReportsDependentDataPopulationService>().As<IReportsDependentDataPopulationService>();
        }

        private void RegisterValidationReports(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ValidationErrorsDetailReport>().As<IReport>();
            containerBuilder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>();
            containerBuilder.RegisterType<RuleViolationSummaryReport>().As<IReport>();
            containerBuilder.RegisterType<RuleViolationSummaryReportModelBuilder>().As<IModelBuilder<RuleViolationSummaryReportModel>>();
            containerBuilder.RegisterType<FrontEndValidationReport>().As<IFrontEndValidationReport>();
            containerBuilder.RegisterType<ValidationErrorsDetailReportBuilder>().As<IValidationErrorsReportBuilder>();
            containerBuilder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>();
        }

        private void RegisterDevolvedAdultEducationOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DevolvedAdultEducationOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<DevolvedAdultEducationOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>>();
        }

        private void RegisterNonContractDevolvedAdultEducationOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<NonContractDevolvedAdultEducationOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<NonContractDevolvedAdultEducationOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<NonContractDevolvedAdultEducationOccupancyReportModel>>>();
        }

        private void RegisterMainOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MainOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<MainOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<MainOccupancyReportModel>>>();
        }

        private void RegisterAEBSTFInitiativesOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AEBSTFInitiativesOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<AEBSTFInitiativesOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<AEBSTFInitiativesOccupancyReportModel>>>();
        }

        private void RegisterAllbOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AllbOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<AllbOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<AllbOccupancyReportModel>>>();
        }

        private void RegisterTrailblazerOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TrailblazerOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<TrailblazerOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<TrailblazerOccupancyReportModel>>>();
        }

        protected virtual void RegisterFundingSummaryReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingSummaryReport>().As<IReport>();
            containerBuilder.RegisterType<FundingSummaryReportModelBuilder>().As<IModelBuilder<IFundingSummaryReport>>();
            containerBuilder.RegisterType<FundingSummaryReportRenderService>().As<IRenderService<IFundingSummaryReport>>();
            containerBuilder.RegisterType<PeriodisedValuesLookupProvider>().As<IPeriodisedValuesLookupProvider>();
        }

        protected virtual void RegisterCommunityLearningReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<CommunityLearningReport>().As<IReport>();
            containerBuilder.RegisterType<CommunityLearningReportModelBuilder>().As<IModelBuilder<CommunityLearningReportModel>>();
        }


        private void RegisterSummaryOfFM35FundingReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummaryOfFM35FundingReport>().As<IReport>();
            containerBuilder.RegisterType<SummaryOfFM35FundingReportModelBuilder>().As<IModelBuilder<IEnumerable<SummaryOfFM35FundingReportModel>>>();
        }

        protected virtual void RegisterDevolvedAdultEducationFundingSummaryReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DevolvedAdultEducationFundingSummaryReport>().As<IReport>();
            containerBuilder.RegisterType<DevolvedAdultEducationFundingSummaryReportModelBuilder>().As<IModelBuilder<IEnumerable<DevolvedAdultEducationFundingSummaryReportModel>>>();
            containerBuilder.RegisterType<DevolvedAdultEducationFundingSummaryReportRenderService>().As<IRenderService<IDevolvedAdultEducationFundingSummaryReport>>();
        }

        private void RegisterAppsIndicativeEarningsReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AppsIndicativeEarningsReport>().As<IReport>();
            containerBuilder.RegisterType<AppsIndicativeEarningsReportModelBuilder>().As<IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>>>();
        }

        private void RegisterNonContractsAppsActivityReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<NonContractedAppsActivityReport>().As<IReport>();
            containerBuilder.RegisterType<NonContractedAppsActivityReportModelBuilder>().As<IModelBuilder<IEnumerable<NonContractedAppsActivityReportModel>>>();
        }

        private void RegisterTrailblazerEmployerIncentivesReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TrailblazerEmployerIncentivesReport>().As<IReport>();
            containerBuilder.RegisterType<TrailblazerEmployerIncentiveReportModelBuilder>().As<IModelBuilder<IEnumerable<TrailblazerEmployerIncentivesReportModel>>>();
        }

        private void RegisterMathsAndEnglishReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MathsAndEnglishReport>().As<IReport>();
            containerBuilder.RegisterType<MathsAndEnglishReportModelBuilder>().As<IModelBuilder<IEnumerable<MathsAndEnglishReportModel>>>();
        }

        private void RegisterHighNeedsStudentReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<HighNeedsStudentDetailReport>().As<IReport>();
            containerBuilder.RegisterType<HighNeedsStudentDetailReportModelBuilder>().As<IModelBuilder<IEnumerable<HighNeedsStudentDetailReportModel>>>();

            containerBuilder.RegisterType<HighNeedsStudentSummaryReport>().As<IReport>();
            containerBuilder.RegisterType<HighNeedsStudentSummaryReportModelBuilder>().As<IModelBuilder<HighNeedsStudentSummaryReportModel>>();
        }

        private void RegisterSummaryOfFundingByStudentReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummaryOfFundingByStudentReport>().As<IReport>();
            containerBuilder.RegisterType<SummaryOfFundingByStudentModelBuilder>().As<IModelBuilder<IEnumerable<SummaryOfFundingByStudentReportModel>>>();
        }

        private void RegisterFundingClaim1619Report(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingClaimReport>().As<IReport>().As<IFilteredReport>();
            containerBuilder.RegisterType<FundingClaimReportModelBuilder>().As<IModelBuilder<FundingClaimReportModel>>();
        }

        private void RegisterFrmReports(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FrmReport>().As<IReport>();
            containerBuilder.RegisterType<FrmLearnerComparer>().As<IEqualityComparer<FrmLearnerKey>>();

            containerBuilder.RegisterType<Frm06Report>().As<IWorksheetReport>();
            containerBuilder.RegisterType<Frm06ReportModelBuilder>().As<IModelBuilder<IEnumerable<Frm06ReportModel>>>();
            containerBuilder.RegisterType<Frm06ReportRenderService>().As<IRenderService<IEnumerable<Frm06ReportModel>>>();

            containerBuilder.RegisterType<Frm07Report>().As<IWorksheetReport>();
            containerBuilder.RegisterType<Frm07ReportModelBuilder>().As<IModelBuilder<IEnumerable<Frm07ReportModel>>>();
            containerBuilder.RegisterType<Frm07ReportRenderService>().As<IRenderService<IEnumerable<Frm07ReportModel>>>();

            containerBuilder.RegisterType<Frm08Report>().As<IWorksheetReport>();
            containerBuilder.RegisterType<Frm08ReportModelBuilder>().As<IModelBuilder<IEnumerable<Frm08ReportModel>>>();
            containerBuilder.RegisterType<Frm08ReportRenderService>().As<IRenderService<IEnumerable<Frm08ReportModel>>>();

            containerBuilder.RegisterType<Frm15Report>().As<IWorksheetReport>();
            containerBuilder.RegisterType<Frm15ReportModelBuilder>().As<IModelBuilder<IEnumerable<Frm15ReportModel>>>();
            containerBuilder.RegisterType<Frm15ReportRenderService>().As<IRenderService<IEnumerable<Frm15ReportModel>>>();
        }

        protected virtual void RegisterAdultFundingClaimReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AdultFundingClaimReport>().As<IReport>();
            containerBuilder.RegisterType<AdultFundingClaimReportModelBuilder>().As<IModelBuilder<AdultFundingClaimReportModel>>();
        }
    }
}
