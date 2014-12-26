using System;

namespace VPrinting.Colections
{
    public class DomainTable
    {
        public T GetValue<T>(string name, T @default)
        {
            object data = AppDomain.CurrentDomain.GetData(name);
            return data != null ? (T)data : @default;
        }

        public void SetValue<T>(string name, T value)
        {
            AppDomain.CurrentDomain.SetData(name, value);
        }
    }
}
