/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;

namespace SiteCodeSrvc
{
    public class SaveThread : CycleWorkerBase
    {
        public static event EventHandler Save;

        protected override void ThreadFunction()
        {
            if (Save != null)
                Save(this, EventArgs.Empty);
        }
    }
}
