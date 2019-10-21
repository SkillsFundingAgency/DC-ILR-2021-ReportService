using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;

namespace ESFA.DC.ILR.ReportService.Data.Interface.Mappers.ReferenceData
{
    public interface IDevolvedPostcodeMapper
    {
        DevolvedPostcodes MapData(ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcodes postcodes);
    }
}
