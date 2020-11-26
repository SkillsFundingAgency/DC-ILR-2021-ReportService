using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Stateless.Configuration
{
    public sealed class VersionInfo : IVersionInfo
    {
        public string ServiceReleaseVersion { get; set; }
    }
}
