using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IValueProvider
    {
        void GetFormattedValue(List<object> values, object value);
    }
}
