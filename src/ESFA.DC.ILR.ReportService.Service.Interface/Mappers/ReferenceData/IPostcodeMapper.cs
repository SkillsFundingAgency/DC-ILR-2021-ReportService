using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData
{
    public interface IPostcodeMapper
    {
        IReadOnlyCollection<Postcode> MapData(IEnumerable<ReferenceDataService.Model.Postcodes.Postcode> postcodes);
    }
}
