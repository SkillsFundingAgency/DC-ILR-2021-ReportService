using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.ALLB;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Validation.Detail;
using ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

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
            containerBuilder.RegisterType<PeriodisedValuesLookupProvider>().As<IPeriodisedValuesLookupProvider>();
        }

        private void RegisterAppsIndicativeEarningsReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AppsIndicativeEarningsReport>().As<IReport>();
            containerBuilder.RegisterType<AppsIndicativeEarningsReportModelBuilder>().As<IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>>>();
        }
    }
}
