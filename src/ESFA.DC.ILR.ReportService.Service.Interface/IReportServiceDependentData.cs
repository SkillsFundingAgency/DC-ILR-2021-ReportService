using System;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportServiceDependentData
    {
        T Get<T>();

        bool Contains<T>();

        void Set(Type type, object value);
    }
}