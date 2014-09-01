/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.Threading;
using VPrinting;

namespace ReceivingServiceLib.FileWorkers
{
    public class ContentModifyWorker : FileWorkerBase
    {
        protected static ContentModifyWorker ms_instance;

        public static ContentModifyWorker Default
        {
            get
            {
                if (ms_instance == null)
                    ms_instance = new ContentModifyWorker();
                return ms_instance;
            }
        }

        protected override void WorkerThreadFunction()
        {
            while (true)
            {
                try
                {
                    var contentRoot = new DirectoryInfo(Global.Strings.CONTENTWORKFOLDER);
                    contentRoot.CreateIfNotExist();


                }
                catch (Exception ex2)
                {
                    FireError(ex2);
                }
                finally
                {
                    Thread.Sleep(TIMEOUT);
                }
            }
        }
    }
}
