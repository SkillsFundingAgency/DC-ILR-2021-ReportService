using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class DevolvedPostcodeMapper : IMapper<ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcodes, DevolvedPostcodes>
    {
        public DevolvedPostcodes MapData(ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcodes postcodes)
        {
            return new DevolvedPostcodes()
            {
                McaGlaSofLookups = postcodes?.McaGlaSofLookups?.Select(MapMcaGlaSofLookup).ToList(),
                Postcodes = postcodes?.Postcodes?.Select(MapDevolvedPostcode).ToList()
            };
        }

        private McaGlaSofLookup MapMcaGlaSofLookup(ReferenceDataService.Model.PostcodesDevolution.McaGlaSofLookup mcaGlaSofLookup)
        {
            return new McaGlaSofLookup()
            {
                McaGlaFullName = mcaGlaSofLookup.McaGlaFullName,
                McaGlaShortCode = mcaGlaSofLookup.McaGlaShortCode,
                SofCode = mcaGlaSofLookup.SofCode,
                EffectiveTo = mcaGlaSofLookup.EffectiveTo,
                EffectiveFrom = mcaGlaSofLookup.EffectiveFrom
            };
        }

        private DevolvedPostcode MapDevolvedPostcode(ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcode devolvedPostcode)
        {
            return new DevolvedPostcode()
            {
                Postcode = devolvedPostcode.Postcode,
                Area = devolvedPostcode.Area,
                SourceOfFunding = devolvedPostcode.SourceOfFunding,
                EffectiveTo = devolvedPostcode.EffectiveTo,
                EffectiveFrom = devolvedPostcode.EffectiveFrom
            };
        }
    }
}
