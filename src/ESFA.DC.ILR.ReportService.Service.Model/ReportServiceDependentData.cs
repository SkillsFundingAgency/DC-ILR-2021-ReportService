using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Service.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Service.Model
{
    public class ReportServiceDependentData : IReportServiceDependentData
    {
        private IDictionary<Type, Object> Data { get; set; } = new ConcurrentDictionary<Type, object>();

        public T Get<T>()
        {
            return (T) Data[typeof(T)];
        }

        public void Set(Type type, object value) 
        {
            Data[type] = value;
        }
    }
}
