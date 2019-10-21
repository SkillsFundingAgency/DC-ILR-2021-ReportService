using ESFA.DC.ILR.ReportService.Models.Fm36;

namespace ESFA.DC.ILR.ReportService.Data.Interface.Mappers
{
    public interface IFm36Mapper
    {
        FM36Global MapData(FundingService.FM36.FundingOutput.Model.Output.FM36Global fm36Global);
    }
}
