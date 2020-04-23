using System;
using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved
{
    public class DevolvedAdultEducationOccupancyReport : AbstractCsvReport<DevolvedAdultEducationOccupancyReportModel, DevolvedAdultEducationOccupancyReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.ValidIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm35,
        };

        public DevolvedAdultEducationOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>> devolvedAdultEducationOccupancyReportModelBuilder,
            ICsvFileService csvService)
            : base(
                fileNameService,
                devolvedAdultEducationOccupancyReportModelBuilder,
                csvService,
                ReportTaskNameConstants.DevolvedAdultEducationOccupancyReport,
                ReportNameConstants.DevolvedAdultEducationOccupancy)
        {
        }
    }
}
