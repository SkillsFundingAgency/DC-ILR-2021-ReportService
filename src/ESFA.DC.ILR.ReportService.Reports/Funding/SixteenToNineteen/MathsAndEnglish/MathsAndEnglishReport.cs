using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.MathsAndEnglish
{
    public class MathsAndEnglishReport : AbstractSixteenToNineteenReport<MathsAndEnglishReportModel, MathsAndEnglishReportClassMap>
    {
        public MathsAndEnglishReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<MathsAndEnglishReportModel>> modelBuilder,
            ICsvFileService csvService) 
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
