using System;
using System.Linq.Expressions;
using Moq;

namespace ESFA.DC.ILR.ReportService.Reports.Tests
{
    public static class MockExtensions
    {
        public static Mock<T> NewMock<T>()
            where T : class
        {
            return new Mock<T>();
        }

        public static Mock<T> With<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> selector, TProperty value)
            where T : class
        {
            mock.SetupGet(selector).Returns(value);

            return mock;
        }

        public static T Build<T>(this Mock<T> mock)
            where T : class
        {
            return mock.Object;
        }
    }
}
