using System.Threading;

namespace VPrinting.Tools
{
    internal class DelegateHelper
    {
        public static EventWaitHandle GetEvent()
        {
            return new EventWaitHandle(false, EventResetMode.ManualReset);
        }
    }
}
