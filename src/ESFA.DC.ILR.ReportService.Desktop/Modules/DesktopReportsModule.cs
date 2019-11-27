using Autofac;
using ESFA.DC.ILR.ReportService.Modules;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Desktop;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Desktop;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim.Desktop;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;

namespace ESFA.DC.ILR.ReportService.Desktop.Modules
{
    public class DesktopReportsModule : ReportsModule
    {
        protected override void RegisterFundingSummaryReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DesktopFundingSummaryReport>().As<IReport>();
            containerBuilder.RegisterType<DesktopFundingSummaryReportModelBuilder>().As<IModelBuilder<IFundingSummaryReport>>();
            containerBuilder.RegisterType<FundingSummaryReportRenderService>().As<IRenderService<IFundingSummaryReport>>();
            containerBuilder.RegisterType<PeriodisedValuesLookupProvider>().As<IPeriodisedValuesLookupProvider>();
        }

        protected override void RegisterDevolvedAdultEducationFundingSummaryReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DevolvedAdultEducationFundingSummaryReport>().As<IReport>();
            containerBuilder.RegisterType<DesktopDevolvedAdultEducationFundingSummaryModelBuilder>().As<IModelBuilder<IEnumerable<DevolvedAdultEducationFundingSummaryReportModel>>>();
            containerBuilder.RegisterType<DevolvedAdultEducationFundingSummaryReportRenderService>().As<IRenderService<IDevolvedAdultEducationFundingSummaryReport>>();
        }

        protected override void RegisterAdultFundingClaimReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DesktopAdultFundingClaimReport>().As<IReport>();
            containerBuilder.RegisterType<AdultFundingClaimReportModelBuilder>().As<IModelBuilder<AdultFundingClaimReportModel>>();
        }
    }
}
