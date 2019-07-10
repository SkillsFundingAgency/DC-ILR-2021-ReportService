using Autofac;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Validation.Detail;
using ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Desktop.Modules
{
    public class ReportsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileNameService>().As<IFileNameService>();

            // Builders 
            builder.RegisterType<ValidationErrorsDetailReportBuilder>().As<IValidationErrorsReportBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>().InstancePerLifetimeScope();

            //Reports
            builder.RegisterType<ValidationErrorsDetailReport>().As<IReport>();
            builder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>();
            builder.RegisterType<FrontEndValidationReport>().As<IFrontEndValidationReport>();
            
            builder.RegisterType<CsvService>().As<ICsvService>();
        }
    }
}
