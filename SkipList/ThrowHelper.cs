namespace SkipList;

internal static class ThrowHelper
{
    internal static void ThrowKeyNotFoundException()
    {
        throw new System.Collections.Generic.KeyNotFoundException();
    }

    internal static void ThrowInvalidOperationException()
    {
        throw new System.InvalidOperationException();
    }

    internal static void ThrowInsertWithExitedKeyException<TKey>(
        TKey key)
    {
        throw new System.InvalidOperationException($"key:{key} has existed");
    }

    internal static void ThrowRandomLevelIsZeroException()
    {
        throw new InvalidOperationException("level is 0");
    }
}
