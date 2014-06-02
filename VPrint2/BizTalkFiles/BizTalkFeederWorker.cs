using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace BizTalkFiles
{
    public class BizTalkFeederWorker : CycleWorkerBase
    {
        public string FvFinOutFolder { get; set; }
        public string BizTalkFolder { get; set; }

        /// <summary>
        /// Files processed on one go
        /// </summary>
        public int MaxProcessFilesCount { get; set; }

        /// <summary>
        /// Maximum files into BT in directory
        /// </summary>
        public int MaxBizTalkFilesCount { get; set; }

        /// <summary>
        /// Maximum number of dehydrated files
        /// </summary>
        public int TalkDehydratedFilesCount { get; set; }

        protected override void ThreadFunction()
        {
            if (!SqlServerHelper.IsBizTalkReady(TalkDehydratedFilesCount))
            {
                Trace.WriteLine("BizTalk Not Ready Gets to Sleep.", "FEE");
                return;
            }

            var bizTalkDir = new DirectoryInfo(BizTalkFolder);
            FileInfo[] bizTalkFiles = bizTalkDir.GetFiles(MaxBizTalkFilesCount, "*.xml").ToArray();

            if (bizTalkFiles.Length < MaxBizTalkFilesCount)
            {
                var fvlOutDir = new DirectoryInfo(FvFinOutFolder);
                var fvlFiles = new List<FileInfo>(fvlOutDir.GetFiles(MaxProcessFilesCount, "*.xml"));

                fvlFiles.Sort(new FileComparer());

                foreach (var file in fvlFiles)
                {
                    if (!Running)
                        break;

                    Trace.WriteLine("Processing file: ".concat(file), "FEE");

                    if (!file.IsFileLocked())
                    {
                        Trace.WriteLine("Moving file: ".concat(file.Name), "FEE");
                        string fileName = Path.Combine(BizTalkFolder, file.Name);
                        file.MoveTo(fileName);

                        FireStep();
                        Trace.WriteLine("=============================", "FEE");
                    }
                    Thread.Sleep(0);
                }
            }
            else
            {
                Trace.WriteLine("Gets to Sleep", "FEE");
                return;
            }
        }
    }
}
