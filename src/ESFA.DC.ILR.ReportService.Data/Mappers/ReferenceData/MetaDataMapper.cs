using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class MetaDataMapper : IMapper<ReferenceDataService.Model.MetaData.MetaData, MetaData>
    {
        public MetaData MapData(ReferenceDataService.Model.MetaData.MetaData metaData)
        {
            return new MetaData()
            {
                DateGenerated = metaData.DateGenerated,
                ReferenceDataVersions = MapReferenceDataVersions(metaData.ReferenceDataVersions),
                ValidationErrors = metaData.ValidationErrors?.Select(MapValidationErrors).ToList(),
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

        private IlrCollectionDates MapIlrCollectionDates(ReferenceDataService.Model.MetaData.CollectionDates.IlrCollectionDates ilrCollectionDates)
        {
            return new IlrCollectionDates()
            {
                CensusDates = ilrCollectionDates.CensusDates.Select(MapCensusDate).ToList()
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
