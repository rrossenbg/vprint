using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BizTalkFiles
{
    public static class ClassExtentions
    {
        #region THREAD

        public static void AbortSafe(this Thread th)
        {
            try
            {
                if (th != null)
                {
                    th.Abort();
                    th.Join(TimeSpan.FromSeconds(5.0));
                }
            }
            catch
            {
            }
        }

        public static void JoinSafe(this Thread th)
        {
            if (th != null)
                th.Join();
        }

        #endregion

        #region WINFORMS

        public static void ShowError(this IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        public static void ShowInformation(this IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public static DialogResult ShowQuestion(this IWin32Window owner, string message, MessageBoxButtons buttons)
        {
            return MessageBox.Show(owner, message, Application.ProductName, buttons, MessageBoxIcon.Question);
        }

        public static void ShowWarning(this IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static bool IsReady(this Control cnt)
        {
            return (cnt.IsHandleCreated && !cnt.IsDisposed);
        }

        public static void SwitchBackColor(this Control cnt, Color color1, Color color2)
        {
            cnt.BackColor = (cnt.BackColor == color1) ? color2 : color1;
        }

        #endregion

        #region STRING

        public static bool CompareNoCase(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string Unique(this string str)
        {
            var u = Guid.NewGuid().ToString().Replace('-', '_');
            return string.Concat(str, u);
        }

        public static string Limit(this string str, int length)
        {
            if (str == null)
                return null;
            return str.Substring(0, Math.Min(str.Length, length));
        }

        public static string concat(this string str, params object[] values)
        {
            return string.Concat(str, string.Concat(values));
        }

        public static string format(this string format, params object[] values)
        {
            return string.Format(format, values);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsInt(this string value)
        {
            int v;
            return int.TryParse(value, out v);
        }

        public static string TrimSafe(this string value)
        {
            if (value == null)
                return value;
            return value.Trim();
        }

        #endregion

        public static bool Exists<T>(this IEnumerable<T> items, Func<T, bool> funct)
        {
            foreach (var item in items)
                if (funct(item))
                    return true;
            return false;
        }

        /// <summary>
        /// The file is unavailable because it is:
        /// still being written to
        /// or being processed by another thread
        /// or does not exist (has already been processed)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileLocked(this FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    stream.Close();

                //file is not locked
                return false;
            }
            catch
            {
                return true;
            }
        }

        public static void RunSafe<T>(this Action<T> act, T arg = default(T))
        {
            try
            {
                act(arg);
            }
            catch
            {
                //
            }
        }

        /// <summary>
        /// Please note that the following line won't work if you try this on a network folder,
        /// like \\Machine\C$
        /// simply remove the \\?\ part in this case or use \\?\UNC\ prefix
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="maxfiles"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <example>
        /// const int MAXFILES = 2;
        /// foreach (var name in new DirectoryInfo(@"C:\SERIALPORT").GetFiles(MAXFILES, "*.cpp"))
        ///     Console.WriteLine(name);
        /// </example>
        public static IEnumerable<FileInfo> GetFiles(this DirectoryInfo directory, int maxfiles, string filter = "*")
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("filter");

            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            int files = 0;
            Kernel32.WIN32_FIND_DATA findData;

            using (Kernel32.SafeFindHandle findHandle = Kernel32.FindFirstFile(@"\\?\" + directory.FullName + @"\" + filter, out findData))
            {
                if (!findHandle.IsInvalid)
                {
                    do
                    {
                        if ((findData.dwFileAttributes & FileAttributes.Directory) == 0)
                        {
                            // File
                            files++;
                            var path = Path.Combine(directory.FullName, findData.cFileName);
                            yield return new FileInfo(path);
                        }
                    }
                    while (files < maxfiles && Kernel32.FindNextFile(findHandle, out findData));
                }
            }
        }

        public static bool Test(this SqlConnection conn, out string message)
        {
            try
            {
                using (conn)
                {
                    conn.Open();
                    message = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}