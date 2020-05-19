using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public class ReportServiceDependentData : IReportServiceDependentData
    {
        private IDictionary<Type, object> Data { get; } = new ConcurrentDictionary<Type, object>();

        public T Get<T>()
        {
            return (T) Data[typeof(T)];
        }

        public bool Contains<T>()
        {
            return Data.ContainsKey(typeof(T));
        }

        public void Set(Type type, object value) 
        {
            Data[type] = value;
        }
    }
}
