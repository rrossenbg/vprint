
namespace VPrinting
{
    public static class ReflectionEx
    {
        public static U GetValueSafe<T, U>(this T t, string propName) where T : class
        {
            if (t == default(T))
                return default(U);

            var p = typeof(T).GetProperty(propName);
            return (U)p.GetValue(t, null);
        }
    }
}