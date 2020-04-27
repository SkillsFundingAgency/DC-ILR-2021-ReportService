using System;
using ESFA.DC.ILR.ReportService.Reports.Service;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Service
{
    public class AcademicYearServiceTests
    {
        [Fact]
        public void YearStart()
        {
            NewService().YearStart.Should().Be(new DateTime(2020, 8, 1));
        }

        [Fact]
        public void YearEnd()
        {
            NewService().YearEnd.Should().Be(new DateTime(2021, 7, 31, 23, 59, 59));
        }

        public AcademicYearService NewService()
        {
            return new AcademicYearService();
        }
    }
}
