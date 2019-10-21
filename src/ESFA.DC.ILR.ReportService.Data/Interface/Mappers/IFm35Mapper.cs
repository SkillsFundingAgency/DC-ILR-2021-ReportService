using ESFA.DC.ILR.ReportService.Models.Fm35;

namespace ESFA.DC.ILR.ReportService.Data.Interface.Mappers
{
    public interface IFm35Mapper
    {
        FM35Global MapData(FundingService.FM35.FundingOutput.Model.Output.FM35Global fm35Global);
    }
}
