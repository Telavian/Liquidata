
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions
{
    public class ArrayExtensionsTests
    {
        private readonly int[] _testList = [1, 2, 3, 4];

        [Fact]
        public void GivenCall_WhenCountNegative_ThenEmptyList()
        {
            var result = _testList.TakeRandom(-1);
            Assert.Empty(result);
        }

        [Fact]
        public void GivenCall_WhenCountZero_ThenEmptyList()
        {
            var result = _testList.TakeRandom(0);
            Assert.Empty(result);
        }

        [Fact]
        public void GivenCall_WhenListSmallerThanCount_ThenList()
        {
            var result = _testList.TakeRandom(_testList.Length + 1);
            Assert.Equal(_testList, result);
        }

        [Fact]
        public void GivenCall_WhenCount_ThenRandomItems()
        {
            var result = _testList.TakeRandom(_testList.Length - 1);
            
            foreach (var item in result)
            {
                Assert.Contains(item, _testList);
            }

            Assert.Distinct(result);
        }
    }
}
