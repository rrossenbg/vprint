/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using VPrinting.Threading;

namespace ReceivingServiceLib.FileWorkers
{
    public abstract class FileWorkerBase : WorkerBase
    {
        protected readonly byte[] m_Buffer50MB = new byte[50 * 1024 * 1024];
    }
}
