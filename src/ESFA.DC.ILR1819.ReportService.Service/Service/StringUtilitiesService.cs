using System.Collections.Generic;
using ESFA.DC.ILR1819.ReportService.Interface.Service;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class StringUtilitiesService : IStringUtilitiesService
    {
        public string JoinWithMaxLength(IEnumerable<string> list)
        {
            string ret = string.Join(", ", list);

            if (ret.Length > 200)
            {
                ret = ret.Substring(0, 200) + ";(message was truncated)";
            }

            return ret;
        }
    }
}
