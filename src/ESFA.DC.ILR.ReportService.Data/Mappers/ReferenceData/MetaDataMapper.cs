using System.Linq;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class MetaDataMapper : IMetaDataMapper
    {
        public MetaData MapData(ReferenceDataService.Model.MetaData.MetaData metaData)
        {
            return new MetaData()
            {
                DateGenerated = metaData.DateGenerated,
                ReferenceDataVersions = MapReferenceDataVersions(metaData.ReferenceDataVersions),
                ValidationErrors = metaData.ValidationErrors?.Select(MapValidationErrors).ToList(),
                ValidationRules = metaData.ValidationRules?.Select(MapValidationRule).ToList(),
                Lookups = metaData.Lookups.Select(MapLookUp).ToList(),
                CollectionDates = MapIlrCollectionDates(metaData.CollectionDates)
            };
        }

        private ReferenceDataVersion MapReferenceDataVersions(ReferenceDataService.Model.MetaData.ReferenceDataVersion metaDataReferenceDataVersions)
        {
            return new ReferenceDataVersion()
            {
                CoFVersion = MapCofVersion(metaDataReferenceDataVersions.CoFVersion),
                CampusIdentifierVersion = MapCampusIdentifierVersion(metaDataReferenceDataVersions.CampusIdentifierVersion),
                Employers = MapEmployersVersion(metaDataReferenceDataVersions.Employers),
                LarsVersion = MapLarsVersion(metaDataReferenceDataVersions.LarsVersion),
                OrganisationsVersion = MapOrganisationsVersion(metaDataReferenceDataVersions.OrganisationsVersion),
                PostcodesVersion = MapPostcodesVersion(metaDataReferenceDataVersions.PostcodesVersion),
                EasUploadDateTime = MapEas(metaDataReferenceDataVersions.EasUploadDateTime)
            };
        }

        private ValidationError MapValidationErrors(ReferenceDataService.Model.MetaData.ValidationError validationError)
        {
            return new ValidationError()
            {
                RuleName = validationError.RuleName,
                Severity = (ValidationError.SeverityLevel) validationError.Severity,
                Message = validationError.Message
            };
        }

        private ValidationRule MapValidationRule(ReferenceDataService.Model.MetaData.ValidationRule validationRule)
        {
            return new ValidationRule()
            {
                RuleName = validationRule.RuleName,
                Desktop = validationRule.Desktop,
                Online = validationRule.Online
            };
        }

        private Lookup MapLookUp(ReferenceDataService.Model.MetaData.Lookup lookup)
        {
            return new Lookup()
            {
                Name = lookup.Name,
                Code = lookup.Code,
                EffectiveFrom = lookup.EffectiveFrom,
                EffectiveTo = lookup.EffectiveTo,
                SubCategories = lookup.SubCategories?.Select(MapLookUpSubCategory).ToList()
            };
        }

        private LookupSubCategory MapLookUpSubCategory(ReferenceDataService.Model.MetaData.LookupSubCategory lookupSubCategory)
        {
            return new LookupSubCategory()
            {
                Code = lookupSubCategory.Code,
                EffectiveFrom = lookupSubCategory.EffectiveFrom,
                EffectiveTo = lookupSubCategory.EffectiveTo
            };
        }

        private IlrCollectionDates MapIlrCollectionDates(ReferenceDataService.Model.MetaData.CollectionDates.IlrCollectionDates ilrCollectionDates)
        {
            return new IlrCollectionDates()
            {
                ReturnPeriods = ilrCollectionDates.ReturnPeriods?.Select(MapReturnPeriod).ToList(),
                CensusDates = ilrCollectionDates.CensusDates.Select(MapCensusDate).ToList()
            };
        }

        private ReturnPeriod MapReturnPeriod(ReferenceDataService.Model.MetaData.CollectionDates.ReturnPeriod returnPeriod)
        {
            return new ReturnPeriod()
            {
                Name = returnPeriod.Name,
                Period = returnPeriod.Period,
                Start = returnPeriod.Start,
                End = returnPeriod.End
            };
        }

        private CensusDate MapCensusDate(ReferenceDataService.Model.MetaData.CollectionDates.CensusDate censusDate)
        {
            return new CensusDate()
            {
                Period = censusDate.Period,
                Start = censusDate.Start,
                End = censusDate.End
            };
        }

        private CoFVersion MapCofVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.CoFVersion coFVersion)
        {
            return new CoFVersion()
            {
                Version = coFVersion.Version
            };
        }

        private CampusIdentifierVersion MapCampusIdentifierVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.CampusIdentifierVersion campusIdentifierVersion)
        {
            return new CampusIdentifierVersion()
            {
                Version = campusIdentifierVersion.Version
            };
        }

        private EmployersVersion MapEmployersVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.EmployersVersion employers)
        {
            return new EmployersVersion()
            {
                Version = employers.Version
            };
        }

        private LarsVersion MapLarsVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.LarsVersion larsVersion)
        {
            return new LarsVersion()
            {
                Version = larsVersion.Version
            };
        }

        private OrganisationsVersion MapOrganisationsVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.OrganisationsVersion organisationsVersion)
        {
            return new OrganisationsVersion()
            {
                Version = organisationsVersion.Version
            };
        }

        private PostcodesVersion MapPostcodesVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.PostcodesVersion postcodesVersion)
        {
            return new PostcodesVersion()
            {
                Version = postcodesVersion.Version
            };
        }

        private EasUploadDateTime MapEas(ReferenceDataService.Model.MetaData.ReferenceDataVersions.EasUploadDateTime easUploadDateTime)
        {
            return new EasUploadDateTime()
            {
                UploadDateTime = easUploadDateTime.UploadDateTime
            };
        }
    }
}
