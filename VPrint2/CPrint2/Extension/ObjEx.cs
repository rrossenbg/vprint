using System;
using System.Runtime;

namespace CPrint2
{
    public static class ObjEx
    {
        [TargetedPatchingOptOut("na")]
        public static bool IsZero(this IntPtr ptr)
        {
            return ptr == IntPtr.Zero;
        }
    }
}
