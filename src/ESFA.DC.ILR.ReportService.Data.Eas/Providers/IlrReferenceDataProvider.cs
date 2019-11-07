using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class IlrReferenceDataProvider : IExternalDataProvider
    {
        private readonly Func<IILR1920_DataStoreEntities> _ilrContext;

        public IlrReferenceDataProvider(Func<IILR1920_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var context = _ilrContext())
            {
                return await context.FileDetails
                    .Where(fd => fd.UKPRN == reportServiceContext.Ukprn)
                    .OrderByDescending(d => d.SubmittedTime)
                    .Select(x => new ReferenceDataRoot()
                    {
                        MetaDatas = new MetaData()
                        {
                            ReferenceDataVersions = new ReferenceDataVersion()
                            {
                                CampusIdentifierVersion = new CampusIdentifierVersion()
                                {
                                    Version = x.CampusIdentifierVersion
                                },
                                Employers = new EmployersVersion()
                                {
                                    Version = x.EmployersVersion
                                },
                                LarsVersion = new LarsVersion()
                                {
                                    Version = x.LarsVersion
                                },
                                OrganisationsVersion = new OrganisationsVersion()
                                {
                                    Version = x.OrgVersion
                                },
                                PostcodesVersion = new PostcodesVersion()
                                {
                                    Version = x.PostcodesVersion
                                },
                                EasUploadDateTime = new EasUploadDateTime()
                                {
                                    UploadDateTime = x.EasUploadDateTime
                                }
                            }
                        },
                        Organisations = new List<Organisation>()
                        {
                            new Organisation()
                            {
                                UKPRN = x.UKPRN,
                                Name = x.OrgName
                            }
                        }
                    }).FirstOrDefaultAsync(cancellationToken) ?? new ReferenceDataRoot();
            }
        }
    }
}
