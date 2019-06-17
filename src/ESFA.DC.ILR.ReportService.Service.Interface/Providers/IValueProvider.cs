using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;
using ESFA.DC.ILR.ReportService.Service.Model.Generation;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Providers
{
    public interface IValueProvider
    {
        void GetFormattedValue(List<object> values, object value, ClassMap mapper, ModelProperty modelProperty);
    }
}
