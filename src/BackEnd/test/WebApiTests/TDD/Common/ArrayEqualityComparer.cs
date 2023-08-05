using System.Diagnostics.CodeAnalysis;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.Common;

public class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
{
    public bool Equals(T[]? x, T[]? y)
    {
        if (x is null || y is null || x.Length != y.Length)
        {
            return false;
        }

        for (var i = 0; i < x.Length; i++)
        {
            var isEqual = x[i] is T a
                && y[i] is T b
                && a.Equals(b)
                ;

            if (isEqual == false)
                return false;
        }

        return true;
    }

    public int GetHashCode([DisallowNull] T[] obj)
    {
        return HashCode.Combine(obj);
    }
}
