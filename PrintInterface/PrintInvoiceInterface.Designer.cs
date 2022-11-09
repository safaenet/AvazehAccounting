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
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.txtCustomerPostAddress = new System.Windows.Forms.TextBox();
            this.txtCustomerDescription = new System.Windows.Forms.TextBox();
            this.txtInvoiceDescription = new System.Windows.Forms.TextBox();
            this.txtUserDescription = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbUserDescriptions = new System.Windows.Forms.ComboBox();
            this.cmbPageSize = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbPrintLayout = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPageFooterFontSize = new System.Windows.Forms.ComboBox();
            this.cmbDescriptionFontSize = new System.Windows.Forms.ComboBox();
            this.cmbDetailsFontSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPageHeaderFontSize = new System.Windows.Forms.ComboBox();
            this.ShowUserDescription = new System.Windows.Forms.CheckBox();
            this.ShowCustomerPostAddress = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceDescription = new System.Windows.Forms.CheckBox();
            this.ShowCustomerDescription = new System.Windows.Forms.CheckBox();
            this.ShowPhoneNumber = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceId = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceCreatedDate = new System.Windows.Forms.CheckBox();
            this.crystalReportViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.btnShowSettings = new System.Windows.Forms.Button();
            this.gbSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSettings
            // 
            this.gbSettings.Controls.Add(this.txtCustomerPostAddress);
            this.gbSettings.Controls.Add(this.txtCustomerDescription);
            this.gbSettings.Controls.Add(this.txtInvoiceDescription);
            this.gbSettings.Controls.Add(this.txtUserDescription);
            this.gbSettings.Controls.Add(this.label6);
            this.gbSettings.Controls.Add(this.label7);
            this.gbSettings.Controls.Add(this.label5);
            this.gbSettings.Controls.Add(this.label3);
            this.gbSettings.Controls.Add(this.cmbUserDescriptions);
            this.gbSettings.Controls.Add(this.cmbPageSize);
            this.gbSettings.Controls.Add(this.label4);
            this.gbSettings.Controls.Add(this.cmbPrintLayout);
            this.gbSettings.Controls.Add(this.label2);
            this.gbSettings.Controls.Add(this.cmbPageFooterFontSize);
            this.gbSettings.Controls.Add(this.cmbDescriptionFontSize);
            this.gbSettings.Controls.Add(this.cmbDetailsFontSize);
            this.gbSettings.Controls.Add(this.label1);
            this.gbSettings.Controls.Add(this.cmbPageHeaderFontSize);
            this.gbSettings.Controls.Add(this.ShowUserDescription);
            this.gbSettings.Controls.Add(this.ShowCustomerPostAddress);
            this.gbSettings.Controls.Add(this.ShowInvoiceDescription);
            this.gbSettings.Controls.Add(this.ShowCustomerDescription);
            this.gbSettings.Controls.Add(this.ShowPhoneNumber);
            this.gbSettings.Controls.Add(this.ShowInvoiceId);
            this.gbSettings.Controls.Add(this.ShowInvoiceCreatedDate);
            this.gbSettings.Dock = System.Windows.Forms.DockStyle.Left;
            this.gbSettings.Location = new System.Drawing.Point(0, 0);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.gbSettings.Size = new System.Drawing.Size(267, 789);
            this.gbSettings.TabIndex = 1;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "تنظیمات چاپ";
            this.gbSettings.Visible = false;
            // 
            // txtCustomerPostAddress
            // 
            this.txtCustomerPostAddress.AcceptsReturn = true;
            this.txtCustomerPostAddress.Location = new System.Drawing.Point(13, 89);
            this.txtCustomerPostAddress.Multiline = true;
            this.txtCustomerPostAddress.Name = "txtCustomerPostAddress";
            this.txtCustomerPostAddress.ReadOnly = true;
            this.txtCustomerPostAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCustomerPostAddress.Size = new System.Drawing.Size(248, 53);
            this.txtCustomerPostAddress.TabIndex = 3;
            this.txtCustomerPostAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCustomerPostAddress.WordWrap = false;
            this.txtCustomerPostAddress.TextChanged += new System.EventHandler(this.txtUserDescription_TextChanged);
            // 
            // txtCustomerDescription
            // 
            this.txtCustomerDescription.AcceptsReturn = true;
            this.txtCustomerDescription.Location = new System.Drawing.Point(13, 170);
            this.txtCustomerDescription.Multiline = true;
            this.txtCustomerDescription.Name = "txtCustomerDescription";
            this.txtCustomerDescription.ReadOnly = true;
            this.txtCustomerDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCustomerDescription.Size = new System.Drawing.Size(248, 53);
            this.txtCustomerDescription.TabIndex = 3;
            this.txtCustomerDescription.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCustomerDescription.WordWrap = false;
            this.txtCustomerDescription.TextChanged += new System.EventHandler(this.txtUserDescription_TextChanged);
            // 
            // txtInvoiceDescription
            // 
            this.txtInvoiceDescription.AcceptsReturn = true;
            this.txtInvoiceDescription.Location = new System.Drawing.Point(12, 251);
            this.txtInvoiceDescription.Multiline = true;
            this.txtInvoiceDescription.Name = "txtInvoiceDescription";
            this.txtInvoiceDescription.ReadOnly = true;
            this.txtInvoiceDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInvoiceDescription.Size = new System.Drawing.Size(248, 53);
            this.txtInvoiceDescription.TabIndex = 3;
            this.txtInvoiceDescription.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInvoiceDescription.WordWrap = false;
            this.txtInvoiceDescription.TextChanged += new System.EventHandler(this.txtUserDescription_TextChanged);
            // 
            // txtUserDescription
            // 
            this.txtUserDescription.AcceptsReturn = true;
            this.txtUserDescription.Location = new System.Drawing.Point(12, 360);
            this.txtUserDescription.Multiline = true;
            this.txtUserDescription.Name = "txtUserDescription";
            this.txtUserDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUserDescription.Size = new System.Drawing.Size(248, 126);
            this.txtUserDescription.TabIndex = 3;
            this.txtUserDescription.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtUserDescription.WordWrap = false;
            this.txtUserDescription.TextChanged += new System.EventHandler(this.txtUserDescription_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(214, 336);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "توضیحات";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(59, 549);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "سایز کاغذ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(210, 549);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "حالت چاپ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 522);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "سایز انتها";
            // 
            // cmbUserDescriptions
            // 
            this.cmbUserDescriptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserDescriptions.FormattingEnabled = true;
            this.cmbUserDescriptions.Location = new System.Drawing.Point(12, 333);
            this.cmbUserDescriptions.Name = "cmbUserDescriptions";
            this.cmbUserDescriptions.Size = new System.Drawing.Size(196, 21);
            this.cmbUserDescriptions.TabIndex = 1;
            this.cmbUserDescriptions.SelectedIndexChanged += new System.EventHandler(this.cmbUserDescriptions_SelectedIndexChanged);
            // 
            // cmbPageSize
            // 
            this.cmbPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPageSize.FormattingEnabled = true;
            this.cmbPageSize.Items.AddRange(new object[] {
            "A5",
            "A4"});
            this.cmbPageSize.Location = new System.Drawing.Point(12, 546);
            this.cmbPageSize.Name = "cmbPageSize";
            this.cmbPageSize.Size = new System.Drawing.Size(41, 21);
            this.cmbPageSize.TabIndex = 1;
            this.cmbPageSize.SelectedIndexChanged += new System.EventHandler(this.cmbPageSize_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(190, 522);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "سایز توضیحات";
            // 
            // cmbPrintLayout
            // 
            this.cmbPrintLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintLayout.FormattingEnabled = true;
            this.cmbPrintLayout.Items.AddRange(new object[] {
            "عمودی",
            "افقی"});
            this.cmbPrintLayout.Location = new System.Drawing.Point(149, 546);
            this.cmbPrintLayout.Name = "cmbPrintLayout";
            this.cmbPrintLayout.Size = new System.Drawing.Size(60, 21);
            this.cmbPrintLayout.TabIndex = 1;
            this.cmbPrintLayout.SelectedIndexChanged += new System.EventHandler(this.cmbPrintLayout_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 495);
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
            this.cmbPageFooterFontSize.Location = new System.Drawing.Point(12, 519);
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
            this.cmbDescriptionFontSize.Location = new System.Drawing.Point(149, 519);
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
            this.cmbDetailsFontSize.Location = new System.Drawing.Point(12, 492);
            this.cmbDetailsFontSize.Name = "cmbDetailsFontSize";
            this.cmbDetailsFontSize.Size = new System.Drawing.Size(35, 21);
            this.cmbDetailsFontSize.TabIndex = 1;
            this.cmbDetailsFontSize.SelectedIndexChanged += new System.EventHandler(this.cmbDetailsFontSize_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(186, 495);
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
            this.cmbPageHeaderFontSize.Location = new System.Drawing.Point(149, 492);
            this.cmbPageHeaderFontSize.Name = "cmbPageHeaderFontSize";
            this.cmbPageHeaderFontSize.Size = new System.Drawing.Size(35, 21);
            this.cmbPageHeaderFontSize.TabIndex = 1;
            this.cmbPageHeaderFontSize.SelectedIndexChanged += new System.EventHandler(this.cmbPageHeaderFontSize_SelectedIndexChanged);
            // 
            // ShowUserDescription
            // 
            this.ShowUserDescription.AutoSize = true;
            this.ShowUserDescription.Location = new System.Drawing.Point(148, 310);
            this.ShowUserDescription.Name = "ShowUserDescription";
            this.ShowUserDescription.Size = new System.Drawing.Size(111, 17);
            this.ShowUserDescription.TabIndex = 0;
            this.ShowUserDescription.Text = "چاپ توضیحات کاربر";
            this.ShowUserDescription.UseVisualStyleBackColor = true;
            this.ShowUserDescription.CheckedChanged += new System.EventHandler(this.ShowUserDescription_CheckedChanged);
            // 
            // ShowCustomerPostAddress
            // 
            this.ShowCustomerPostAddress.AutoSize = true;
            this.ShowCustomerPostAddress.Location = new System.Drawing.Point(147, 66);
            this.ShowCustomerPostAddress.Name = "ShowCustomerPostAddress";
            this.ShowCustomerPostAddress.Size = new System.Drawing.Size(113, 17);
            this.ShowCustomerPostAddress.TabIndex = 0;
            this.ShowCustomerPostAddress.Text = "چاپ آدرس مشتری";
            this.ShowCustomerPostAddress.UseVisualStyleBackColor = true;
            this.ShowCustomerPostAddress.CheckedChanged += new System.EventHandler(this.ShowCustomerPostalAddress_CheckedChanged);
            // 
            // ShowInvoiceDescription
            // 
            this.ShowInvoiceDescription.AutoSize = true;
            this.ShowInvoiceDescription.Location = new System.Drawing.Point(143, 228);
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
            this.ShowCustomerDescription.Location = new System.Drawing.Point(132, 147);
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
            this.ShowPhoneNumber.Location = new System.Drawing.Point(153, 43);
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
            this.ShowInvoiceId.Location = new System.Drawing.Point(153, 20);
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
            this.ShowInvoiceCreatedDate.Location = new System.Drawing.Point(12, 20);
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
            this.crystalReportViewer.Location = new System.Drawing.Point(287, 0);
            this.crystalReportViewer.Name = "crystalReportViewer";
            this.crystalReportViewer.Size = new System.Drawing.Size(647, 789);
            this.crystalReportViewer.TabIndex = 0;
            this.crystalReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // btnShowSettings
            // 
            this.btnShowSettings.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnShowSettings.Location = new System.Drawing.Point(267, 0);
            this.btnShowSettings.Name = "btnShowSettings";
            this.btnShowSettings.Size = new System.Drawing.Size(20, 789);
            this.btnShowSettings.TabIndex = 2;
            this.btnShowSettings.Text = "<";
            this.btnShowSettings.UseVisualStyleBackColor = true;
            this.btnShowSettings.Click += new System.EventHandler(this.btnShowSettings_Click);
            // 
            // PrintInvoiceInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 789);
            this.Controls.Add(this.crystalReportViewer);
            this.Controls.Add(this.btnShowSettings);
            this.Controls.Add(this.gbSettings);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Name = "PrintInvoiceInterface";
            this.Text = "چاپ فاکتور";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrintInvoiceInterface_FormClosing);
            this.Load += new System.EventHandler(this.PrintInvoice_Load);
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSettings;
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
        private System.Windows.Forms.ComboBox cmbPrintLayout;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbUserDescriptions;
        private System.Windows.Forms.TextBox txtUserDescription;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbPageSize;
        private System.Windows.Forms.Button btnShowSettings;
        private System.Windows.Forms.TextBox txtCustomerDescription;
        private System.Windows.Forms.TextBox txtInvoiceDescription;
        private System.Windows.Forms.TextBox txtCustomerPostAddress;
        private System.Windows.Forms.CheckBox ShowCustomerPostAddress;
    }
}

