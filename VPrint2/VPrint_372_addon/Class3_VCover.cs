/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VPrint;
using VPrinting;
using VPrinting.Common;
using VPrinting.Communication;
using VPrinting.Interfaces;

namespace VPrint_addon
{
    public class Class3_VCover : IRunnable
    {
        private static Hashtable m_WaitObjects = Hashtable.Synchronized(new Hashtable());
        private static CallVCoverService_ReadDataDelegate ms_ReadDataDelegate = new CallVCoverService_ReadDataDelegate(CallVCoverService_ReadData);

        public void Run()
        {
            StateSaver.Default.Set(Strings.USE_VCOVER, true);

            var prtReadData = ms_ReadDataDelegate.GetFunctionPointer();
            StateSaver.Default.Set(Strings.VCOVER_FUNC, prtReadData);

            NamedPipes.ReceivedData += new ReceivedDataDelegate(NamedPipes_ReceivedData);
            NamedPipes.Error += new ThreadExceptionEventHandler(NamedPipes_Error);
            NamedPipes.StartServer("VPRINT");
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
                m_WaitObjects[obj.Key] = obj;
                List<FileInfo> files = (List<FileInfo>)obj.Value;

                try
                {
                    StringBuilder b = new StringBuilder();
                    b.Append(obj.Key);
                    b.Append(";");

                    foreach (FileInfo info in files)
                    {
                        b.Append(info.FullName);
                        b.Append(";");
                    }

                    NamedPipes.SendMessage("VCOVER", b.ToString());
                }
                catch (Exception ex)
                {
                    obj.Err = ex;
                }

            }, @object);
        }

        private string NamedPipes_ReceivedData(string data)
        {
            WaitObject obj = null;
            Guid id = Guid.Empty;
            try
            {
                var strs = data.FromStr();
                id = Guid.Parse(strs[0]);

                obj = (WaitObject)m_WaitObjects[id];
                if (obj == null)
                    throw new ApplicationException("Cannot find key " + id);

                List<FileInfo> files = (List<FileInfo>)obj.Value;
                files.AddRange(strs.Skip(1).Where(s => !string.IsNullOrWhiteSpace(s)).ToList().ConvertAll<FileInfo>(s => new FileInfo(s)));
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (obj != null)
                    obj.Err = ex;
                return string.Empty;
            }
            finally
            {
                if (obj != null)
                    obj.Signal();
                m_WaitObjects.Remove(id);
            }
        }

        private void NamedPipes_Error(object sender, ThreadExceptionEventArgs e)
        {
            Global.FireError(e.Exception);
        }
    }
}
