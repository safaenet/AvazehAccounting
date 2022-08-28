using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            PrintInvoice pi = new PrintInvoice();
            pi.SetDataSource(pim.Products);
            pi.SetParameterValue("InvoiceDateCreated", pim.InvoiceDateCreated);
            pi.SetParameterValue("CustomerFullName", pim.CustomerFullName);
            pi.SetParameterValue("CustomerPhoneNumber", pim.CustomerPhoneNumber);
            pi.SetParameterValue("TotalItemsSellSum", pim.TotalItemsSellSum);
            pi.SetParameterValue("TotalDiscountAmount", pim.TotalDiscountAmount);
            pi.SetParameterValue("TotalInvoiceSum", pim.TotalInvoiceSum);
            pi.SetParameterValue("TotalPayments", pim.TotalPayments);
            pi.SetParameterValue("TotalBalance", pim.TotalBalance);
            pi.SetParameterValue("FooterTextRight", pim.FooterTextRight);
            pi.SetParameterValue("FooterTextLeft", pim.FooterTextLeft);
            pi.SetParameterValue("CustomerPreviousBalance", pim.CustomerPreviousBalance);
            pi.SetParameterValue("InvoiceFinStatus", pim.InvoiceFinStatus);
            pi.SetParameterValue("InvoiceType", pim.InvoiceType);
            crystalReportViewer.ReportSource = pi;
        }

        private void PrintInvoiceInterface_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}