/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;

namespace CPrint2.Common
{
    public class SendWorker : CycleWorkerBase
    {
        public static string ImageFilter { get; set; }
        public static string WorkDirectory { get; set; }

        public override void RunOnce()
        {
            while (true)
            {
                try
                {
                    var files = Directory.GetFiles(WorkDirectory, ImageFilter);

                    foreach (string file in files)
                    {
                    }
                }
                catch (Exception ex)
                {
                    FireError(ex);
                }
            }
        }

        protected override void FireStarted()
        {
        }
    }
}
