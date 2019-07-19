using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model;
using ESFA.DC.ILR.ReportService.Reports.Interface;
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
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileNameService>().As<IFileNameService>();
            
            builder.RegisterType<ValidationErrorsDetailReport>().As<IReport>();
            builder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>();
            builder.RegisterType<FrontEndValidationReport>().As<IFrontEndValidationReport>();
            builder.RegisterType<ValidationErrorsDetailReportBuilder>().As<IValidationErrorsReportBuilder>();
            builder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>();

            builder.RegisterType<DevolvedAdultEducationOccupancyReport>().As<IReport>();
            builder.RegisterType<DevolvedAdultEducationOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>>();

            builder.RegisterType<CsvService>().As<ICsvService>();
        }
    }
}
