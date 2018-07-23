using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IStringUtilitiesService
    {
        string JoinWithMaxLength(IEnumerable<string> list);

        System.DateTime GetIlrFileDate(string ilrFilename);
    }
}
