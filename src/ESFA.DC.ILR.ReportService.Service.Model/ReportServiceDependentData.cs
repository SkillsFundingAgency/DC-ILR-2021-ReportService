using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Service.Model.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Service.Model
{
    public class ReportServiceDependentData : IReportServiceDependentData
    {
        private IDictionary<Type, Object> Data { get; set; } = new Dictionary<Type, object>();

        public IMessage IlrMessage { get; set; }

        public ReferenceDataRoot IlrReferenceData { get; set; }

        public List<ValidationError> ValidationErrors { get; set; }

        public T Get<T>()
        {
            return (T) Data[typeof(T)];
        }

        public void Set(Type type, Object value) 
        {
            Data[type] = value;
        }
    }
}
