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
        }

        PrintInvoiceModel pim = new PrintInvoiceModel();
        PrintInvoice pi = new PrintInvoice();

        private void PrintInvoice_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args == null || args.Length < 2 || !File.Exists(args[1]))
            {
                MessageBox.Show("پارامترهای لازم وارد نشده اند یا به درستی وارد نشده اند", "خطای پارامتر ورودی", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            var FilePath = args[1];
            var xmlSerializer = new XmlSerializer(pim.GetType());
            string xmlString = File.ReadAllText(FilePath);
            StringReader stringReader = new StringReader(xmlString);
            pim = xmlSerializer.Deserialize(stringReader) as PrintInvoiceModel;

            //var file = @"D:\Users\avazeh1\Downloads\AvazehAccountingClone\AvazehWpf\bin\Debug\net5.0-windows\Temp\637973019071157097.xml";
            //var xmlSerializer = new XmlSerializer(pim.GetType());
            //string xmlString = File.ReadAllText(file);
            //StringReader stringReader = new StringReader(xmlString);
            //pim = xmlSerializer.Deserialize(stringReader) as PrintInvoiceModel;

            pi.SetDataSource(pim.Products);
            pi.SetParameterValue("InvoiceId", pim.InvoiceId);
            pi.SetParameterValue("InvoiceDateCreated", pim.InvoiceDateCreated);
            pi.SetParameterValue("CustomerFullName", pim.CustomerFullName);
            pi.SetParameterValue("CustomerPhoneNumber", pim.CustomerPhoneNumber);
            pi.SetParameterValue("TotalItemsSellSum", string.Format(CultureInfo.InvariantCulture, "{0:0,0}", pim.TotalItemsSellSum));
            pi.SetParameterValue("TotalDiscountAmount", string.Format(CultureInfo.InvariantCulture, "{0:0,0}", pim.TotalDiscountAmount));
            pi.SetParameterValue("TotalInvoiceSum", string.Format(CultureInfo.InvariantCulture, "{0:0,0}", pim.TotalInvoiceSum));
            pi.SetParameterValue("TotalPayments", string.Format(CultureInfo.InvariantCulture, "{0:0,0}", pim.TotalPayments));
            pi.SetParameterValue("TotalBalance", string.Format(CultureInfo.InvariantCulture, "{0:0,0}", pim.TotalBalance));
            pi.SetParameterValue("FooterTextRight", pim.FooterTextRight);
            pi.SetParameterValue("FooterTextLeft", pim.FooterTextLeft);
            pi.SetParameterValue("CustomerPreviousBalance", pim.CustomerPreviousBalance);
            pi.SetParameterValue("InvoiceFinStatus", pim.InvoiceFinStatus);
            pi.SetParameterValue("InvoiceType", pim.InvoiceType);
            pi.SetParameterValue("ShowInvoiceId", true);
            pi.SetParameterValue("ShowInvoiceCreatedDate", true);
            pi.SetParameterValue("ShowCustomerPhoneNumber", true);
            pi.SetParameterValue("ShowCustomerDescription", false);
            pi.SetParameterValue("ShowInvoiceDescription", false);
            pi.SetParameterValue("ShowUserDescription", false);
            pi.SetParameterValue("PageHeaderFontSize", 10);
            pi.SetParameterValue("DetailsFontSize", 10);
            pi.SetParameterValue("PageFooterFontSize", 10);
            pi.SetParameterValue("LeftHeaderImage", Application.StartupPath + @"\Images\LeftImage.png");
            pi.SetParameterValue("RightHeaderImage", Application.StartupPath + @"\Images\RightImage.png");
            pi.SetParameterValue("MainHeaderText", "فروشگاه آوازه");
            pi.SetParameterValue("HeaderDescription1", "دوربین مداربسته، کرکره برقی، جک پارکینگی");
            pi.SetParameterValue("HeaderDescription2", "01734430827");
            crystalReportViewer.ReportSource = pi;

            cmbPageHeaderFontSize.Text = "10";
            cmbDetailsFontSize.Text = "10";
            cmbPageFooterFontSize.Text = "10";
        }

        private void PrintInvoiceInterface_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void RefreshCrystalReport()
        {
            crystalReportViewer.ReportSource = pi;
            crystalReportViewer.Refresh();
        }

        private void ShowInvoiceId_CheckedChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("ShowInvoiceId", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowInvoiceCreatedDate_CheckedChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("ShowInvoiceCreatedDate", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowPhoneNumber_CheckedChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("ShowCustomerPhoneNumber", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowCustomerDescription_CheckedChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("ShowCustomerDescription", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowInvoiceDescription_CheckedChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("ShowInvoiceDescription", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void ShowUserDescription_CheckedChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("ShowUserDescription", (sender as CheckBox).Checked);
            RefreshCrystalReport();
        }

        private void cmbPageHeaderFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("PageHeaderFontSize", int.Parse((sender as ComboBox).Text));
            RefreshCrystalReport();
        }

        private void cmbDetailsFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("DetailsFontSize", int.Parse((sender as ComboBox).Text));
            RefreshCrystalReport();
        }

        private void cmbPageFooterFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("PageFooterFontSize", int.Parse((sender as ComboBox).Text));
            RefreshCrystalReport();
        }

        private void cmbDescriptionFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            pi.SetParameterValue("DescriptionFontSize", int.Parse((sender as ComboBox).Text));
            RefreshCrystalReport();
        }
    }
}