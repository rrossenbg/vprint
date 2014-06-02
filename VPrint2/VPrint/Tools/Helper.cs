using System.Text;

namespace VPrinting
{
    public static class Helper
    {
        public static string IIF(bool condition, string @true, string @false)
        {
            return condition ? @true : @false;
        }

        /// <summary>
        /// Esc,X,10,70 => LPRINT CHR$(27); CHR$(88); CHR$(10); CHR$(70);
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ToChr(params object[] values)
        {
            return values.ToChr();
        }

        /// <summary>
        /// Esc,X,10,70 => LPRINT CHR$(27); CHR$(88); CHR$(10); CHR$(70);
        /// Use text to leave values as they are
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ToEsc(params object[] values)
        {
            return string.Concat(ASCII.ESC, values.ToChr());
        }

        public static string Repeate(int count, string @string)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < count; i++)
                b.Append(@string);
            return b.ToString();
        }
    }
}
