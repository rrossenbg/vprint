/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Threading;

namespace DEMATLib
{
    public abstract class Processor
    {
        public string ExportDirectory { get; private set; }

        public static event ThreadExceptionEventHandler Error;

        public Processor(string exportDirectory)
        {
            ExportDirectory = exportDirectory;
        }

        protected void FireError(Exception ex)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex));
        }
    }
}
