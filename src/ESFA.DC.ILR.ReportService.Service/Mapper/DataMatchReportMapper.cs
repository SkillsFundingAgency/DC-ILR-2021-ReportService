﻿using CsvHelper.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Mapper
{
    public sealed class DataMatchReportMapper : ClassMap<DataMatchModel>, IClassMapper
    {
        public DataMatchReportMapper()
        {
            Map(m => m.LearnRefNumber).Index(0).Name("Learner reference number");
            Map(m => m.Uln).Index(1).Name("Unique learner number");
            Map(m => m.AimSeqNumber).Index(2).Name("Aim sequence number");
            Map(m => m.RuleName).Index(3).Name("Rule name");
            Map(m => m.Description).Index(4).Name("Description");
            Map(m => m.ILRValue).Index(5).Name("ILR value");
            Map(m => m.ApprenticeshipServiceValue).Index(6).Name("Apprenticeship service value");
            Map(m => m.PriceEpisodeStartDate).Index(7).Name("Price episode start date");
            Map(m => m.PriceEpisodeActualEndDate).Index(8).Name("Price episode actual end date");
            Map(m => m.OfficalSensitive).Index(9).Name("OFFICIAL - SENSITIVE");
        }
    }
}
