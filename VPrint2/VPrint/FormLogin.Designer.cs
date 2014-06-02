namespace VPrinting
{
    partial class FormLogin
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonEnter = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblEmailSupport = new System.Windows.Forms.LinkLabel();
            this.lblUpdateVersion = new System.Windows.Forms.LinkLabel();
            this.cbCountryID = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.pictureBoxPtfLogo = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPtfLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label1.Location = new System.Drawing.Point(136, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(219, 71);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(202, 20);
            this.textBoxUsername.TabIndex = 1;
            this.textBoxUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(219, 97);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(202, 20);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label2.Location = new System.Drawing.Point(136, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // buttonEnter
            // 
            this.buttonEnter.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonEnter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonEnter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonEnter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnter.Image = ((System.Drawing.Image)(resources.GetObject("buttonEnter.Image")));
            this.buttonEnter.Location = new System.Drawing.Point(354, 142);
            this.buttonEnter.Name = "buttonEnter";
            this.buttonEnter.Size = new System.Drawing.Size(67, 26);
            this.buttonEnter.TabIndex = 4;
            this.buttonEnter.UseVisualStyleBackColor = true;
            this.buttonEnter.Click += new System.EventHandler(this.Enter_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblEmailSupport);
            this.panel1.Controls.Add(this.lblUpdateVersion);
            this.panel1.Controls.Add(this.cbCountryID);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lblVersion);
            this.panel1.Controls.Add(this.pictureBoxPtfLogo);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.buttonEnter);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxPassword);
            this.panel1.Controls.Add(this.textBoxUsername);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(451, 214);
            this.panel1.TabIndex = 5;
            // 
            // lblEmailSupport
            // 
            this.lblEmailSupport.AutoSize = true;
            this.lblEmailSupport.Location = new System.Drawing.Point(27, 189);
            this.lblEmailSupport.Name = "lblEmailSupport";
            this.lblEmailSupport.Size = new System.Drawing.Size(72, 13);
            this.lblEmailSupport.TabIndex = 11;
            this.lblEmailSupport.TabStop = true;
            this.lblEmailSupport.Text = "Email Support";
            this.lblEmailSupport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.EmailSupport_LinkClicked);
            // 
            // lblUpdateVersion
            // 
            this.lblUpdateVersion.AutoSize = true;
            this.lblUpdateVersion.Location = new System.Drawing.Point(341, 189);
            this.lblUpdateVersion.Name = "lblUpdateVersion";
            this.lblUpdateVersion.Size = new System.Drawing.Size(80, 13);
            this.lblUpdateVersion.TabIndex = 10;
            this.lblUpdateVersion.TabStop = true;
            this.lblUpdateVersion.Text = "Update Version";
            this.lblUpdateVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UpdateVersion_LinkClicked);
            // 
            // cbCountryID
            // 
            this.cbCountryID.FormattingEnabled = true;
            this.cbCountryID.Location = new System.Drawing.Point(219, 128);
            this.cbCountryID.Name = "cbCountryID";
            this.cbCountryID.Size = new System.Drawing.Size(88, 21);
            this.cbCountryID.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label4.Location = new System.Drawing.Point(149, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Country";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(136, 189);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(10, 13);
            this.lblVersion.TabIndex = 7;
            this.lblVersion.Text = ".";
            // 
            // pictureBoxPtfLogo
            // 
            this.pictureBoxPtfLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxPtfLogo.Image")));
            this.pictureBoxPtfLogo.Location = new System.Drawing.Point(30, 52);
            this.pictureBoxPtfLogo.Name = "pictureBoxPtfLogo";
            this.pictureBoxPtfLogo.Size = new System.Drawing.Size(83, 116);
            this.pictureBoxPtfLogo.TabIndex = 6;
            this.pictureBoxPtfLogo.TabStop = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label3.Location = new System.Drawing.Point(1, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(449, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = " Login";
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(81)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(475, 238);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPtfLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonEnter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBoxPtfLogo;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbCountryID;
        private System.Windows.Forms.LinkLabel lblUpdateVersion;
        private System.Windows.Forms.LinkLabel lblEmailSupport;
    }
}