using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.MathsAndEnglish
{
    public class MathsAndEnglishReport : AbstractSixteenToNineteenReport<MathsAndEnglishReportModel, MathsAndEnglishReportClassMap>
    {
        public MathsAndEnglishReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<MathsAndEnglishReportModel>> modelBuilder,
            ICsvService csvService) 
            : base(
                fileNameService,
                modelBuilder,
                csvService,
                ReportTaskNameConstants.MathsAndEnglishReport,
                "Maths and English Report")
        {
        }
    }
}
