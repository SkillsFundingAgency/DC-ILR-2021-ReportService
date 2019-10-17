using ESFA.DC.ILR.ReportService.Models.Fm81;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers
{
    public interface IFm81Mapper
    {
        FM81Global MapData(FundingService.FM81.FundingOutput.Model.Output.FM81Global fm81Global);
    }
}
