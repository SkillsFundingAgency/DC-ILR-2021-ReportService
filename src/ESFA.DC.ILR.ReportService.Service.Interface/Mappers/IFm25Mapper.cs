using ESFA.DC.ILR.ReportService.Models.Fm25;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers
{
    public interface IFm25Mapper
    {
        FM25Global MapData(FundingService.FM25.Model.Output.FM25Global fm25Global);
    }
}
