using ESFA.DC.ILR.ReportService.Models.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Data.Interface.Mappers
{
    public interface IReferenceDataMapper
    {
        ReferenceDataRoot MapData(ReferenceDataService.Model.ReferenceDataRoot root);
    }
}
