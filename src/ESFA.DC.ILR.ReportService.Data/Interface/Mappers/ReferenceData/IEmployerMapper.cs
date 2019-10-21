using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Employers;

namespace ESFA.DC.ILR.ReportService.Data.Interface.Mappers.ReferenceData
{
    public interface IEmployerMapper
    {
        IReadOnlyCollection<Employer> MapData(IEnumerable<ReferenceDataService.Model.Employers.Employer> employers);
    }
}
