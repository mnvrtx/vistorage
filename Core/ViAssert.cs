using System;
using System.Runtime.CompilerServices;

namespace Core
{
    public static class ViAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void True(bool b, string message)
        {
            if (!b)
                throw new Exception($"Should be true: {message}");
        }
    }
}