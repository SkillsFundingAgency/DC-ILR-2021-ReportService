﻿using ESFA.DC.ILR.ReportService.Interface.Service;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class ValidationStageOutputCache : IValidationStageOutputCache
    {
        public int DataMatchProblemCount { get; set; }

        public int DataMatchProblemLearnersCount { get; set; }
    }
}
