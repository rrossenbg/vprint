/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CardCodeCover
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            var list = DataAccess.SelectAllTemplateInfos().ConvertAll<TemplateInfoLight>((da) => new TemplateInfoLight(da));
            var blist = new BindingList<TemplateInfoLight>(list);
            dataGridView1.DataSource = blist;
            base.OnLoad(e);
        }

        private void AddNewMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new MatchForm())
                form.ShowDialog(this);
        }

        private void MatchMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new MatchForm())
                form.ShowDialog(this);
        }

        private void DataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column.Name == "Name")
                e.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void RefreshMenuItem_Click(object sender, EventArgs e)
        {
            var list = DataAccess.SelectAllTemplateInfos().ConvertAll<TemplateInfoLight>((da) => new TemplateInfoLight(da));
            var blist = new BindingList<TemplateInfoLight>(list);
            dataGridView1.DataSource = blist;
        }

        private void EditMenuItem_Click(object sender, EventArgs e)
        {
            var datagrid = this.contextMenuStrip1.SourceControl;
            var selectedRows = ((DataGridView)datagrid).SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
            {
                using (var form = new MatchForm())
                {
                    var light = (TemplateInfoLight)selectedRows[0].DataBoundItem;
                    DataAccess.SelectTemplate((DataAccess.TemplateInfoDb)light);
                    form.MatchTemplate = new TemplateInfo(light);
                    form.ShowDialog(this);
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            var datagrid = this.contextMenuStrip1.SourceControl;
            var selectedRows = ((DataGridView)datagrid).SelectedRows;
            foreach (DataGridViewRow row in selectedRows.ToList())
            {
                int id = row.DataGridView["Id", row.Index].Value.Cast<int>();
                DataAccess.DeleteTemplateById(id);
                dataGridView1.Rows.Remove(row);
            }
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
