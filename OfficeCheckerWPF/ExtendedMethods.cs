using System.Collections.Generic;

namespace OfficeCheckerWPF
{
    public static class ExtendedMethods
    {
        public static string ListToString(this IReadOnlyCollection<string> list)
        {
            var result = string.Empty;
            if (list.Count <= 0)
                return result;
            foreach (var s in list)
            {
                result += $"[{s}] ";
            }
            return result;
        }

        public static bool ReverseBool(this bool variable)
        {
            return !variable;
        }
    }
}