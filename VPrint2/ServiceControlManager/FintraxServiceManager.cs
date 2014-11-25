/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Xml.XPath;
using System.Reflection;

namespace FintraxServiceManager
{
    public class FintraxServiceManager : ServiceBase
    {
        private string serviceFile = "";

        private readonly CircularWorker m_ThreadWorker = new CircularWorker();

        private XPathDocument docNav;

        const string strExpression = "count(/Services/Service)";

        private string logPath;

        public FintraxServiceManager()
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();
            this.AutoLog = true;
        }

        static void Main()
        {
            var ServicesToRun = new ServiceBase[] { new FintraxServiceManager() };
            ServiceBase.Run(ServicesToRun);
        }

        private void InitializeComponent()
        {
            this.ServiceName = "FintraxServiceManager";
            m_ThreadWorker.Info += new EventHandler<EntryEventArgs<string>>(ThreadWorker_Info);
            m_ThreadWorker.Error += new System.Threading.ThreadExceptionEventHandler(ThreadWorker_Error);
        }

        protected override void OnStart(string[] args)
        {
            this.logPath = Convert.ToString(ConfigurationSettings.AppSettings["LogPath"]);
            this.serviceFile = Convert.ToString(ConfigurationSettings.AppSettings["ServiceFilePath"]);
            this.docNav = new XPathDocument(serviceFile);
            this.EventLog.WriteEntrySf("Service file path:" + serviceFile);

            m_ThreadWorker.ToDoList.Clear();
            m_ThreadWorker.DoneList.Clear();

            var from = Path.Combine(Path.GetDirectoryName(this.serviceFile), "bin");
            var to = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            CopyLibraries(from, to);

            ReadXml();

            m_ThreadWorker.Start();
        }

        protected override void OnStop()
        {
            m_ThreadWorker.Stop();
        }

        private void CopyLibraries(string fromPath, string toPath)
        {
            var dlls = Directory.GetFiles(fromPath, "*.dll");
            foreach (var dll in dlls)
            {
                var newname = Path.Combine(toPath, Path.GetFileName(dll));
                File.Copy(dll, newname, true);
            }
        }

        private void ReadXml()
        {
            int jobsCount = 0;
            var nav = docNav.CreateNavigator();
            string count = Convert.ToString(nav.Evaluate(strExpression));

            for (int i = 1; i <= int.Parse(count); i++)
            {
                var name = "";
                var time = "";
                var type = "";
                var method = "";
                var parameters = "";
                var str = "//Services//Service[position()=" + i + "]";

                //~1. Get the Name
                #region Get Name

                string nameExpr = str + "//name";
                var nodeIterator1 = nav.Select(nameExpr);

                while (nodeIterator1.MoveNext())
                    name = nodeIterator1.Current.Value;

                #endregion

                //2. Get the Time(s).
                #region Get Time

                string timeExpr = str + "//time";
                var nodeIterator2 = nav.Select(timeExpr);

                while (nodeIterator2.MoveNext())
                    time = nodeIterator2.Current.Value;

                string[] times = time.Split(',');//There can be more than one time specified.

                #endregion

                //3. Get the Type to run.
                #region Get Type to instantiate

                string typeExpr = str + "//type";
                var nodeIterator3 = nav.Select(typeExpr);

                while (nodeIterator3.MoveNext())
                    type = nodeIterator3.Current.Value;

                #endregion

                //4. Get the Method to run.
                #region Get the method to run

                string methodExpr = str + "//method";
                var nodeIterator4 = nav.Select(methodExpr);

                while (nodeIterator4.MoveNext())
                    method = nodeIterator4.Current.Value;

                #endregion

                //5. Get the parameters to run.

                #region Parameters to pass to the method

                string parameterExpr = str + "//parameters";
                var nodeIterator5 = nav.Select(parameterExpr);

                while (nodeIterator5.MoveNext())
                    parameters = nodeIterator5.Current.Value;

                #endregion

                foreach (string timeToRun in times)
                {
                    var t = TimeSpan.Parse(timeToRun).Add(TimeSpan.FromSeconds(jobsCount++));
                    var p = new TypeParam(name, type, method, parameters, t);
                    m_ThreadWorker.ToDoList.Add(t, p);
                }
            }
        }

        private void ThreadWorker_Info(object sender, EntryEventArgs<string> e)
        {
            EventLog.WriteEntrySf(e.Value, EventLogEntryType.Information);
        }

        private void ThreadWorker_Error(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            EventLog.WriteEntrySf("Exception in service: " + ServiceName, EventLogEntryType.Error);
            EventLog.WriteEntrySf(e.Exception.ToString(), EventLogEntryType.Error);

            try
            {
                using (var logFile = new StreamWriter(string.Format("{0}{1:dd_MM_yyyy}_FintraxServiceManager.log", logPath, DateTime.Now), false))
                {
                    logFile.WriteLine(DateTime.Now + ": Service = " + ServiceName);
                    logFile.WriteLine(DateTime.Now + ": " + e.Exception.Message);
                    logFile.WriteLine(DateTime.Now + ": " + e.Exception.StackTrace);
                    logFile.Close();
                }
            }
            catch
            {
                // do nothing. Ignore
            }
        }
    }
}