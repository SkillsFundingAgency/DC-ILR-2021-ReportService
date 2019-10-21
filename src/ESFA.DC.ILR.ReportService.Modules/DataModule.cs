using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Data.Mappers;
using ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData;
using ESFA.DC.ILR.ReportService.Data.Providers;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.Fm36;
using ESFA.DC.ILR.ReportService.Models.Fm81;
using ESFA.DC.ILR.ReportService.Models.Fm99;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.EAS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Modules
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IlrReferenceDataProviderService>().Keyed<IExternalDataProvider>(DependentDataCatalog.ReferenceData);
            builder.RegisterType<ValidIlrFileServiceProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ValidIlr);
            builder.RegisterType<InvalidIlrFileServiceProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.InvalidIlr);
            builder.RegisterType<IlrValidationErrorsProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ValidationErrors);
            builder.RegisterType<Fm25Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm25);
            builder.RegisterType<Fm35Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm35);
            builder.RegisterType<Fm36Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm36);
            builder.RegisterType<Fm81Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm81);
            builder.RegisterType<Fm99Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm99);

            builder.RegisterType<ReportsDependentDataPopulationService>().As<IReportsDependentDataPopulationService>();

            builder.RegisterType<Fm25Mapper>().As<IMapper<FundingService.FM25.Model.Output.FM25Global, FM25Global>>();
            builder.RegisterType<Fm35Mapper>().As<IMapper<FundingService.FM35.FundingOutput.Model.Output.FM35Global, FM35Global>>();
            builder.RegisterType<Fm36Mapper>().As<IMapper<FundingService.FM36.FundingOutput.Model.Output.FM36Global, FM36Global>>();
            builder.RegisterType<Fm81Mapper>().As<IMapper<FundingService.FM81.FundingOutput.Model.Output.FM81Global, FM81Global>>();
            builder.RegisterType<Fm99Mapper>().As<IMapper<FundingService.ALB.FundingOutput.Model.Output.ALBGlobal, ALBGlobal>>();

            builder.RegisterType<ReferenceDataMapper>().As<IMapper<ReferenceDataService.Model.ReferenceDataRoot, ReferenceDataRoot>>();
            builder.RegisterType<ApprenticeshipEarningsHistoryMapper>().As<IMapper<IEnumerable<ReferenceDataService.Model.AppEarningsHistory.ApprenticeshipEarningsHistory>, IReadOnlyCollection<ApprenticeshipEarningsHistory>>>();
            builder.RegisterType<DevolvedPostcodeMapper>().As<IMapper<ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcodes, DevolvedPostcodes>>();
            builder.RegisterType<EasFundingLineMapper>().As<IMapper<IEnumerable<ReferenceDataService.Model.EAS.EasFundingLine>, IReadOnlyCollection<EasFundingLine>>>();
            builder.RegisterType<FcsContractAllocationMapper>().As<IMapper<IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation>, IReadOnlyCollection<FcsContractAllocation>>>();
            builder.RegisterType<LarsLearningDeliveryMapper>().As<IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDelivery>, IReadOnlyCollection<LARSLearningDelivery>>>();
            builder.RegisterType<LarsStandardMapper>().As<IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSStandard>, IReadOnlyCollection<LARSStandard>>>();
            builder.RegisterType<MetaDataMapper>().As<IMapper<ReferenceDataService.Model.MetaData.MetaData, MetaData>>();
            builder.RegisterType<OrganisationMapper>().As<IMapper<IEnumerable<ReferenceDataService.Model.Organisations.Organisation>, IReadOnlyCollection<Organisation>>>();
            builder.RegisterType<PostcodeMapper>().As<IMapper<IEnumerable<ReferenceDataService.Model.Postcodes.Postcode>, IReadOnlyCollection<Postcode>>>();
        }
    }
}
