namespace HobexServer
{
    partial class FintraxHobexServer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_FileWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.m_FileWatcher)).BeginInit();
            // 
            // m_FileWatcher
            // 
            this.m_FileWatcher.EnableRaisingEvents = true;
            this.m_FileWatcher.Path = "C:\\";
            this.m_FileWatcher.Created += new System.IO.FileSystemEventHandler(this.File_Created);
            // 
            // FintraxHobexServer
            // 
            this.ServiceName = "FintraxHobexServer";
            ((System.ComponentModel.ISupportInitialize)(this.m_FileWatcher)).EndInit();

        }

        #endregion

        private System.IO.FileSystemWatcher m_FileWatcher;
    }
}
