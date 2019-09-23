using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship;
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
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Validation.Detail;
using ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding;

namespace ESFA.DC.ILR.ReportService.Modules
{
    public class ReportsModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            RegisterValidationReports(containerBuilder);
          
            RegisterDevolvedAdultEducationOccupancyReport(containerBuilder);
            RegisterMainOccupancyReport(containerBuilder);
            RegisterAllbOccupancyReport(containerBuilder);
            RegisterAppsIndicativeEarningsReport(containerBuilder);

            RegisterFundingSummaryReport(containerBuilder);

            RegisterSummaryOfFM35FundingReport(containerBuilder);

            RegisterTrailblazerEmployerIncentivesReport(containerBuilder);
            RegisterTrailblazerOccupancyReport(containerBuilder);

            RegisterDevolvedAdultEducationFundingSummaryReport(containerBuilder);

            RegisterMathsAndEnglishReport(containerBuilder);
            RegisterHighNeedsStudentReport(containerBuilder);
            RegisterSummaryOfFundingByStudentReport(containerBuilder);
            RegisterFundingClaim1619Report(containerBuilder);
            RegisterAdultFundingClaimReport(containerBuilder);

            containerBuilder.RegisterType<IlrModelMapper>().As<IIlrModelMapper>();

            containerBuilder.RegisterType<CsvService>().As<ICsvService>();
            containerBuilder.RegisterType<ExcelService>().As<IExcelService>();
        }

        private void RegisterValidationReports(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ValidationErrorsDetailReport>().As<IReport>();
            containerBuilder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>();
            containerBuilder.RegisterType<FrontEndValidationReport>().As<IFrontEndValidationReport>();
            containerBuilder.RegisterType<ValidationErrorsDetailReportBuilder>().As<IValidationErrorsReportBuilder>();
            containerBuilder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>();
        }

        private void RegisterDevolvedAdultEducationOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DevolvedAdultEducationOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<DevolvedAdultEducationOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>>();
        }

        private void RegisterMainOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MainOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<MainOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<MainOccupancyReportModel>>>();
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
            containerBuilder.RegisterType<SummaryPageRenderService>().As<IRenderService<ISummaryPage>>();
            containerBuilder.RegisterType<PeriodisedValuesLookupProvider>().As<IPeriodisedValuesLookupProvider>();
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
            containerBuilder.RegisterType<FundingClaimReport>().As<IReport>();
            containerBuilder.RegisterType<FundingClaimReportModelBuilder>().As<IModelBuilder<FundingClaimReportModel>>();
        }
        private void RegisterAdultFundingClaimReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AdultFundingClaimReport>().As<IReport>();
            containerBuilder.RegisterType<AdultFundingClaimReportModelBuilder>().As<IModelBuilder<AdultFundingClaimReportModel>>();
        }
    }
}
