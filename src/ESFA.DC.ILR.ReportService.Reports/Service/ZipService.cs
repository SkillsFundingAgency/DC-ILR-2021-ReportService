using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Service
{
    public class ZipService : IZipService
    {
        private readonly IFileNameService _fileNameService;
        private readonly IFileService _fileService;

        public ZipService(IFileNameService fileNameService, IFileService fileService)
        {
            _fileNameService = fileNameService;
            _fileService = fileService;
        }

        public async Task CreateZip(IReportServiceContext reportServiceContext, IEnumerable<string> fileNames, string container, CancellationToken cancellationToken)
        {
            var zipName = _fileNameService.GetFilename(reportServiceContext, "Reports", OutputTypes.Zip, false);

            using (var writeStream = await _fileService.OpenWriteStreamAsync(zipName, container, cancellationToken))
            {
                using (var zipArchive = new ZipArchive(writeStream, ZipArchiveMode.Create, true))
                {
                    foreach (var fileName in fileNames)
                    {
                        var archiveEntry = zipArchive.CreateEntry(fileName);

                        using (var archiveEntryStream = archiveEntry.Open())
                        {
                            using (var readStream =
                                await _fileService.OpenReadStreamAsync(fileName, container, cancellationToken))
                            {
                                await readStream.CopyToAsync(archiveEntryStream, 8096, cancellationToken);
                            }
                        }
                    }
                }
            }
        }
    }
}
