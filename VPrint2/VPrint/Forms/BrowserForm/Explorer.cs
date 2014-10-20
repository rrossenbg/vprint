/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using VPrinting.Common;
using VPrinting.Tools;

namespace VPrinting.Forms.Explorer
{
    public class Explorer : Form
    {
        #region CONTROLS
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ImageList m_imageListTreeView;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem runCopyWaitMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem closeToolStripMenuItem;
        private BackgroundWorker backgroundWorker1;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripMenuItem cancelToolStripMenuItem;
        private ToolStripMenuItem runCopyMenuItem;
        private System.ComponentModel.IContainer components;
        #endregion

        private class ListViewItemComparer : IComparer
        {
            private int m_column;

            private int m_comparertype = 1;

            public ListViewItemComparer()
            {
                m_column = 0;
            }

            public ListViewItemComparer(int column)
            {
                m_column = column;
            }

            public void Reset(int column)
            {
                if (m_column == column)
                    m_comparertype *= -1;
                else
                    m_column = column;
            }

            public int Compare(object x, object y)
            {
                int returnVal = m_comparertype * string.Compare(((ListViewItem)x).SubItems[m_column].Text, ((ListViewItem)y).SubItems[m_column].Text);
                return returnVal;
            }
        }

        public Explorer()
        {
            InitializeComponent();
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.WorkerReportsProgress = true;

            PopulateDriveList();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnClosed(EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            base.OnClosed(e);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Explorer));
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runCopyWaitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_imageListTreeView = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvFolders
            // 
            this.tvFolders.ContextMenuStrip = this.contextMenuStrip1;
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvFolders.ImageIndex = 0;
            this.tvFolders.ImageList = this.m_imageListTreeView;
            this.tvFolders.Location = new System.Drawing.Point(0, 0);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.SelectedImageIndex = 0;
            this.tvFolders.Size = new System.Drawing.Size(224, 520);
            this.tvFolders.TabIndex = 2;
            this.tvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterSelect);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runCopyMenuItem,
            this.runCopyWaitMenuItem,
            this.cancelToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(147, 98);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenu_Opening);
            // 
            // runCopyMenuItem
            // 
            this.runCopyMenuItem.Name = "runCopyMenuItem";
            this.runCopyMenuItem.Size = new System.Drawing.Size(146, 22);
            this.runCopyMenuItem.Text = "Run Copy";
            this.runCopyMenuItem.Click += new System.EventHandler(this.BrowseMenu_Click);
            // 
            // runCopyWaitMenuItem
            // 
            this.runCopyWaitMenuItem.Name = "runCopyWaitMenuItem";
            this.runCopyWaitMenuItem.Size = new System.Drawing.Size(146, 22);
            this.runCopyWaitMenuItem.Text = "Run Copy Wait";
            this.runCopyWaitMenuItem.Click += new System.EventHandler(this.BrowseMenu_Click);
            // 
            // cancelToolStripMenuItem
            // 
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.cancelToolStripMenuItem.Text = "Cancel";
            this.cancelToolStripMenuItem.Click += new System.EventHandler(this.CancelMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
            // 
            // m_imageListTreeView
            // 
            this.m_imageListTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageListTreeView.ImageStream")));
            this.m_imageListTreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.m_imageListTreeView.Images.SetKeyName(0, "");
            this.m_imageListTreeView.Images.SetKeyName(1, "");
            this.m_imageListTreeView.Images.SetKeyName(2, "");
            this.m_imageListTreeView.Images.SetKeyName(3, "");
            this.m_imageListTreeView.Images.SetKeyName(4, "");
            this.m_imageListTreeView.Images.SetKeyName(5, "");
            this.m_imageListTreeView.Images.SetKeyName(6, "");
            this.m_imageListTreeView.Images.SetKeyName(7, "");
            this.m_imageListTreeView.Images.SetKeyName(8, "");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(224, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 498);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // lvFiles
            // 
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.HideSelection = false;
            this.lvFiles.Location = new System.Drawing.Point(227, 0);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(671, 498);
            this.lvFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvFiles.TabIndex = 4;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvFiles_ColumnClick);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
            this.menuItem1.Text = "&File";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "&Close";
            this.menuItem2.Click += new System.EventHandler(this.MenuItem2_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(224, 498);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(674, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(400, 16);
            // 
            // Explorer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(898, 520);
            this.Controls.Add(this.lvFiles);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tvFolders);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Explorer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "File Browser";
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void PopulateDriveList()
        {
            TreeNode nodeTreeNode;
            int imageIndex = 0;
            int selectIndex = 0;

            const int Removable = 2;
            const int LocalDisk = 3;
            const int Network = 4;
            const int CD = 5;
            const int RAMDrive = 6;

            this.Cursor = Cursors.WaitCursor;
            tvFolders.Nodes.Clear();
            nodeTreeNode = new TreeNode("My Computer", 0, 0);
            tvFolders.Nodes.Add(nodeTreeNode);

            TreeNodeCollection nodeCollection = nodeTreeNode.Nodes;

            ManagementObjectCollection queryCollection = getDrives();
            foreach (ManagementObject mo in queryCollection)
            {
                switch (int.Parse(mo["DriveType"].ToString()))
                {
                    case Removable:			//removable drives
                        imageIndex = 5;
                        selectIndex = 5;
                        break;
                    case LocalDisk:			//Local drives
                        imageIndex = 6;
                        selectIndex = 6;
                        break;
                    case CD:				//CD rom drives
                        imageIndex = 7;
                        selectIndex = 7;
                        break;
                    case Network:			//Network drives
                        imageIndex = 8;
                        selectIndex = 8;
                        break;
                    default:				//defalut to folder
                        imageIndex = 2;
                        selectIndex = 3;
                        break;
                }
                nodeTreeNode = new TreeNode(mo["Name"].ToString() + "\\", imageIndex, selectIndex);

                nodeCollection.Add(nodeTreeNode);

            }

            InitListView();

            this.Cursor = Cursors.Default;
        }

        private void tvFolders_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            TreeNode nodeCurrent = e.Node;

            nodeCurrent.Nodes.Clear();

            if (nodeCurrent.SelectedImageIndex == 0)
            {
                PopulateDriveList();
            }
            else
            {
                PopulateDirectory(nodeCurrent, nodeCurrent.Nodes);
            }
            this.Cursor = Cursors.Default;
        }

        protected void InitListView()
        {
            lvFiles.Clear();		
            lvFiles.Columns.Add("Name", 150, System.Windows.Forms.HorizontalAlignment.Left);
            lvFiles.Columns.Add("Size", 75, System.Windows.Forms.HorizontalAlignment.Right);
            lvFiles.Columns.Add("Created", 140, System.Windows.Forms.HorizontalAlignment.Left);
            lvFiles.Columns.Add("Modified", 140, System.Windows.Forms.HorizontalAlignment.Left);

        }

        protected void PopulateDirectory(TreeNode nodeCurrent, TreeNodeCollection nodeCurrentCollection)
        {
            TreeNode nodeDir;
            int imageIndex = 2;		//unselected image index
            int selectIndex = 3;	//selected image index

            if (nodeCurrent.SelectedImageIndex != 0)
            {
                try
                {
                    if (Directory.Exists(getFullPath(nodeCurrent.FullPath)) == false)
                    {
                        MessageBox.Show("Directory or path " + nodeCurrent.ToString() + " does not exist.");
                    }
                    else
                    {
                        PopulateFiles(nodeCurrent);

                        string[] stringDirectories = Directory.GetDirectories(getFullPath(nodeCurrent.FullPath));
                        string stringFullPath = "";
                        string stringPathName = "";

                        foreach (string stringDir in stringDirectories)
                        {
                            stringFullPath = stringDir;
                            stringPathName = GetPathName(stringFullPath);

                            nodeDir = new TreeNode(stringPathName.ToString(), imageIndex, selectIndex);
                            nodeCurrentCollection.Add(nodeDir);
                        }
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("Error: Drive not ready or directory does not exist.");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Error: Drive or directory access denided.");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e);
                }
            }
        }

        protected string GetPathName(string stringPath)
        {
            string[] stringSplit = stringPath.Split('\\');
            int _maxIndex = stringSplit.Length;
            return stringSplit[_maxIndex - 1];
        }

        protected void PopulateFiles(TreeNode nodeCurrent)
        {
            string[] lvData = new string[4];

            InitListView();

            if (nodeCurrent.SelectedImageIndex != 0)
            {
                if (Directory.Exists((string)getFullPath(nodeCurrent.FullPath)) == false)
                {
                    MessageBox.Show("Directory or path " + nodeCurrent.ToString() + " does not exist.");
                }
                else
                {
                    try
                    {
                        string[] stringFiles = Directory.GetFiles(getFullPath(nodeCurrent.FullPath));
                        string stringFileName = "";
                        DateTime dtCreateDate, dtModifyDate;
                        Int64 lFileSize = 0;

                        //loop throught all files
                        foreach (string stringFile in stringFiles)
                        {
                            stringFileName = stringFile;
                            FileInfo fileInfo = new FileInfo(stringFileName);
                            lFileSize = fileInfo.Length;
                            dtCreateDate = fileInfo.CreationTime; //GetCreationTime(stringFileName);
                            dtModifyDate = fileInfo.LastWriteTime; //GetLastWriteTime(stringFileName);

                            //create listview data
                            lvData[0] = GetPathName(stringFileName);
                            lvData[1] = formatSize(lFileSize);

                            //check if file is in local current day light saving time
                            if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dtCreateDate) == false)
                            {
                                //not in day light saving time adjust time
                                lvData[2] = formatDate(dtCreateDate.AddHours(1));
                            }
                            else
                            {
                                //is in day light saving time adjust time
                                lvData[2] = formatDate(dtCreateDate);
                            }

                            //check if file is in local current day light saving time
                            if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dtModifyDate) == false)
                            {
                                //not in day light saving time adjust time
                                lvData[3] = formatDate(dtModifyDate.AddHours(1));
                            }
                            else
                            {
                                //not in day light saving time adjust time
                                lvData[3] = formatDate(dtModifyDate);
                            }


                            //Create actual list item
                            ListViewItem lvItem = new ListViewItem(lvData, 0);
                            lvFiles.Items.Add(lvItem);


                        }
                    }
                    catch (IOException e)
                    {
                        MessageBox.Show("Error: Drive not ready or directory does not exist.");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        MessageBox.Show("Error: Drive or directory access denided.");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error: " + e);
                    }
                }
            }
        }

        protected string getFullPath(string stringPath)
        {
            //Get Full path
            string stringParse = "";
            //remove My Computer from path.
            stringParse = stringPath.Replace("My Computer\\", "");
            return stringParse;
        }

        protected ManagementObjectCollection getDrives()
        {
            //get drive collection
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * From Win32_LogicalDisk ");
            ManagementObjectCollection queryCollection = query.Get();
            return queryCollection;
        }

        protected string formatDate(DateTime dtDate)
        {
            //Get date and time in short format
            string stringDate = dtDate.ToShortDateString().ToString() + " " + dtDate.ToShortTimeString().ToString();
            return stringDate;
        }

        protected string formatSize(Int64 lSize)
        {
            //Format number to KB
            string stringSize = "";
            NumberFormatInfo myNfi = new NumberFormatInfo();

            Int64 lKBSize = 0;

            if (lSize < 1024)
            {
                if (lSize == 0)
                {
                    //zero byte
                    stringSize = "0";
                }
                else
                {
                    //less than 1K but not zero byte
                    stringSize = "1";
                }
            }
            else
            {
                //convert to KB
                lKBSize = lSize / 1024;
                //format number with default format
                stringSize = lKBSize.ToString("n", myNfi);
                //remove decimal
                stringSize = stringSize.Replace(".00", "");
            }

            return stringSize + " KB";

        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var cnt = contextMenuStrip1.SourceControl;
            var note = (contextMenuStrip1.SourceControl as TreeView).SelectedNode;
            e.Cancel = (note == null);
        }

        private void BrowseMenu_Click(object sender, EventArgs e)
        {
            var cnt = contextMenuStrip1.SourceControl;
            var note = (contextMenuStrip1.SourceControl as TreeView).SelectedNode;
            if (note != null)
            {
                var fromPath = getFullPath(note.FullPath);
                bool wait = sender == runCopyWaitMenuItem;
                var list = new List<FileInfo>();

                foreach (ListViewItem lv in lvFiles.SelectedItems)
                    list.Add(new FileInfo(Path.Combine(fromPath, lv.Text)));

                backgroundWorker1.RunWorkerAsync(new Tuple<string, bool, List<FileInfo>>(fromPath, wait, list));
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var ev = DelegateHelper.GetEvent();
            try
            {
                Tuple<string, bool, List<FileInfo>> para = e.Argument.Cast<Tuple<string, bool, List<FileInfo>>>();
                string pathTo = StateSaver.Default.Get<string>(Strings.tbScanDirectory, "C:\\");
                var timeout = StateSaver.Default.Get<TimeSpan>(Strings.ScanCopyTimeout, TimeSpan.FromSeconds(20));
                var wait = StateSaver.Default.Get<TimeSpan>(Strings.ScanCopyWait, TimeSpan.FromSeconds(2));

                backgroundWorker1.ReportProgress(0);

                var files = (para.Item3 != null && para.Item3.Count > 0) ? para.Item3.ToArray() : new DirectoryInfo(para.Item1).GetFiles();

                int count = 0;

                foreach (var file in files)
                {
                    var newFileName = Path.Combine(pathTo, file.Name);
                    file.CopyTo(newFileName, true);
                    backgroundWorker1.ReportProgress(Convert.ToInt32(((float)++count / files.Length) * 100));
                    if (para.Item2)
                    {
                        if (!ev.WaitOne(timeout))
                            throw new TimeoutException();
                    }
                    else
                    {
                        Thread.Sleep(wait);
                    }
                    if (backgroundWorker1.CancellationPending)
                        break;
                }
            }
            finally
            {
                ev.Close();
            }
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                this.ShowError(e.Error.Message);
            else if (e.Cancelled)
                this.ShowExclamation("Cancelled by the user");
        }

        private void CancelMenuItem_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void MenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lvFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewItemComparer comp = lvFiles.ListViewItemSorter as ListViewItemComparer;
            if (comp == null)
                lvFiles.ListViewItemSorter = new ListViewItemComparer(e.Column);
            else
                comp.Reset(e.Column);
            lvFiles.Sort();
        }
    }
}
