using QuipuSolution.Application.Dto.Common;

namespace QuipuSolution.UnitTests.Application
{
    public class PagedResultTests
    {
        [Theory]
        [InlineData(0, 10, 0)]
        [InlineData(1, 10, 1)]
        [InlineData(10, 10, 1)]
        [InlineData(11, 10, 2)]
        [InlineData(4, 2, 2)]
        public void TotalPages_IsTotalCountDividedByPageSize_RoundedUp(int totalCount, int pageSize, int expectedPages)
        {
            var result = new PagedResult<string> { TotalCount = totalCount, PageSize = pageSize };

            Assert.Equal(expectedPages, result.TotalPages);
        }
    }
}
