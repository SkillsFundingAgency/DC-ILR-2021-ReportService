using ESFA.DC.ILR.ReportService.Interface.Configuration;

namespace ESFA.DC.ILR.ReportService.Stateless.Configuration
{
    public sealed class VersionInfo : IVersionInfo
    {
        public string ServiceReleaseVersion { get; set; }
    }
}
