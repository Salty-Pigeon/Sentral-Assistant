namespace Sentral_Assistant
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.SchoolListBox = new System.Windows.Forms.ComboBox();
            this.servicesList = new System.Windows.Forms.ListBox();
            this.outputBox = new System.Windows.Forms.TextBox();
            this.executeButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.demoBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "School:";
            // 
            // SchoolListBox
            // 
            this.SchoolListBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SchoolListBox.FormattingEnabled = true;
            this.SchoolListBox.Location = new System.Drawing.Point(61, 12);
            this.SchoolListBox.Name = "SchoolListBox";
            this.SchoolListBox.Size = new System.Drawing.Size(244, 21);
            this.SchoolListBox.TabIndex = 2;
            this.SchoolListBox.SelectedIndexChanged += new System.EventHandler(this.SchoolListBox_SelectedIndexChanged);
            // 
            // servicesList
            // 
            this.servicesList.FormattingEnabled = true;
            this.servicesList.Location = new System.Drawing.Point(15, 58);
            this.servicesList.Name = "servicesList";
            this.servicesList.Size = new System.Drawing.Size(290, 355);
            this.servicesList.TabIndex = 3;
            this.servicesList.SelectedIndexChanged += new System.EventHandler(this.ServicesList_SelectedIndexChanged);
            // 
            // outputBox
            // 
            this.outputBox.AcceptsReturn = true;
            this.outputBox.Location = new System.Drawing.Point(347, 58);
            this.outputBox.Multiline = true;
            this.outputBox.Name = "outputBox";
            this.outputBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputBox.Size = new System.Drawing.Size(413, 355);
            this.outputBox.TabIndex = 4;
            this.outputBox.WordWrap = false;
            this.outputBox.TextChanged += new System.EventHandler(this.OutputBox_TextChanged);
            // 
            // executeButton
            // 
            this.executeButton.AllowDrop = true;
            this.executeButton.Location = new System.Drawing.Point(675, 419);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(75, 23);
            this.executeButton.TabIndex = 5;
            this.executeButton.Text = "Go!";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(347, 15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // demoBox
            // 
            this.demoBox.AutoSize = true;
            this.demoBox.Checked = true;
            this.demoBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.demoBox.Location = new System.Drawing.Point(15, 35);
            this.demoBox.Name = "demoBox";
            this.demoBox.Size = new System.Drawing.Size(58, 17);
            this.demoBox.TabIndex = 7;
            this.demoBox.Text = "DEMO";
            this.demoBox.UseVisualStyleBackColor = true;
            this.demoBox.CheckedChanged += new System.EventHandler(this.DemoBox_CheckedChanged);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.demoBox);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.outputBox);
            this.Controls.Add(this.servicesList);
            this.Controls.Add(this.SchoolListBox);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Sentral Assistant";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox SchoolListBox;
        private System.Windows.Forms.ListBox servicesList;
        private System.Windows.Forms.TextBox outputBox;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox demoBox;
    }
}

