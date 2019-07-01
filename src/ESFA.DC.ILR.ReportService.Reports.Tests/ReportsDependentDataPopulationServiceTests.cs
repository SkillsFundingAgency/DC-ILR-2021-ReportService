using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests
{
    
    public class ReportsDependentDataPopulationServiceTests
    {
        [Fact]
        public async Task ShouldPopulateDependentData()
        {
            var cancellationToken = CancellationToken.None;
            var reportServiceMock = new Mock<IReportServiceContext>();
            var indexMock = new Mock<IIndex<Type, IExternalDataProvider>>();
            var message = new Message();
            var referenceDataRoot = new ReferenceDataRoot();
            var validationErrors = new List<ValidationErrors.Interface.Models.ValidationError>();
            indexMock.Setup(x => x[typeof(IMessage)].ProvideAsync(reportServiceMock.Object, cancellationToken)).ReturnsAsync(message);
            indexMock.Setup(x => x[typeof(ReferenceDataRoot)].ProvideAsync(reportServiceMock.Object, cancellationToken)) .ReturnsAsync(referenceDataRoot);
            indexMock.Setup(x => x[typeof(List<ValidationErrors.Interface.Models.ValidationError>)].ProvideAsync(reportServiceMock.Object, cancellationToken)).ReturnsAsync(validationErrors);

            var service = new ReportsDependentDataPopulationService(indexMock.Object);

            var dependsOnTypes = new List<Type>
            {
                typeof(IMessage),
                typeof(ReferenceDataRoot),
                typeof(List<ValidationErrors.Interface.Models.ValidationError>)
            };

            var result = await service.PopulateAsync(reportServiceMock.Object, dependsOnTypes, cancellationToken);

            
            result.Get<IMessage>().Should().BeSameAs(message);
            result.Get<ReferenceDataRoot>().Should().BeSameAs(referenceDataRoot);
            result.Get<List<ValidationErrors.Interface.Models.ValidationError>>().Should().BeSameAs(validationErrors);
        }
    }
}
