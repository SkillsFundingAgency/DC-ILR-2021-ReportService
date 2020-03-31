using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Service
{
    public class ZipServiceTests
    {
        [Fact]
        public async void CreateZipAsync()
        {
            var cancellationToken = CancellationToken.None;
            var container = "Output";
            var zipName = "Reports.zip";

            var fileNames = new List<string>()
            {
                "file1.csv"
            };

            var reportServiceContextMock = new Mock<IReportServiceContext>();
            var fileNameServiceMock = new Mock<IFileNameService>();
            fileNameServiceMock.Setup(fn => fn.GetFilename(reportServiceContextMock.Object, It.IsAny<string>(), OutputTypes.Zip, false)).Returns(zipName);


            Stream writeStream = new MemoryStream();
            Stream readStream = new MemoryStream();

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(sm => sm.OpenWriteStreamAsync(It.IsAny<string>(), container, cancellationToken)).Returns(Task.FromResult(writeStream)).Verifiable();
            fileServiceMock.Setup(sm => sm.OpenReadStreamAsync(It.IsAny<string>(), container, cancellationToken)).Returns(Task.FromResult(readStream)).Verifiable();

            await NewService(fileNameServiceMock.Object, fileServiceMock.Object).CreateZipAsync(reportServiceContextMock.Object, fileNames, container, cancellationToken);

            fileServiceMock.VerifyAll();
            fileNameServiceMock.VerifyAll();
        }

        private CreateZipService NewService(IFileNameService fileNameService = null, IFileService fileService = null)
        {
            return new CreateZipService(fileNameService, fileService);
        }
    }
}
