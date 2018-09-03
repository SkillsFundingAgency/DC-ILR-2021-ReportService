using System;

namespace ESFA.DC.ILR1819.ReportService.Model.Lars
{
    public sealed class LarsFrameworkAim
    {
        public LarsFrameworkAim()
        {
        }

        public string LearnAimRef { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime EffectiveTo { get; set; }

        public int? FrameworkComponentType { get; set; }
    }
}
