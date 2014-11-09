namespace PRFCreator
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ftf_label = new System.Windows.Forms.Label();
            this.ftf_textbox = new System.Windows.Forms.TextBox();
            this.ftf_button = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.su_label = new System.Windows.Forms.Label();
            this.su_textbox = new System.Windows.Forms.TextBox();
            this.su_button = new System.Windows.Forms.Button();
            this.create_button = new System.Windows.Forms.Button();
            this.status_textbox = new System.Windows.Forms.RichTextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.version_label = new System.Windows.Forms.Label();
            this.rec_textbox = new System.Windows.Forms.TextBox();
            this.rec_label = new System.Windows.Forms.Label();
            this.rec_button = new System.Windows.Forms.Button();
            this.job_label = new System.Windows.Forms.Label();
            this.jobnum_label = new System.Windows.Forms.Label();
            this.include_label = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.include_checklist = new PRFCreator.CustomCheckedListBox();
            this.options_checklist = new PRFCreator.CustomCheckedListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ftf_label
            // 
            this.ftf_label.AutoSize = true;
            this.ftf_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.ftf_label.Location = new System.Drawing.Point(6, 15);
            this.ftf_label.Name = "ftf_label";
            this.ftf_label.Size = new System.Drawing.Size(51, 15);
            this.ftf_label.TabIndex = 0;
            this.ftf_label.Text = "FTF File";
            // 
            // ftf_textbox
            // 
            this.ftf_textbox.Location = new System.Drawing.Point(6, 33);
            this.ftf_textbox.Name = "ftf_textbox";
            this.ftf_textbox.Size = new System.Drawing.Size(429, 20);
            this.ftf_textbox.TabIndex = 1;
            // 
            // ftf_button
            // 
            this.ftf_button.Location = new System.Drawing.Point(438, 31);
            this.ftf_button.Name = "ftf_button";
            this.ftf_button.Size = new System.Drawing.Size(30, 23);
            this.ftf_button.TabIndex = 2;
            this.ftf_button.Text = "...";
            this.ftf_button.UseVisualStyleBackColor = true;
            this.ftf_button.Click += new System.EventHandler(this.ftf_button_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // su_label
            // 
            this.su_label.AutoSize = true;
            this.su_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.su_label.Location = new System.Drawing.Point(6, 56);
            this.su_label.Name = "su_label";
            this.su_label.Size = new System.Drawing.Size(77, 15);
            this.su_label.TabIndex = 3;
            this.su_label.Text = "SuperSU Zip";
            // 
            // su_textbox
            // 
            this.su_textbox.Location = new System.Drawing.Point(6, 76);
            this.su_textbox.Name = "su_textbox";
            this.su_textbox.Size = new System.Drawing.Size(429, 20);
            this.su_textbox.TabIndex = 4;
            // 
            // su_button
            // 
            this.su_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.su_button.Location = new System.Drawing.Point(438, 74);
            this.su_button.Name = "su_button";
            this.su_button.Size = new System.Drawing.Size(30, 23);
            this.su_button.TabIndex = 5;
            this.su_button.Text = "...";
            this.su_button.UseVisualStyleBackColor = true;
            this.su_button.Click += new System.EventHandler(this.su_button_Click);
            // 
            // create_button
            // 
            this.create_button.Location = new System.Drawing.Point(518, 294);
            this.create_button.Name = "create_button";
            this.create_button.Size = new System.Drawing.Size(79, 23);
            this.create_button.TabIndex = 8;
            this.create_button.Text = "Create";
            this.create_button.UseVisualStyleBackColor = true;
            this.create_button.Click += new System.EventHandler(this.create_button_Click);
            // 
            // status_textbox
            // 
            this.status_textbox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.status_textbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_textbox.Location = new System.Drawing.Point(15, 162);
            this.status_textbox.Name = "status_textbox";
            this.status_textbox.ReadOnly = true;
            this.status_textbox.Size = new System.Drawing.Size(582, 129);
            this.status_textbox.TabIndex = 9;
            this.status_textbox.Text = "";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 297);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(435, 16);
            this.progressBar.TabIndex = 10;
            // 
            // version_label
            // 
            this.version_label.AutoSize = true;
            this.version_label.Font = new System.Drawing.Font("Consolas", 7.5F);
            this.version_label.Location = new System.Drawing.Point(583, 1);
            this.version_label.Name = "version_label";
            this.version_label.Size = new System.Drawing.Size(0, 12);
            this.version_label.TabIndex = 11;
            // 
            // rec_textbox
            // 
            this.rec_textbox.Location = new System.Drawing.Point(6, 117);
            this.rec_textbox.Name = "rec_textbox";
            this.rec_textbox.Size = new System.Drawing.Size(429, 20);
            this.rec_textbox.TabIndex = 12;
            // 
            // rec_label
            // 
            this.rec_label.AutoSize = true;
            this.rec_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.rec_label.Location = new System.Drawing.Point(6, 99);
            this.rec_label.Name = "rec_label";
            this.rec_label.Size = new System.Drawing.Size(77, 15);
            this.rec_label.TabIndex = 13;
            this.rec_label.Text = "Recovery Zip";
            // 
            // rec_button
            // 
            this.rec_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rec_button.Location = new System.Drawing.Point(438, 115);
            this.rec_button.Name = "rec_button";
            this.rec_button.Size = new System.Drawing.Size(30, 23);
            this.rec_button.TabIndex = 14;
            this.rec_button.Text = "...";
            this.rec_button.UseVisualStyleBackColor = true;
            this.rec_button.Click += new System.EventHandler(this.dr_button_Click);
            // 
            // job_label
            // 
            this.job_label.AutoSize = true;
            this.job_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.job_label.Location = new System.Drawing.Point(457, 297);
            this.job_label.Name = "job_label";
            this.job_label.Size = new System.Drawing.Size(30, 15);
            this.job_label.TabIndex = 15;
            this.job_label.Text = "Job:";
            // 
            // jobnum_label
            // 
            this.jobnum_label.AutoSize = true;
            this.jobnum_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jobnum_label.Location = new System.Drawing.Point(485, 297);
            this.jobnum_label.Name = "jobnum_label";
            this.jobnum_label.Size = new System.Drawing.Size(0, 15);
            this.jobnum_label.TabIndex = 16;
            // 
            // include_label
            // 
            this.include_label.AutoSize = true;
            this.include_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.include_label.Location = new System.Drawing.Point(3, 15);
            this.include_label.Name = "include_label";
            this.include_label.Size = new System.Drawing.Size(47, 15);
            this.include_label.TabIndex = 18;
            this.include_label.Text = "Include";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ftf_textbox);
            this.groupBox1.Controls.Add(this.ftf_label);
            this.groupBox1.Controls.Add(this.su_label);
            this.groupBox1.Controls.Add(this.su_textbox);
            this.groupBox1.Controls.Add(this.rec_label);
            this.groupBox1.Controls.Add(this.rec_button);
            this.groupBox1.Controls.Add(this.rec_textbox);
            this.groupBox1.Controls.Add(this.ftf_button);
            this.groupBox1.Controls.Add(this.su_button);
            this.groupBox1.Location = new System.Drawing.Point(15, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(474, 147);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.include_checklist);
            this.groupBox2.Controls.Add(this.include_label);
            this.groupBox2.Location = new System.Drawing.Point(495, 9);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(102, 104);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.options_checklist);
            this.groupBox3.Location = new System.Drawing.Point(495, 105);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(102, 51);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            // 
            // include_checklist
            // 
            this.include_checklist.AccessibleDescription = "";
            this.include_checklist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.include_checklist.BackColor = System.Drawing.SystemColors.Menu;
            this.include_checklist.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.include_checklist.CheckOnClick = true;
            this.include_checklist.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.include_checklist.FormattingEnabled = true;
            this.include_checklist.Items.AddRange(new object[] {
            "Kernel",
            "FOTAKernel",
            "Modem",
            "LTALabel"});
            this.include_checklist.Location = new System.Drawing.Point(5, 33);
            this.include_checklist.Name = "include_checklist";
            this.include_checklist.Size = new System.Drawing.Size(92, 64);
            this.include_checklist.TabIndex = 17;
            // 
            // options_checklist
            // 
            this.options_checklist.BackColor = System.Drawing.SystemColors.Menu;
            this.options_checklist.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.options_checklist.CheckOnClick = true;
            this.options_checklist.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.options_checklist.FormattingEnabled = true;
            this.options_checklist.Items.AddRange(new object[] {
            "Sign zip"});
            this.options_checklist.Location = new System.Drawing.Point(6, 12);
            this.options_checklist.Name = "options_checklist";
            this.options_checklist.Size = new System.Drawing.Size(90, 16);
            this.options_checklist.TabIndex = 22;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 326);
            this.Controls.Add(this.version_label);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.jobnum_label);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.status_textbox);
            this.Controls.Add(this.create_button);
            this.Controls.Add(this.job_label);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(627, 365);
            this.MinimumSize = new System.Drawing.Size(627, 365);
            this.Name = "Form1";
            this.Text = "PRFCreator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ftf_label;
        private System.Windows.Forms.Button ftf_button;
        private System.Windows.Forms.Label su_label;
        private System.Windows.Forms.Button su_button;
        private System.Windows.Forms.Button create_button;
        public System.Windows.Forms.RichTextBox status_textbox;
        private System.Windows.Forms.Label version_label;
        public System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label rec_label;
        private System.Windows.Forms.Button rec_button;
        public System.Windows.Forms.TextBox ftf_textbox;
        public System.Windows.Forms.TextBox su_textbox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.TextBox rec_textbox;
        private System.Windows.Forms.Label job_label;
        public System.Windows.Forms.Label jobnum_label;
        private System.Windows.Forms.Label include_label;
        private System.Windows.Forms.GroupBox groupBox1;
        public CustomCheckedListBox include_checklist;
        private System.Windows.Forms.GroupBox groupBox2;
        public CustomCheckedListBox options_checklist;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

