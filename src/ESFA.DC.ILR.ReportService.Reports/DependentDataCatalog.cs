using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public static class DependentDataCatalog
    {
        public static readonly Type Ilr = typeof(IMessage);

        public static readonly Type ValidationErrors = typeof(List<ValidationError>);

        public static readonly Type ReferenceData = typeof(ReferenceDataRoot);
    }
}
