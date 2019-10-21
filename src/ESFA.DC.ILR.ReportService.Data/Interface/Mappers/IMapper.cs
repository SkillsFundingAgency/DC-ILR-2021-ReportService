namespace ESFA.DC.ILR.ReportService.Data.Interface.Mappers
{
    public interface IMapper<in TIn, out TOut>
    {
        TOut MapData(TIn source);
    }
}