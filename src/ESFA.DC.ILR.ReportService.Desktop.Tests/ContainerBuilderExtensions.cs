using Autofac;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Moq;

namespace ESFA.DC.ILR.ReportService.Desktop.Tests
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterCommonServiceStubs(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterMock<IDateTimeProvider>();
            containerBuilder.RegisterMock<ILogger>();
            containerBuilder.RegisterMock<IFileService>();
            containerBuilder.RegisterMock<IXmlSerializationService>();
            containerBuilder.RegisterMock<IJsonSerializationService>();
            containerBuilder.RegisterMock<ISerializationService>();
            containerBuilder.RegisterMock<IExcelFileService>();
            containerBuilder.RegisterMock<ICsvFileService>();
            containerBuilder.RegisterMock<IReportServiceContextKeysMutator>();
        }

        private static void RegisterMock<T>(this ContainerBuilder containerBuilder)
            where T : class
        {
            var mock = Mock.Of<T>();

            containerBuilder.RegisterInstance(mock).As<T>();
        }
    }
}
