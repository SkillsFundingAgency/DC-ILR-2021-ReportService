using ESFA.DC.ILR.ReportService.Models.Fm99;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers
{
    public interface IFm99Mapper
    {
        ALBGlobal MapData(FundingService.ALB.FundingOutput.Model.Output.ALBGlobal albGlobal);
    }
}
