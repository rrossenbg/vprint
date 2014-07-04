using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCopy
{
    public partial class MainForm : Form
    {
        public class KeyValue<T,V>
        {
            public T Key { get; set; }
            public T Value1 { get; set; }
            public V Value2 { get; set; }

            public KeyValue(T key, T value1, V value2 = default(V))
            {
                Key = key;
                Value1 = value1;
                Value2 = value2;
            }

            public override string ToString()
            {
                return string.Concat(Key, " <-> ", Value1);
            }
        }

        private readonly List<Task> m_Tasks = new List<Task>();
        private SynchronizationContext m_CurrentCnt;
        private CancellationTokenSource m_Cancel = new CancellationTokenSource();

        public MainForm()
        {
            InitializeComponent();
            m_CurrentCnt = SynchronizationContext.Current;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            m_Cancel.Cancel();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (m_Tasks.Exists(t => t.Status == TaskStatus.Running) && this.ShowMsg("There is not completed task. Close?", MessageBoxIcon.Question))
                e.Cancel = true;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            m_Cancel.Cancel();
            m_Cancel.DisposeSafe();
            base.OnClosed(e);
        }

        private void DirectoryCopy_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtFromPath.Text))
            {
                this.ShowMsg("From path not found");
                return;
            }
            else if(!Directory.Exists(txtToPath.Text))
            {
                this.ShowMsg("To path not found");
                return;
            }

            m_Cancel.DisposeSafe();
            m_Tasks.Clear();

            m_Cancel = new CancellationTokenSource();

            var fromFileNames = Directory.GetFiles(txtFromPath.Text);
            var toFileNames = new List<string>(Directory.GetFiles(txtToPath.Text));

            m_CurrentCnt.Post(UIShowMessage, "Copying ".concat(fromFileNames.Length, " files"));

            foreach (string fromFileName in fromFileNames)
            {
                var name = Path.GetFileName(fromFileName);
                var newFileName = Path.Combine(txtToPath.Text, name);

                if (toFileNames.Contains(newFileName) && !cbOverride.Checked)
                    continue;

                m_Tasks.Add(RunCopyTask(fromFileName, newFileName));
            }
        }

        private void FileCopy_Click(object sender, EventArgs e)
        {
            var name = txtFileName.Text;
            var fromFileName = Path.Combine(txtFromPath.Text, name);
            if (!File.Exists(fromFileName))
            {
                this.ShowMsg("From file not found");
                return;
            }
            else if (!Directory.Exists(txtToPath.Text))
            {
                this.ShowMsg("To directory not found");
                return;
            }

            var toFileName = Path.Combine(txtToPath.Text, name);
            m_Tasks.Add(RunCopyTask(fromFileName, toFileName));
        }

        private const int BUFFER_SIZE = 1024 * 256;

        private void SplitFileCopy_Click(object sender, EventArgs e)
        {
            if (m_Tasks.Exists((t) => t.Status == TaskStatus.Running))
            {
                this.ShowMsg("There is a running task. Wait.");
                return;
            }

            m_Tasks.Clear();

            if (!Directory.Exists(txtFromPath.Text))
            {
                this.ShowMsg("From directory not found");
                return;
            }

            var name = txtSplitFileName.Text;
            var fromFileName = Path.Combine(txtFromPath.Text, name);

            if (!File.Exists(fromFileName))
            {
                this.ShowMsg("From file not found");
                return;
            }
            else if (!Directory.Exists(txtToPath.Text))
            {
                this.ShowMsg("To directory not found");
                return;
            }

            var toFileName = Path.Combine(txtToPath.Text, name);

            var fileInfo = new FileInfo(fromFileName);

            var task = Task.Factory.StartNew(() =>
            {
                for (long index = 0; index < fileInfo.Length; index += BUFFER_SIZE)
                    RunSplitCopyTask(fromFileName, toFileName, index);

            }, m_Cancel.Token);

            m_Tasks.Add(task);
        }

        public Task RunCopyTask(string fromFileName, string toFileName)
        {
            var task = Task.Factory.StartNew<bool>((o) =>
            {
                KeyValue<string, int> kv = (KeyValue<string, int>)o;

                try
                {
                    File.Copy(kv.Key, kv.Value1, true);
                    m_CurrentCnt.Send(UIShowMessage, kv.ToString());
                    return true;
                }
                catch (Exception ex)
                {
                    m_CurrentCnt.Send(UIShowMessage, ex.Message);
                    return false;
                }

            }, new KeyValue<string, int>(fromFileName, toFileName), m_Cancel.Token);

            return task;
        }

        public void RunSplitCopyTask(string fromFileName, string toFileName, long offset)
        {
            var task = Task.Factory.StartNew<bool>((o) =>
            {
                KeyValue<string, long> kv = (KeyValue<string, long>)o;

                byte[] buffer = BufferCache.Instance.Get(BUFFER_SIZE);

                try
                {
                    using (var fromFile = new FileStream(kv.Key, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, true))
                    using (var toFile = new FileStream(kv.Value1, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, BUFFER_SIZE, true))
                    {
                        fromFile.Seek(kv.Value2, SeekOrigin.Begin);
                        int size = fromFile.Read(buffer, 0, BUFFER_SIZE);
                        toFile.Seek(kv.Value2, SeekOrigin.Begin);
                        toFile.Write(buffer, 0, size);
                        toFile.Flush(true);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    m_CurrentCnt.Send(UIShowMessage, ex.Message);
                    return false;
                }
                finally
                {
                    BufferCache.Instance.Set(buffer);
                    Thread.Yield();
                }

            }, new KeyValue<string, long>(fromFileName, toFileName, offset), TaskCreationOptions.AttachedToParent);
        }

        public void UIShowMessage(object data)
        {
            if (!this.IsDisposed)
                txtMessage.Text = Convert.ToString(data);
        }
    }

    public static class Ex
    {
        public static bool ShowMsg(this Form form, string message, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            return MessageBox.Show(form, message, Application.ProductName, MessageBoxButtons.YesNo, icon) == DialogResult.Yes;
        }

        public static string concat(this string str, params object[] values)
        {
            StringBuilder b = new StringBuilder(str);
            foreach (object s in values)
                b.Append(s);
            return b.ToString();
        }

        public static void DisposeSafe(this IDisposable obj)
        {
            using (obj) ;
        }
    }

    public class BufferCache
    {
        private readonly ConcurrentQueue<byte[]> m_Buffers = new ConcurrentQueue<byte[]>();

        private static readonly BufferCache ms_Instance = new BufferCache();

        public static BufferCache Instance
        {
            get
            {
                return ms_Instance;
            }
        }

        public byte[] Get(int size)
        {
            byte[] buffer;
            if (m_Buffers.TryDequeue(out buffer))
                return buffer;
            return new byte[size];
        }

        public void Set(byte[] buffer)
        {
            m_Buffers.Enqueue(buffer);
        }
    }
}
