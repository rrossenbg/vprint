using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using HobexCommonLib;
using HeaderNP = HobexCommonLib.VoucherNP.AuthenticationHeader;

namespace HobexServer
{
    public partial class FintraxHobexServer : ServiceBase
    {
        private HobexTraceWriter m_Tracer = new HobexTraceWriter();

        public FintraxHobexServer()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            m_Tracer.FolderName = path;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (!Directory.Exists(Config.InDirName))
                throw new IOException("Directory not found");

            m_FileWatcher.Path = Config.InDirName;
            m_FileWatcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            m_FileWatcher.EnableRaisingEvents = false;
        }

        private void File_Created(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == Config.FileEx)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    var header = new HeaderNP();
                    header.Init();

                    FileAccessClass faccess = new FileAccessClass();
                    faccess.Count = Config.AllocationsCount;
                    faccess.Prepare(Config.ExDirName, Config.IgnoreTerminalIds.ToEnumerable());
                    faccess.Process(header, e.FullPath, Config.OutDirName, Config.ErrDirName);

                    Config.LastRunDate = DateTime.Now;
                    Config.AllocationsCount = faccess.Count;
                });
            }
        }
    }
}
