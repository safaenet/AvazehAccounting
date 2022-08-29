namespace PrintInterface
{
    partial class PrintInvoiceInterface
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPageFooterFontSize = new System.Windows.Forms.ComboBox();
            this.cmbDescriptionFontSize = new System.Windows.Forms.ComboBox();
            this.cmbDetailsFontSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPageHeaderFontSize = new System.Windows.Forms.ComboBox();
            this.ShowUserDescription = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceDescription = new System.Windows.Forms.CheckBox();
            this.ShowCustomerDescription = new System.Windows.Forms.CheckBox();
            this.ShowPhoneNumber = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceId = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceCreatedDate = new System.Windows.Forms.CheckBox();
            this.crystalReportViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbPageFooterFontSize);
            this.groupBox1.Controls.Add(this.cmbDescriptionFontSize);
            this.groupBox1.Controls.Add(this.cmbDetailsFontSize);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbPageHeaderFontSize);
            this.groupBox1.Controls.Add(this.ShowUserDescription);
            this.groupBox1.Controls.Add(this.ShowInvoiceDescription);
            this.groupBox1.Controls.Add(this.ShowCustomerDescription);
            this.groupBox1.Controls.Add(this.ShowPhoneNumber);
            this.groupBox1.Controls.Add(this.ShowInvoiceId);
            this.groupBox1.Controls.Add(this.ShowInvoiceCreatedDate);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(726, 73);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "تنظیمات چاپ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "سایز انتها";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(409, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "سایز توضیحات";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(528, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "سایز رکورد ها";
            // 
            // cmbPageFooterFontSize
            // 
            this.cmbPageFooterFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPageFooterFontSize.FormattingEnabled = true;
            this.cmbPageFooterFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbPageFooterFontSize.Location = new System.Drawing.Point(270, 43);
            this.cmbPageFooterFontSize.Name = "cmbPageFooterFontSize";
            this.cmbPageFooterFontSize.Size = new System.Drawing.Size(35, 21);
            this.cmbPageFooterFontSize.TabIndex = 1;
            this.cmbPageFooterFontSize.SelectedIndexChanged += new System.EventHandler(this.cmbPageFooterFontSize_SelectedIndexChanged);
            // 
            // cmbDescriptionFontSize
            // 
            this.cmbDescriptionFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDescriptionFontSize.FormattingEnabled = true;
            this.cmbDescriptionFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.cmbDescriptionFontSize.Location = new System.Drawing.Point(368, 43);
            this.cmbDescriptionFontSize.Name = "cmbDescriptionFontSize";
            this.cmbDescriptionFontSize.Size = new System.Drawing.Size(35, 21);
            this.cmbDescriptionFontSize.TabIndex = 1;
            this.cmbDescriptionFontSize.SelectedIndexChanged += new System.EventHandler(this.cmbDescriptionFontSize_SelectedIndexChanged);
            // 
            // cmbDetailsFontSize
            // 
            this.cmbDetailsFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDetailsFontSize.FormattingEnabled = true;
            this.cmbDetailsFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbDetailsFontSize.Location = new System.Drawing.Point(487, 43);
            this.cmbDetailsFontSize.Name = "cmbDetailsFontSize";
            this.cmbDetailsFontSize.Size = new System.Drawing.Size(35, 21);
            this.cmbDetailsFontSize.TabIndex = 1;
            this.cmbDetailsFontSize.SelectedIndexChanged += new System.EventHandler(this.cmbDetailsFontSize_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(640, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "سایز مشخصات";
            // 
            // cmbPageHeaderFontSize
            // 
            this.cmbPageHeaderFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPageHeaderFontSize.FormattingEnabled = true;
            this.cmbPageHeaderFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbPageHeaderFontSize.Location = new System.Drawing.Point(603, 43);
            this.cmbPageHeaderFontSize.Name = "cmbPageHeaderFontSize";
            this.cmbPageHeaderFontSize.Size = new System.Drawing.Size(35, 21);
            this.cmbPageHeaderFontSize.TabIndex = 1;
            this.cmbPageHeaderFontSize.SelectedIndexChanged += new System.EventHandler(this.cmbPageHeaderFontSize_SelectedIndexChanged);
            // 
            // ShowUserDescription
            // 
            this.ShowUserDescription.AutoSize = true;
            this.ShowUserDescription.Location = new System.Drawing.Point(12, 20);
            this.ShowUserDescription.Name = "ShowUserDescription";
            this.ShowUserDescription.Size = new System.Drawing.Size(111, 17);
            this.ShowUserDescription.TabIndex = 0;
            this.ShowUserDescription.Text = "چاپ توضیحات کاربر";
            this.ShowUserDescription.UseVisualStyleBackColor = true;
            this.ShowUserDescription.CheckedChanged += new System.EventHandler(this.ShowUserDescription_CheckedChanged);
            // 
            // ShowInvoiceDescription
            // 
            this.ShowInvoiceDescription.AutoSize = true;
            this.ShowInvoiceDescription.Location = new System.Drawing.Point(129, 20);
            this.ShowInvoiceDescription.Name = "ShowInvoiceDescription";
            this.ShowInvoiceDescription.Size = new System.Drawing.Size(117, 17);
            this.ShowInvoiceDescription.TabIndex = 0;
            this.ShowInvoiceDescription.Text = "چاپ توضیحات فاکتور";
            this.ShowInvoiceDescription.UseVisualStyleBackColor = true;
            this.ShowInvoiceDescription.CheckedChanged += new System.EventHandler(this.ShowInvoiceDescription_CheckedChanged);
            // 
            // ShowCustomerDescription
            // 
            this.ShowCustomerDescription.AutoSize = true;
            this.ShowCustomerDescription.Location = new System.Drawing.Point(252, 20);
            this.ShowCustomerDescription.Name = "ShowCustomerDescription";
            this.ShowCustomerDescription.Size = new System.Drawing.Size(128, 17);
            this.ShowCustomerDescription.TabIndex = 0;
            this.ShowCustomerDescription.Text = "چاپ توضیحات مشتری";
            this.ShowCustomerDescription.UseVisualStyleBackColor = true;
            this.ShowCustomerDescription.CheckedChanged += new System.EventHandler(this.ShowCustomerDescription_CheckedChanged);
            // 
            // ShowPhoneNumber
            // 
            this.ShowPhoneNumber.AutoSize = true;
            this.ShowPhoneNumber.Checked = true;
            this.ShowPhoneNumber.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowPhoneNumber.Location = new System.Drawing.Point(386, 20);
            this.ShowPhoneNumber.Name = "ShowPhoneNumber";
            this.ShowPhoneNumber.Size = new System.Drawing.Size(107, 17);
            this.ShowPhoneNumber.TabIndex = 0;
            this.ShowPhoneNumber.Text = "چاپ شماره تماس";
            this.ShowPhoneNumber.UseVisualStyleBackColor = true;
            this.ShowPhoneNumber.CheckedChanged += new System.EventHandler(this.ShowPhoneNumber_CheckedChanged);
            // 
            // ShowInvoiceId
            // 
            this.ShowInvoiceId.AutoSize = true;
            this.ShowInvoiceId.Checked = true;
            this.ShowInvoiceId.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowInvoiceId.Location = new System.Drawing.Point(603, 20);
            this.ShowInvoiceId.Name = "ShowInvoiceId";
            this.ShowInvoiceId.Size = new System.Drawing.Size(107, 17);
            this.ShowInvoiceId.TabIndex = 0;
            this.ShowInvoiceId.Text = "چاپ شماره فاکتور";
            this.ShowInvoiceId.UseVisualStyleBackColor = true;
            this.ShowInvoiceId.CheckedChanged += new System.EventHandler(this.ShowInvoiceId_CheckedChanged);
            // 
            // ShowInvoiceCreatedDate
            // 
            this.ShowInvoiceCreatedDate.AutoSize = true;
            this.ShowInvoiceCreatedDate.Checked = true;
            this.ShowInvoiceCreatedDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowInvoiceCreatedDate.Location = new System.Drawing.Point(499, 20);
            this.ShowInvoiceCreatedDate.Name = "ShowInvoiceCreatedDate";
            this.ShowInvoiceCreatedDate.Size = new System.Drawing.Size(98, 17);
            this.ShowInvoiceCreatedDate.TabIndex = 0;
            this.ShowInvoiceCreatedDate.Text = "چاپ تاریخ فاکتور";
            this.ShowInvoiceCreatedDate.UseVisualStyleBackColor = true;
            this.ShowInvoiceCreatedDate.CheckedChanged += new System.EventHandler(this.ShowInvoiceCreatedDate_CheckedChanged);
            // 
            // crystalReportViewer
            // 
            this.crystalReportViewer.ActiveViewIndex = -1;
            this.crystalReportViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer.Location = new System.Drawing.Point(0, 73);
            this.crystalReportViewer.Name = "crystalReportViewer";
            this.crystalReportViewer.Size = new System.Drawing.Size(726, 906);
            this.crystalReportViewer.TabIndex = 0;
            this.crystalReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "عمودی",
            "افقی"});
            this.comboBox1.Location = new System.Drawing.Point(148, 43);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(55, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(209, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "حالت چاپ";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "عمودی",
            "افقی"});
            this.comboBox2.Location = new System.Drawing.Point(12, 43);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(77, 21);
            this.comboBox2.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(95, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "توضیحات";
            // 
            // PrintInvoiceInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 979);
            this.Controls.Add(this.crystalReportViewer);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Name = "PrintInvoiceInterface";
            this.Text = "چاپ فاکتور";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrintInvoiceInterface_FormClosing);
            this.Load += new System.EventHandler(this.PrintInvoice_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer;
        private System.Windows.Forms.CheckBox ShowUserDescription;
        private System.Windows.Forms.CheckBox ShowInvoiceDescription;
        private System.Windows.Forms.CheckBox ShowCustomerDescription;
        private System.Windows.Forms.CheckBox ShowInvoiceCreatedDate;
        private System.Windows.Forms.CheckBox ShowPhoneNumber;
        private System.Windows.Forms.CheckBox ShowInvoiceId;
        private System.Windows.Forms.ComboBox cmbPageHeaderFontSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDetailsFontSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbPageFooterFontSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbDescriptionFontSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox2;
    }
}

