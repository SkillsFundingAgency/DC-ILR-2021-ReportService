using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.ReferenceData.Postcodes.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class IlrReferenceDataProvider : IExternalDataProvider
    {
        private readonly Func<IILR2021_DataStoreEntities> _ilrContext;
        private readonly Func<IPostcodesContext> _postcodesContext;

        public IlrReferenceDataProvider(Func<IILR2021_DataStoreEntities> ilrContext, Func<IPostcodesContext> postcodesContext)
        {
            _ilrContext = ilrContext;
            _postcodesContext = postcodesContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var ilrContext = _ilrContext())
            {
                var latestFileDetails = await ilrContext.FileDetails
                    .Where(fd => fd.UKPRN == reportServiceContext.Ukprn)
                    .OrderByDescending(d => d.SubmittedTime)
                    .FirstOrDefaultAsync(cancellationToken);

                return new ReferenceDataRoot()
                {
                    MetaDatas = new MetaData()
                    {
                        ReferenceDataVersions = new ReferenceDataVersion()
                        {
                            CampusIdentifierVersion = new CampusIdentifierVersion()
                            {
                                Version = latestFileDetails?.CampusIdentifierVersion
                            },
                            Employers = new EmployersVersion()
                            {
                                Version = latestFileDetails?.EmployersVersion
                            },
                            LarsVersion = new LarsVersion()
                            {
                                Version = latestFileDetails?.LarsVersion
                            },
                            OrganisationsVersion = new OrganisationsVersion()
                            {
                                Version = latestFileDetails?.OrgVersion
                            },
                            PostcodesVersion = new PostcodesVersion()
                            {
                                Version = latestFileDetails?.PostcodesVersion
                            },
                            EasFileDetails = new EasFileDetails()
                            {
                                UploadDateTime = latestFileDetails?.EasUploadDateTime
                            }
                        }
                    },
                    Organisations = new List<Organisation>()
                    {
                        new Organisation()
                        {
                            UKPRN = reportServiceContext.Ukprn,
                            Name = latestFileDetails?.OrgName
                        }
                    },
                    DevolvedPostocdes = new DevolvedPostcodes()
                    {
                        McaGlaSofLookups = BuildMcaglaSofLookups(cancellationToken).Result
                    }
                };
            }
        }

        private async Task<List<McaGlaSofLookup>> BuildMcaglaSofLookups(CancellationToken cancellationToken)
        {
            using (var postcodesContext = _postcodesContext())
            {
                var mcaGlaFullNames = await postcodesContext.McaglaFullNames?
                           .Where(e => e.EffectiveTo == null)
                           .ToDictionaryAsync(
                           k => k.McaglaShortCode,
                           v => v.FullName,
                           StringComparer.OrdinalIgnoreCase,
                           cancellationToken);

                var mcaGlaSofCodes = await postcodesContext.McaglaSofs?.ToListAsync(cancellationToken);

                return mcaGlaSofCodes.Select(m => new McaGlaSofLookup
                {
                    SofCode = m.SofCode,
                    McaGlaShortCode = m.McaglaShortCode,
                    McaGlaFullName = mcaGlaFullNames.TryGetValue(m.McaglaShortCode, out var fullname) ? fullname : string.Empty,
                    EffectiveFrom = m.EffectiveFrom,
                    EffectiveTo = m.EffectiveTo
                }).ToList();
            }
        }
    }
}
