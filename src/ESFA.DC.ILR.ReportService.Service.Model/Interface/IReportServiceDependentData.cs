using System;

namespace ESFA.DC.ILR.ReportService.Service.Model.Interface
{
    public interface IReportServiceDependentData
    {
        T Get<T>();
        void Set(Type type, object value);
    }
}