namespace Liquidata.Common.Extensions
{
    public static class HashSetExtensions
    {
        public static HashSet<T> AddRange<T>(this HashSet<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }

            return list;
        }
    }
}
