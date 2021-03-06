﻿using System;
using System.Collections.Generic;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main
{
    public class MainOccupancyReport : AbstractCsvReport<MainOccupancyReportModel, MainOccupancyReportClassMap>, IReport
    {
        public IEnumerable<Type> DependsOn => new[]
        {
            DependentDataCatalog.ValidIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.Fm35,
            DependentDataCatalog.Fm25,
        };

        public MainOccupancyReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<MainOccupancyReportModel>> modelBuilder,
            ICsvFileService csvService)
            : base(fileNameService, modelBuilder, csvService, ReportTaskNameConstants.MainOccupancyReport, ReportNameConstants.MainOccupancy)
        {
        }
    }
}
