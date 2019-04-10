﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;

namespace ESFA.DC.ILR.ReportService.Service.Service
{
    public class ZipService : IZipService
    {
        private readonly IFileService _fileService;

        public ZipService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task AddReportToArchiveAsync(ZipArchive zipArchive, IReport report, IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var zipFileName = report.GetZipFilename(reportServiceContext);
            var fileReference = report.GetFilename(reportServiceContext);

            ZipArchiveEntry archiveEntry = zipArchive.GetEntry(zipFileName);
            archiveEntry?.Delete();

            archiveEntry = zipArchive.CreateEntry(zipFileName, CompressionLevel.Optimal);

            using (var fileStream = await _fileService.OpenReadStreamAsync(fileReference, reportServiceContext.Container, cancellationToken))
            using (var stream = archiveEntry.Open())
            {
                await fileStream.CopyToAsync(stream, 81920, cancellationToken);
            }
        }
    }
}
