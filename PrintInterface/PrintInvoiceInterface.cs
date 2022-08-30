using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace PrintInterface
{
    public partial class PrintInvoiceInterface : Form
    {
        public PrintInvoiceInterface()
        {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs();
            FilePath = args[2]; //Report file path
        }

        PrintInvoiceModel pim = new PrintInvoiceModel();
        ReportDocument rd;
        string FilePath;
        bool CanRefresh = false;

        private void PrintInvoice_Load(object sender, EventArgs e)
        {
            if (!File.Exists(FilePath))
            {
                MessageBox.Show("فایل فاکتور یافت نشد", "خطای پارامتر ورودی", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            var xmlSerializer = new XmlSerializer(pim.GetType());
            string xmlString = File.ReadAllText(FilePath);
            StringReader stringReader = new StringReader(xmlString);
            pim = xmlSerializer.Deserialize(stringReader) as PrintInvoiceModel;
            File.Delete(FilePath);

            if (pim.UserDescriptions != null && pim.UserDescriptions.Count > 0)
                cmbUserDescriptions.DataSource = pim.UserDescriptions.Select(i => i.DescriptionText).ToList();

            cmbPageHeaderFontSize.Text = "10";
            cmbDetailsFontSize.Text = "10";
            cmbPageFooterFontSize.Text = "10";
            cmbDescriptionFontSize.Text = "14";
            cmbPrintLayout.Text = "عمودی";
            cmbPageSize.Text = "A5";
            if (string.IsNullOrEmpty(pim.CustomerPhoneNumber)) ShowPhoneNumber.Enabled = ShowPhoneNumber.Checked = false;
            if (string.IsNullOrEmpty(pim.CustomerDescription)) ShowCustomerDescription.Enabled = ShowCustomerDescription.Checked = false;
            if (string.IsNullOrEmpty(pim.InvoiceDescription)) ShowInvoiceDescription.Enabled = ShowInvoiceDescription.Checked = false;
            if (pim.UserDescriptions == null || pim.UserDescriptions.Count == 0) ShowUserDescription.Enabled = ShowUserDescription.Checked = false;
            if (string.IsNullOrEmpty(pim.CustomerPhoneNumber) && string.IsNullOrEmpty(pim.InvoiceDescription) && (pim.UserDescriptions == null || pim.UserDescriptions.Count == 0)) cmbDescriptionFontSize.Enabled = false;
            
            CanRefresh = true;

            InitilizeReport();
        }

        private void InitilizeReport()
        {
            if (cmbPageSize.Text == "A5")
            {
                if (cmbPrintLayout.Text == "عمودی") rd = new PrintInvoicePortraitA5();
                else rd = new PrintInvoiceLandscapeA5();
            }
            else if (cmbPageSize.Text == "A4") rd = new PrintInvoicePortraitA4();


            rd.SetDataSource(pim.Products);
            rd.SetParameterValue("InvoiceId", pim.InvoiceId);
            rd.SetParameterValue("InvoiceDateCreated", pim.InvoiceDateCreated);
            rd.SetParameterValue("CustomerFullName", pim.CustomerFullName);
            rd.SetParameterValue("CustomerPhoneNumber", pim.CustomerPhoneNumber);
            rd.SetParameterValue("TotalItemsSellSum", pim.TotalItemsSellSum);
            rd.SetParameterValue("TotalDiscountAmount", pim.TotalDiscountAmount);
            rd.SetParameterValue("TotalInvoiceSum", pim.TotalInvoiceSum);
            rd.SetParameterValue("TotalPayments", pim.TotalPayments);
            rd.SetParameterValue("TotalBalance", pim.TotalBalance);
            rd.SetParameterValue("FooterTextRight", pim.FooterTextRight);
            rd.SetParameterValue("FooterTextLeft", pim.FooterTextLeft);
            rd.SetParameterValue("CustomerPreviousBalance", pim.CustomerPreviousBalance);
            rd.SetParameterValue("InvoiceFinStatus", pim.InvoiceFinStatus);
            rd.SetParameterValue("InvoiceType", pim.InvoiceType);
            rd.SetParameterValue("ShowInvoiceId", pim.PrintInvoiceId);
            rd.SetParameterValue("ShowInvoiceCreatedDate", pim.PrintDate);
            rd.SetParameterValue("ShowCustomerPhoneNumber", pim.PrintCustomerPhoneNumber);
            rd.SetParameterValue("ShowCustomerDescription", pim.PrintCustomerDescription);
            rd.SetParameterValue("ShowInvoiceDescription", pim.PrintInvoiceDescription);
            rd.SetParameterValue("ShowUserDescription", pim.PrintUserDescription);
            rd.SetParameterValue("PageHeaderFontSize", pim.PageHeaderFontSize);
            rd.SetParameterValue("DetailsFontSize", pim.DetailsFontSize);
            rd.SetParameterValue("PageFooterFontSize", pim.PageFooterFontSize);
            rd.SetParameterValue("DescriptionFontSize", pim.DescriptionFontSize);
            rd.SetParameterValue("LeftHeaderImage", pim.LeftImagePath);
            rd.SetParameterValue("RightHeaderImage", pim.RightImagePath);
            rd.SetParameterValue("MainHeaderText", pim.MainHeaderText);
            rd.SetParameterValue("HeaderDescription1", pim.HeaderDescription1);
            rd.SetParameterValue("HeaderDescription2", pim.HeaderDescription2);
            rd.SetParameterValue("CustomerDescription", pim.CustomerDescription);
            rd.SetParameterValue("InvoiceDescription", pim.InvoiceDescription);
            rd.SetParameterValue("MainHeaderTextFontSize", pim.MainHeaderTextFontSize);
            rd.SetParameterValue("HeaderDescriptionFontSize", pim.HeaderDescriptionFontSize);
            rd.SetParameterValue("InvoiceTypeTextFontSize", pim.InvoiceTypeTextFontSize);
            rd.SetParameterValue("UserDescription", "");
            crystalReportViewer.ReportSource = rd;
        }

        private void RefreshCrystalReport()
        {
            if (!CanRefresh) return;
            crystalReportViewer.ReportSource = rd;
            crystalReportViewer.Refresh();
        }

        private void ShowInvoiceId_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("ShowInvoiceId", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowInvoiceCreatedDate_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("ShowInvoiceCreatedDate", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowPhoneNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("ShowCustomerPhoneNumber", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowCustomerDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("ShowCustomerDescription", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowInvoiceDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("ShowInvoiceDescription", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowUserDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("ShowUserDescription", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void cmbPageHeaderFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("PageHeaderFontSize", int.Parse((sender as ComboBox).Text));
            RefreshCrystalReport();
        }

        private void cmbDetailsFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("DetailsFontSize", int.Parse((sender as ComboBox).Text));
            RefreshCrystalReport();
        }

        private void cmbPageFooterFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("PageFooterFontSize", int.Parse((sender as ComboBox).Text));
            RefreshCrystalReport();
        }

        private void cmbDescriptionFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("DescriptionFontSize", int.Parse((sender as ComboBox).Text));
            RefreshCrystalReport();
        }

        private void PrintInvoiceInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void cmbUserDescriptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUserDescription.Text = cmbUserDescriptions.Text;
        }

        private void txtUserDescription_TextChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            rd.SetParameterValue("UserDescription", txtUserDescription.Text);
            if (!ShowUserDescription.Checked) return;
            RefreshCrystalReport();
        }

        private void cmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPageSize.Text == "A4")
            {
                cmbPrintLayout.Text = "عمودی";
                cmbPrintLayout.Enabled = false;
            }
            else cmbPrintLayout.Enabled = true;
            if (!CanRefresh) return;
            InitilizeReport();
            RefreshCrystalReport();
        }

        private void cmbPrintLayout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            InitilizeReport();
            RefreshCrystalReport();
        }
    }
}