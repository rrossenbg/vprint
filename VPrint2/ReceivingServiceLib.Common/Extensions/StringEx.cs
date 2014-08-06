using System.Runtime;

namespace ReceivingServiceLib
{
    public static class StringEx
    {
        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        [TargetedPatchingOptOut("na")]
        public static string EmptyOrWhiteSpaceToNull(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
