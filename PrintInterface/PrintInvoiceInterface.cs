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
            //var xmlSerializer = new XmlSerializer(pim.GetType());
            //string xmlString = File.ReadAllText(@"D:\Users\avazeh1\Downloads\AvazehAccountingClone\AvazehWpf\bin\Debug\net5.0-windows\Temp\637980039153641537.xml");
            //StringReader stringReader = new StringReader(xmlString);
            //pim = xmlSerializer.Deserialize(stringReader) as PrintInvoiceModel;
            if (!File.Exists(FilePath))
            {
                MessageBox.Show("فایل فاکتور یافت نشد\n" + FilePath, "خطای پارامتر ورودی", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            var xmlSerializer = new XmlSerializer(pim.GetType());
            string xmlString = File.ReadAllText(FilePath);
            StringReader stringReader = new StringReader(xmlString);
            pim = xmlSerializer.Deserialize(stringReader) as PrintInvoiceModel;
            File.Delete(FilePath);

            if (pim.PrintSettings.UserDescriptions != null && pim.PrintSettings.UserDescriptions.Count > 0)
            {
                cmbUserDescriptions.DataSource = pim.PrintSettings.UserDescriptions;
                cmbUserDescriptions.DisplayMember = nameof(UserDescriptionModel.DescriptionTitle);
                cmbUserDescriptions.ValueMember = nameof(UserDescriptionModel.DescriptionText);
            }

            ShowInvoiceId.Checked = pim.PrintInvoiceId;
            ShowInvoiceCreatedDate.Checked = pim.PrintDate;
            ShowPhoneNumber.Checked = pim.PrintCustomerPhoneNumber;
            ShowCustomerPostAddress.Checked = pim.PrintCustomerPostAddress;
            ShowCustomerDescription.Checked = pim.PrintCustomerDescription;
            ShowInvoiceDescription.Checked = pim.PrintInvoiceDescription;
            ShowUserDescription.Checked = pim.PrintUserDescription;

            cmbPageHeaderFontSize.Text = pim.PrintSettings.PageHeaderFontSize.ToString();
            cmbDetailsFontSize.Text = pim.PrintSettings.DetailsFontSize.ToString();
            cmbPageFooterFontSize.Text = pim.PrintSettings.PageFooterFontSize.ToString();
            cmbDescriptionFontSize.Text = pim.PrintSettings.DescriptionFontSize.ToString();
            cmbPrintLayout.Text = pim.PrintSettings.DefaultPrintLayout;
            cmbPageSize.Text = pim.PrintSettings.DefaultPaperSize;

            txtCustomerDescription.Text = pim.CustomerDescription;
            txtInvoiceDescription.Text = pim.InvoiceDescription;
            txtCustomerPostAddress.Text = pim.CustomerPostAddress;

            if (string.IsNullOrEmpty(pim.CustomerPhoneNumber)) ShowPhoneNumber.Enabled = ShowPhoneNumber.Checked = false;
            //if (string.IsNullOrEmpty(pim.CustomerDescription)) ShowCustomerDescription.Enabled = txtCustomerDescription.Enabled = ShowCustomerDescription.Checked = false;
            //if (string.IsNullOrEmpty(pim.InvoiceDescription)) ShowInvoiceDescription.Enabled = txtInvoiceDescription.Enabled = ShowInvoiceDescription.Checked = false;
            if (string.IsNullOrEmpty(pim.CustomerPhoneNumber) && string.IsNullOrEmpty(pim.InvoiceDescription) && (pim.PrintSettings.UserDescriptions == null || pim.PrintSettings.UserDescriptions.Count == 0)) cmbDescriptionFontSize.Enabled = false;
            //if (string.IsNullOrEmpty(pim.CustomerPostAddress)) txtCustomerPostAddress.Enabled = ShowCustomerPostAddress.Enabled = false;

            CanRefresh = true;
            InitilizeReport();
            if (cmbUserDescriptions.Items != null && cmbUserDescriptions.Items.Count > 0) txtUserDescription.Text = (string)cmbUserDescriptions.SelectedValue;
        }

        private void InitilizeReport()
        {
            if (pim.PrintSettings.DefaultPaperSize == "A5")
            {
                if (pim.PrintSettings.DefaultPrintLayout == "عمودی") rd = new PrintInvoicePortraitA5();
                else rd = new PrintInvoiceLandscapeA5();
            }
            else if (pim.PrintSettings.DefaultPaperSize == "A4") rd = new PrintInvoicePortraitA4();


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
            rd.SetParameterValue("FooterTextRight", pim.PrintSettings.FooterTextRight);
            rd.SetParameterValue("FooterTextLeft", pim.PrintSettings.FooterTextLeft);
            rd.SetParameterValue("CustomerPreviousBalance", pim.CustomerPreviousBalance);
            rd.SetParameterValue("InvoiceFinStatus", pim.InvoiceFinStatus);
            rd.SetParameterValue("InvoiceType", pim.InvoiceType);
            rd.SetParameterValue("ShowInvoiceId", pim.PrintInvoiceId);
            rd.SetParameterValue("ShowInvoiceCreatedDate", pim.PrintDate);
            rd.SetParameterValue("ShowCustomerPhoneNumber", pim.PrintCustomerPhoneNumber);
            rd.SetParameterValue("ShowCustomerPostAddress", pim.PrintCustomerPostAddress);
            rd.SetParameterValue("ShowCustomerDescription", pim.PrintCustomerDescription);
            rd.SetParameterValue("ShowInvoiceDescription", pim.PrintInvoiceDescription);
            rd.SetParameterValue("ShowUserDescription", pim.PrintUserDescription);
            rd.SetParameterValue("PageHeaderFontSize", pim.PrintSettings.PageHeaderFontSize);
            rd.SetParameterValue("DetailsFontSize", pim.PrintSettings.DetailsFontSize);
            rd.SetParameterValue("PageFooterFontSize", pim.PrintSettings.PageFooterFontSize);
            rd.SetParameterValue("DescriptionFontSize", pim.PrintSettings.DescriptionFontSize);
            rd.SetParameterValue("LeftHeaderImage", Application.StartupPath + pim.PrintSettings.LeftHeaderImage);
            rd.SetParameterValue("RightHeaderImage", Application.StartupPath + pim.PrintSettings.RightHeaderImage);
            rd.SetParameterValue("MainHeaderText", pim.PrintSettings.MainHeaderText);
            rd.SetParameterValue("HeaderDescription1", pim.PrintSettings.HeaderDescription1);
            rd.SetParameterValue("HeaderDescription2", pim.PrintSettings.HeaderDescription2);
            rd.SetParameterValue("CustomerDescription", pim.CustomerDescription);
            rd.SetParameterValue("InvoiceDescription", pim.InvoiceDescription);
            SetFontSizeValuesBasedOnPaper();
            rd.SetParameterValue("CustomerPostAddress", pim.CustomerPostAddress);
            rd.SetParameterValue("UserDescription", "");
            crystalReportViewer.ReportSource = rd;
        }

        private void SetFontSizeValuesBasedOnPaper()
        {
            if (pim.PrintSettings.DefaultPaperSize == "A4")
            {
                rd.SetParameterValue("MainHeaderTextFontSize", pim.PrintSettings.MainHeaderTextFontSizeA4P);
                rd.SetParameterValue("HeaderDescriptionFontSize", pim.PrintSettings.HeaderDescriptionFontSizeA4P);
                rd.SetParameterValue("InvoiceTypeTextFontSize", pim.PrintSettings.TypeTextFontSizeA4P);
            }
            else if (pim.PrintSettings.DefaultPaperSize == "A5")
                if (pim.PrintSettings.DefaultPrintLayout == "عمودی")
                {
                    rd.SetParameterValue("MainHeaderTextFontSize", pim.PrintSettings.MainHeaderTextFontSizeA5P);
                    rd.SetParameterValue("HeaderDescriptionFontSize", pim.PrintSettings.HeaderDescriptionFontSizeA5P);
                    rd.SetParameterValue("InvoiceTypeTextFontSize", pim.PrintSettings.TypeTextFontSizeA5P);
                }
                else if (pim.PrintSettings.DefaultPrintLayout == "افقی")
                {
                    rd.SetParameterValue("MainHeaderTextFontSize", pim.PrintSettings.MainHeaderTextFontSizeA5L);
                    rd.SetParameterValue("HeaderDescriptionFontSize", pim.PrintSettings.HeaderDescriptionFontSizeA5L);
                    rd.SetParameterValue("InvoiceTypeTextFontSize", pim.PrintSettings.TypeTextFontSizeA5L);
                }
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
            pim.PrintInvoiceId = (sender as CheckBox).Checked;
            rd.SetParameterValue("ShowInvoiceId", pim.PrintInvoiceId);
            RefreshCrystalReport();
        }

        private void ShowInvoiceCreatedDate_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintDate = (sender as CheckBox).Checked;
            rd.SetParameterValue("ShowInvoiceCreatedDate", pim.PrintDate);
            RefreshCrystalReport();
        }

        private void ShowPhoneNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintCustomerPhoneNumber = (sender as CheckBox).Checked;
            rd.SetParameterValue("ShowCustomerPhoneNumber", pim.PrintCustomerPhoneNumber);
            RefreshCrystalReport();
        }

        private void ShowCustomerDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintCustomerDescription = (sender as CheckBox).Checked;
            rd.SetParameterValue("ShowCustomerDescription", pim.PrintCustomerDescription);
            RefreshCrystalReport();
        }

        private void ShowInvoiceDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintInvoiceDescription = (sender as CheckBox).Checked;
            rd.SetParameterValue("ShowInvoiceDescription", pim.PrintInvoiceDescription);
            RefreshCrystalReport();
        }

        private void ShowUserDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintUserDescription = (sender as CheckBox).Checked;
            rd.SetParameterValue("ShowUserDescription", pim.PrintUserDescription);
            rd.SetParameterValue("UserDescription", txtUserDescription.Text);
            RefreshCrystalReport();
        }

        private void cmbPageHeaderFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintSettings.PageHeaderFontSize = int.Parse((sender as ComboBox).Text, CultureInfo.InvariantCulture);
            rd.SetParameterValue("PageHeaderFontSize", pim.PrintSettings.PageHeaderFontSize);
            RefreshCrystalReport();
        }

        private void cmbDetailsFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintSettings.DetailsFontSize = int.Parse((sender as ComboBox).Text, CultureInfo.InvariantCulture);
            rd.SetParameterValue("DetailsFontSize", pim.PrintSettings.DetailsFontSize);
            RefreshCrystalReport();
        }

        private void cmbPageFooterFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintSettings.PageFooterFontSize = int.Parse((sender as ComboBox).Text, CultureInfo.InvariantCulture);
            rd.SetParameterValue("PageFooterFontSize", pim.PrintSettings.PageFooterFontSize);
            RefreshCrystalReport();
        }

        private void cmbDescriptionFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintSettings.DescriptionFontSize = int.Parse((sender as ComboBox).Text, CultureInfo.InvariantCulture);
            rd.SetParameterValue("DescriptionFontSize", pim.PrintSettings.DescriptionFontSize);
            RefreshCrystalReport();
        }

        private void PrintInvoiceInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void cmbUserDescriptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUserDescription.Text = (string)cmbUserDescriptions.SelectedValue;
        }

        private void txtUserDescription_TextChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            if (!ShowUserDescription.Checked) return;
            rd.SetParameterValue("UserDescription", txtUserDescription.Text);
            //RefreshCrystalReport();
        }

        private void cmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            pim.PrintSettings.DefaultPaperSize = (sender as ComboBox).Text;
            if (pim.PrintSettings.DefaultPaperSize == "A4")
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
            pim.PrintSettings.DefaultPrintLayout = (sender as ComboBox).Text;
            if (!CanRefresh) return;
            InitilizeReport();
            RefreshCrystalReport();
        }

        private void btnShowSettings_Click(object sender, EventArgs e)
        {
            gbSettings.Visible= !gbSettings.Visible;
            if (gbSettings.Visible)
            {
                (sender as Button).Text = ">";
            }
            else (sender as Button).Text = "<";
        }

        private void ShowCustomerPostalAddress_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            pim.PrintCustomerPostAddress = (sender as CheckBox).Checked;
            rd.SetParameterValue("ShowCustomerPostAddress", pim.PrintCustomerPostAddress);
            RefreshCrystalReport();
        }

        private void btnUpdateReport_Click(object sender, EventArgs e)
        {
            RefreshCrystalReport();
        }

        private void txtCustomerPostAddress_TextChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            if (!ShowCustomerPostAddress.Checked) return;
            rd.SetParameterValue("CustomerPostAddress", txtCustomerPostAddress.Text);
        }

        private void txtCustomerDescription_TextChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            if (!ShowCustomerDescription.Checked) return;
            rd.SetParameterValue("CustomerDescription", txtCustomerDescription.Text);
        }

        private void txtInvoiceDescription_TextChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            if (!ShowInvoiceDescription.Checked) return;
            rd.SetParameterValue("InvoiceDescription", txtInvoiceDescription.Text);
        }
    }
}