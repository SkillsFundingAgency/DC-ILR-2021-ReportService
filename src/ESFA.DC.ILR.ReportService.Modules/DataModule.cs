using Autofac;
using ESFA.DC.ILR.ReportService.Data;
using ESFA.DC.ILR.ReportService.Data.Providers;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Modules
{
    public class DataModule : Module
    {
        public DataModule(bool fm36Switch)
        {
            FM36Switch = fm36Switch;
        }

        public bool FM36Switch { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReportServiceContextKeysMutator>().As<IReportServiceContextKeysMutator>();
            builder.RegisterType<IlrReferenceDataProviderService>().Keyed<IExternalDataProvider>(DependentDataCatalog.ReferenceData);
            builder.RegisterType<ValidIlrFileServiceProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ValidIlr);
            builder.RegisterType<InputIlrFileServiceProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.InputIlr);
            builder.RegisterType<IlrValidationErrorsProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ValidationErrors);
            builder.RegisterType<Fm25Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm25);
            builder.RegisterType<Fm35Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm35);

            if (FM36Switch)
            {
                builder.RegisterType<Fm36Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm36);
            }
            else
            {
                builder.RegisterType<Fm36ProviderStub>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm36);
            }           

            builder.RegisterType<Fm81Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm81);
            builder.RegisterType<Fm99Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm99);
            builder.RegisterType<EasProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Eas);
            builder.RegisterType<FrmReferenceDataProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Frm);
        }
    }
}
