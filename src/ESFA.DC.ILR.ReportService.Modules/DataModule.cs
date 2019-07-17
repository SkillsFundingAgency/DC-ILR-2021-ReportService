using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Data.Providers;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Modules
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IlrReferenceDataProviderService>().Keyed<IExternalDataProvider>(typeof(ReferenceDataRoot));
            builder.RegisterType<IlrFileServiceProvider>().Keyed<IExternalDataProvider>(typeof(IMessage));
            builder.RegisterType<IlrValidationErrorsProvider>().Keyed<IExternalDataProvider>(typeof(List<ValidationErrors.Interface.Models.ValidationError>));
            builder.RegisterType<Fm35Provider>().Keyed<IExternalDataProvider>(typeof(FM35Global));

            builder.RegisterType<ReportsDependentDataPopulationService>().As<IReportsDependentDataPopulationService>();
        }
    }
}
