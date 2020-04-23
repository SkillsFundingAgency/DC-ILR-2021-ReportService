using System;
using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.NonContractDevolved
{
    public class NonContractDevolvedAdultEducationOccupancyReport : AbstractCsvReport<NonContractDevolvedAdultEducationOccupancyReportModel, NonContractDevolvedAdultEducationOccupancyReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.ValidIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm35,
        };

        public NonContractDevolvedAdultEducationOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<NonContractDevolvedAdultEducationOccupancyReportModel>> nonContractDevolvedAdultEducationOccupancyReportModelBuilder,
            ICsvFileService csvService)
            : base(
                fileNameService,
                nonContractDevolvedAdultEducationOccupancyReportModelBuilder,
                csvService,
                ReportTaskNameConstants.NonContractDevolvedAdultEducationOccupancyReport,
                ReportNameConstants.NonContractDevolvedAdultEducationOccupancy)
        {
        }
    }
}
