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
}