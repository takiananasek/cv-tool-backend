using CVTool.Data.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Data.Entity.Infrastructure;

namespace CVTool.Tests.Helpers
{
    public static class DbSetMockHelper
    {
        public static Mock<DbSet<T>> GetDbSetMock<T>(List<T> mockData) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IDbAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<T>(mockData.AsQueryable().GetEnumerator()));

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<User>(mockData.AsQueryable().Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mockData.AsQueryable().Expression); ;
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mockData.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mockData.AsQueryable().GetEnumerator());

            return mockSet;
        }
    }
}
