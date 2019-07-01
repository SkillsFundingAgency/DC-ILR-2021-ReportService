using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Service.Model.Interface
{
    public interface IReportServiceDependentData
    {
        IMessage IlrMessage { get; set; }
        ReferenceDataRoot IlrReferenceData { get; set; }
        List<ValidationError> ValidationErrors { get; set; }
        T Get<T>();
        void Set(Type type, object value);
    }
}