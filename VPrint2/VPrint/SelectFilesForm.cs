using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using VPrinting.Common;

namespace VPrinting
{
    public partial class SelectFilesForm : Form
    {
        static SelectFilesForm()
        {
            //FileProtector.Error += Program.OnThreadException;
        }

        public SelectFilesForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        public static bool ShowFiles(IWin32Window owner, IList<FileInfo> files)
        {
            if (files.Count == 0)
                return false;

            using (SelectFilesForm form = new SelectFilesForm())
            {
                form.LoadData(files);
                form.ShowDialog(owner);
            }
            return true;
        }

        public void LoadData(IList<FileInfo> files)
        {
            foreach (var fi in files)
            {
                int index = dgvData.Rows.Add(dgvData.Rows.Count + 1, fi.Name, "...");
                dgvData.Rows[index].Tag = fi;
            }
        }

        private void Data_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvData.Columns[OpenColumn.Name].Index && e.RowIndex >= 0)
            {
                var tag = dgvData.Rows[e.RowIndex].Tag;

                var protector = new FileProtector();
                protector.Protect((FileInfo)tag);
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
