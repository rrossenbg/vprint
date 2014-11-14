using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VPrint;
using VPrinting;
using VPrinting.Common;
using VPrinting.Interfaces;
using VPrinting.Communication;

namespace VPrint_addon
{
    public class Class3_VCover : IRunnable
    {
        private static CallVCoverService_ReadDataDelegate ms_ReadDataDelegate = new CallVCoverService_ReadDataDelegate(CallVCoverService_ReadData);

        public void Run()
        {
            StateSaver.Default.Set(Strings.USE_VCOVER, true);

            var prtReadData = ms_ReadDataDelegate.GetFunctionPointer();
            StateSaver.Default.Set(Strings.VCOVER_FUNC, prtReadData);
        }

        public void Exec(object data)
        {
        }

        public void Exit()
        {
        }

        private static void CallVCoverService_ReadData(WaitObject @object)
        {
            Task.Factory.StartNew((o) =>
            {
                WaitObject obj = (WaitObject)o;
                List<FileInfo> files = (List<FileInfo>)obj.Value;
                List<FileInfo> toadd = new List<FileInfo>();

                try
                {
                    foreach (FileInfo info in files)
                    {
                        var id = Guid.NewGuid();

                        NamedPipes.SendMessage("PIPE1", string.Concat("Start;", id, ";", info.FullName));

                        Thread.Sleep(TimeSpan.FromSeconds(10));

                        string result = null;
                        for (int count = 0; (result = NamedPipes.SendMessage("PIPE1", id.toString())) == null && count < 100; count++)
                            Thread.Sleep(TimeSpan.FromSeconds(10));

                        if (result != null)
                            toadd.AddRange(result.Split(';').ToList().ConvertAll<FileInfo>(s => new FileInfo(s)));
                    }
                }
                catch (Exception ex)
                {
                    obj.Err = ex;
                }
                finally
                {
                    files.AddRange(toadd);

                    obj.Signal();
                }
            }, @object);
        }
    }
}
