using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Shared;

namespace Liquidata.Common.Extensions
{
    public static class SelectionExtensions
    {
        public static IReadOnlyCollection<SelectionActionBase> GetAllSelectionAncestors(this ActionBase? node)
        {
            var results = new List<SelectionActionBase>();

            while (node != null)
            {
                if (node.IsSelectionAction())
                {
                    results.Add((SelectionActionBase)node);
                }

                node = node.Parent;
            }

            return results;
        }
    }
}
