using System;
using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public static class DependentDataCatalog
    {
        public static readonly Type ValidIlr = typeof(IMessage);

        public static readonly Type InvalidIlr = typeof(ILooseMessage);

        public static readonly Type ValidationErrors = typeof(List<ValidationError>);

        public static readonly Type ReferenceData = typeof(ReferenceDataRoot);

        public static readonly Type Fm25 = typeof(FM25Global);

        public static readonly Type Fm35 = typeof(FM35Global);

        public static readonly Type Fm36 = typeof(FM36Global);

        public static readonly Type Fm81 = typeof(FM81Global);

        public static readonly Type Fm99 = typeof(ALBGlobal);
    }
}
