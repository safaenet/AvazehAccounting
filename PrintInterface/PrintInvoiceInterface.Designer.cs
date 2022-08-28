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
            this.ShowUserDescription = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceDescription = new System.Windows.Forms.CheckBox();
            this.ShowCustomerDescription = new System.Windows.Forms.CheckBox();
            this.ShowPhoneNumber = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceId = new System.Windows.Forms.CheckBox();
            this.ShowInvoiceCreatedDate = new System.Windows.Forms.CheckBox();
            this.crystalReportViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
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
            this.groupBox1.Size = new System.Drawing.Size(726, 43);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "تنظیمات چاپ";
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
            // 
            // ShowPhoneNumber
            // 
            this.ShowPhoneNumber.AutoSize = true;
            this.ShowPhoneNumber.Location = new System.Drawing.Point(386, 20);
            this.ShowPhoneNumber.Name = "ShowPhoneNumber";
            this.ShowPhoneNumber.Size = new System.Drawing.Size(107, 17);
            this.ShowPhoneNumber.TabIndex = 0;
            this.ShowPhoneNumber.Text = "چاپ شماره تماس";
            this.ShowPhoneNumber.UseVisualStyleBackColor = true;
            // 
            // ShowInvoiceId
            // 
            this.ShowInvoiceId.AutoSize = true;
            this.ShowInvoiceId.Location = new System.Drawing.Point(603, 20);
            this.ShowInvoiceId.Name = "ShowInvoiceId";
            this.ShowInvoiceId.Size = new System.Drawing.Size(107, 17);
            this.ShowInvoiceId.TabIndex = 0;
            this.ShowInvoiceId.Text = "چاپ شماره فاکتور";
            this.ShowInvoiceId.UseVisualStyleBackColor = true;
            // 
            // ShowInvoiceCreatedDate
            // 
            this.ShowInvoiceCreatedDate.AutoSize = true;
            this.ShowInvoiceCreatedDate.Location = new System.Drawing.Point(499, 20);
            this.ShowInvoiceCreatedDate.Name = "ShowInvoiceCreatedDate";
            this.ShowInvoiceCreatedDate.Size = new System.Drawing.Size(98, 17);
            this.ShowInvoiceCreatedDate.TabIndex = 0;
            this.ShowInvoiceCreatedDate.Text = "چاپ تاریخ فاکتور";
            this.ShowInvoiceCreatedDate.UseVisualStyleBackColor = true;
            // 
            // crystalReportViewer
            // 
            this.crystalReportViewer.ActiveViewIndex = -1;
            this.crystalReportViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer.Location = new System.Drawing.Point(0, 43);
            this.crystalReportViewer.Name = "crystalReportViewer";
            this.crystalReportViewer.Size = new System.Drawing.Size(726, 730);
            this.crystalReportViewer.TabIndex = 0;
            this.crystalReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // PrintInvoiceInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 773);
            this.Controls.Add(this.crystalReportViewer);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.MaximizeBox = false;
            this.Name = "PrintInvoiceInterface";
            this.Text = "چاپ فاکتور";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PrintInvoiceInterface_FormClosed);
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
    }
}

