using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions;

public class HashSetExtensionsTests
{
    [Fact]
    public void GivenHashSet_WhenAddRange_ThenItemsAdded()
    {
        var testList = new[] { 1, 2, 3 };

        var hashSet = new HashSet<int>();
        hashSet.AddRange(testList);

        foreach (var item in testList)
        {
            Assert.Contains(item, hashSet);
        }
    }
}
