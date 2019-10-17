using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Employers;
using ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class EmployerMapper : IEmployerMapper
    {
        public IReadOnlyCollection<Employer> MapData(IEnumerable<ReferenceDataService.Model.Employers.Employer> employers)
        {
            return employers?.Select(MapEmployer).ToList();
        }

        private Employer MapEmployer(ReferenceDataService.Model.Employers.Employer employer)
        {
            return new Employer()
            {
                ERN = employer.ERN,
                LargeEmployerEffectiveDates = employer.LargeEmployerEffectiveDates?.Select(MapLargeEmployerEffectiveDates).ToList()
            };
        }

        private LargeEmployerEffectiveDates MapLargeEmployerEffectiveDates(ReferenceDataService.Model.Employers.LargeEmployerEffectiveDates largeEmployerEffectiveDates)
        {
            return new LargeEmployerEffectiveDates()
            {
                EffectiveFrom = largeEmployerEffectiveDates.EffectiveFrom,
                EffectiveTo = largeEmployerEffectiveDates.EffectiveTo
            };
        }
    }
}
