using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData
{
    public interface IMetaDataMapper
    {
        MetaData MapData(ReferenceDataService.Model.MetaData.MetaData metaData);
    }
}
