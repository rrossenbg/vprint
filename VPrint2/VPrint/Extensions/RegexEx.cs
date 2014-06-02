using System.Diagnostics;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;

namespace VPrinting
{
    public static class RegexEx
    {
        [TargetedPatchingOptOut("na")]
        public static Match Match(this Regex re, StringBuilder builder)
        {
            Debug.Assert(re != null);
            Debug.Assert(builder != null);

            var str = builder.ToString();
            var result = re.Match(str);
            return result;
        }

        [TargetedPatchingOptOut("na")]
        public static void Replace(this Regex re, ref StringBuilder builder, MatchEvaluator funct)
        {
            Debug.Assert(re != null);
            Debug.Assert(builder != null);
            Debug.Assert(funct != null);

            var str = builder.ToString();
            var result = re.Replace(str, funct);
            if (!result.IsNullOrEmpty())
                builder = new StringBuilder(result);
        }
    }
}
