using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IRenderService<T>
    {
        Worksheet Render(T model, Worksheet worksheet);
    }
}
