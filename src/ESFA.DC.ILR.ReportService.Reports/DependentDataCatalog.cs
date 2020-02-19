using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReportService.Models.EAS;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.Fm36;
using ESFA.DC.ILR.ReportService.Models.Fm81;
using ESFA.DC.ILR.ReportService.Models.Fm99;
using ESFA.DC.ILR.ReportService.Models.FRM;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public static class DependentDataCatalog
    {
        public static readonly Type ValidIlr = typeof(IMessage);

        public static readonly Type InputIlr = typeof(ILooseMessage);

        public static readonly Type ValidationErrors = typeof(List<ValidationError>);

        public static readonly Type ReferenceData = typeof(ReferenceDataRoot);

        public static readonly Type Fm25 = typeof(FM25Global);

        public static readonly Type Fm35 = typeof(FM35Global);

        public static readonly Type Fm36 = typeof(FM36Global);

        public static readonly Type Fm81 = typeof(FM81Global);

        public static readonly Type Fm99 = typeof(ALBGlobal);

        public static readonly Type Eas = typeof(List<EasFundingLine>);

        public static readonly Type Frm = typeof(FrmReferenceData);
    }
}
