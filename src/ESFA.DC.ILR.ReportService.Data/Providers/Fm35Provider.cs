﻿using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm35Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public Fm35Provider(IFileService fileService, IJsonSerializationService serializationService) 
            : base(fileService, serializationService)
        {
        }

        public Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
            => ProvideAsync<FM35Global>(reportServiceContext.FundingFM35OutputKey, reportServiceContext.Container, cancellationToken);
    }
}
