using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Service.Model
{
    public class ReportServiceDependentData
    {
        public IDictionary<Type, Object> Data { get; set; }

        public IMessage IlrMessage { get; set; }

        public ReferenceDataRoot IlrReferenceData { get; set; }

        public List<ValidationError> ValidationErrors { get; set; }

        public T Get<T>()
            where T : class
        {
            return Data[typeof(T)] as T;
        }
    }
}
