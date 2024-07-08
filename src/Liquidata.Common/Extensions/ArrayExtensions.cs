namespace Liquidata.Common.Extensions;

public static class ArrayExtensions
{
    public static T[] TakeRandom<T>(this T[] items, int count)
    {
        if (items is null || items.Length == 0 || count <= 0)
        {
            return [];
        }

        if (items.Length <= count)
        {
            return items;
        }

        var results = new HashSet<T>();

        while (true)
        {
            var pickCount = count - results.Count;
            var picked = Random.Shared.GetItems(items, pickCount);

            foreach (var pickedItem in picked)
            {
                results.Add(pickedItem);
            }

            if (results.Count == count)
            {
                return results.ToArray();
            }
        }
    }
}
