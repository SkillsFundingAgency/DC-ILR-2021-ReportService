using System;
using ESFA.DC.ILR1819.ReportService.Interface.Model;

namespace ESFA.DC.ILR1819.ReportService.Model
{
    public sealed class LarsConfiguration : ILarsConfiguration
    {
        public string LarsConnectionString { get; set; }
    }
}
