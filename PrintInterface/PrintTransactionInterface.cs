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
    public partial class PrintTransactionInterface : Form
    {
        public PrintTransactionInterface()
        {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs();
            FilePath = args[2]; //Report file path
        }

        PrintTransactionModel ptm = new PrintTransactionModel();
        ReportDocument rd;
        string FilePath;
        bool CanRefresh = false;

        private void PrintWindow_Load(object sender, EventArgs e)
        {
            //var xmlSerializer = new XmlSerializer(ptm.GetType());
            //string xmlString = File.ReadAllText(@"D:\Users\avazeh1\Downloads\AvazehAccountingClone\AvazehWpf\bin\Debug\net5.0-windows\Temp\637982546060107715.xml");
            //StringReader stringReader = new StringReader(xmlString);
            //ptm = xmlSerializer.Deserialize(stringReader) as PrintTransactionModel;

            if (!File.Exists(FilePath))
            {
                MessageBox.Show("فایل فاکتور یافت نشد\n" + FilePath, "خطای پارامتر ورودی", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            var xmlSerializer = new XmlSerializer(ptm.GetType());
            string xmlString = File.ReadAllText(FilePath);
            StringReader stringReader = new StringReader(xmlString);
            ptm = xmlSerializer.Deserialize(stringReader) as PrintTransactionModel;
            File.Delete(FilePath);

            if (ptm.PrintSettings.UserDescriptions != null && ptm.PrintSettings.UserDescriptions.Count > 0)
            {
                cmbUserDescriptions.DataSource = ptm.PrintSettings.UserDescriptions;
                cmbUserDescriptions.DisplayMember = nameof(UserDescriptionModel.DescriptionTitle);
                cmbUserDescriptions.ValueMember = nameof(UserDescriptionModel.DescriptionText);
            }

            ShowTransactionId.Checked = ptm.PrintTransactionId;
            ShowTransactionCreatedDate.Checked = ptm.PrintDate;
            ShowTransactionDescription.Checked = ptm.PrintTransactionDescription;
            ShowUserDescription.Checked = ptm.PrintUserDescription;

            cmbPageHeaderFontSize.Text = ptm.PrintSettings.PageHeaderFontSize.ToString();
            cmbDetailsFontSize.Text = ptm.PrintSettings.DetailsFontSize.ToString();
            cmbPageFooterFontSize.Text = ptm.PrintSettings.PageFooterFontSize.ToString();
            cmbDescriptionFontSize.Text = ptm.PrintSettings.DescriptionFontSize.ToString();
            cmbPrintLayout.Text = ptm.PrintSettings.DefaultPrintLayout;
            cmbPageSize.Text = ptm.PrintSettings.DefaultPaperSize;

            txtTransactionDescription.Text = ptm.TransactionDescription;

            if (string.IsNullOrEmpty(ptm.TransactionDescription)) ShowTransactionDescription.Enabled = txtTransactionDescription.Enabled = ShowTransactionDescription.Checked = false;
            if (string.IsNullOrEmpty(ptm.TransactionDescription) && (ptm.PrintSettings.UserDescriptions == null || ptm.PrintSettings.UserDescriptions.Count == 0)) cmbDescriptionFontSize.Enabled = false;

            CanRefresh = true;
            InitilizeReport();
        }

        private void InitilizeReport()
        {
            if (ptm.PrintSettings.DefaultPaperSize == "A5")
            {
                if (ptm.PrintSettings.DefaultPrintLayout == "عمودی") rd = new PrintTransactionPortraitA5();
                else rd = new PrintTransactionLandscapeA5();
            }
            else if (ptm.PrintSettings.DefaultPaperSize == "A4") rd = new PrintTransactionPortraitA4();

            rd.SetDataSource(ptm.Items);
            rd.SetParameterValue(nameof(ptm.FileName), ptm.FileName);
            rd.SetParameterValue(nameof(ptm.TransactionId), ptm.TransactionId);
            rd.SetParameterValue(nameof(ptm.TransactionDateCreated), ptm.TransactionDateCreated);
            rd.SetParameterValue(nameof(ptm.TotalPositiveItemsSum), ptm.TotalPositiveItemsSum);
            rd.SetParameterValue(nameof(ptm.TotalNegativeItemsSum), ptm.TotalNegativeItemsSum);
            rd.SetParameterValue(nameof(ptm.TotalBalance), ptm.TotalBalance);
            rd.SetParameterValue(nameof(ptm.PrintSettings.FooterTextRight), ptm.PrintSettings.FooterTextRight);
            rd.SetParameterValue(nameof(ptm.PrintSettings.FooterTextLeft), ptm.PrintSettings.FooterTextLeft);
            rd.SetParameterValue(nameof(ptm.TransactionFinStatus), ptm.TransactionFinStatus);
            rd.SetParameterValue(nameof(ptm.PrintTransactionId), ptm.PrintTransactionId);
            rd.SetParameterValue(nameof(ptm.PrintDate), ptm.PrintDate);
            rd.SetParameterValue(nameof(ptm.PrintTransactionDescription), ptm.PrintTransactionDescription);
            rd.SetParameterValue(nameof(ptm.PrintUserDescription), ptm.PrintUserDescription);
            if(ptm.PrintUserDescription) rd.SetParameterValue("UserDescription", txtUserDescription.Text); else rd.SetParameterValue("UserDescription", "");
            rd.SetParameterValue(nameof(ptm.PrintSettings.PageHeaderFontSize), ptm.PrintSettings.PageHeaderFontSize);
            rd.SetParameterValue(nameof(ptm.PrintSettings.DetailsFontSize), ptm.PrintSettings.DetailsFontSize);
            rd.SetParameterValue(nameof(ptm.PrintSettings.PageFooterFontSize), ptm.PrintSettings.PageFooterFontSize);
            rd.SetParameterValue(nameof(ptm.PrintSettings.DescriptionFontSize), ptm.PrintSettings.DescriptionFontSize);
            rd.SetParameterValue(nameof(ptm.PrintSettings.LeftHeaderImage), Application.StartupPath + ptm.PrintSettings.LeftHeaderImage);
            rd.SetParameterValue(nameof(ptm.PrintSettings.RightHeaderImage), Application.StartupPath + ptm.PrintSettings.RightHeaderImage);
            rd.SetParameterValue(nameof(ptm.PrintSettings.MainHeaderText), ptm.PrintSettings.MainHeaderText);
            rd.SetParameterValue(nameof(ptm.PrintSettings.HeaderDescription1), ptm.PrintSettings.HeaderDescription1);
            rd.SetParameterValue(nameof(ptm.PrintSettings.HeaderDescription2), ptm.PrintSettings.HeaderDescription2);
            rd.SetParameterValue(nameof(ptm.TransactionDescription), ptm.TransactionDescription);
            rd.SetParameterValue(nameof(ptm.TransactionType), ptm.TransactionType);
            SetFontSizeValuesBasedOnPaper();
            crystalReportViewer.ReportSource = rd;
        }

        private void SetFontSizeValuesBasedOnPaper()
        {
            if (ptm.PrintSettings.DefaultPaperSize == "A4")
            {
                rd.SetParameterValue("MainHeaderTextFontSize", ptm.PrintSettings.MainHeaderTextFontSizeA4P);
                rd.SetParameterValue("HeaderDescriptionFontSize", ptm.PrintSettings.HeaderDescriptionFontSizeA4P);
            }
            else if (ptm.PrintSettings.DefaultPaperSize == "A5")
                if (ptm.PrintSettings.DefaultPrintLayout == "عمودی")
                {
                    rd.SetParameterValue("MainHeaderTextFontSize", ptm.PrintSettings.MainHeaderTextFontSizeA5P);
                    rd.SetParameterValue("HeaderDescriptionFontSize", ptm.PrintSettings.HeaderDescriptionFontSizeA5P);
                }
                else if (ptm.PrintSettings.DefaultPrintLayout == "افقی")
                {
                    rd.SetParameterValue("MainHeaderTextFontSize", ptm.PrintSettings.MainHeaderTextFontSizeA5L);
                    rd.SetParameterValue("HeaderDescriptionFontSize", ptm.PrintSettings.HeaderDescriptionFontSizeA5L);
                }
        }

        private void RefreshCrystalReport()
        {
            if (!CanRefresh) return;
            crystalReportViewer.ReportSource = rd;
            crystalReportViewer.Refresh();
        }

        private void ShowTransactionId_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            ptm.PrintTransactionId = (sender as CheckBox).Checked;
            rd.SetParameterValue(nameof(ptm.PrintTransactionId), ptm.PrintTransactionId);
            RefreshCrystalReport();
        }

        private void ShowTransactionCreatedDate_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            ptm.PrintDate = (sender as CheckBox).Checked;
            rd.SetParameterValue(nameof(ptm.PrintDate), ptm.PrintDate);
            RefreshCrystalReport();
        }

        private void ShowTransactionDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            ptm.PrintTransactionDescription = (sender as CheckBox).Checked;
            rd.SetParameterValue(nameof(ptm.PrintTransactionDescription), ptm.PrintTransactionDescription);
            RefreshCrystalReport();
        }

        private void ShowUserDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            ptm.PrintUserDescription = (sender as CheckBox).Checked;
            rd.SetParameterValue(nameof(ptm.PrintUserDescription), ptm.PrintUserDescription);
            rd.SetParameterValue("UserDescription", txtUserDescription.Text);
            RefreshCrystalReport();
        }

        private void cmbPageHeaderFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            ptm.PrintSettings.PageHeaderFontSize = int.Parse((sender as ComboBox).Text, CultureInfo.InvariantCulture);
            rd.SetParameterValue(nameof(ptm.PrintSettings.PageHeaderFontSize), ptm.PrintSettings.PageHeaderFontSize);
            RefreshCrystalReport();
        }

        private void cmbDetailsFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            ptm.PrintSettings.DetailsFontSize = int.Parse((sender as ComboBox).Text, CultureInfo.InvariantCulture);
            rd.SetParameterValue(nameof(ptm.PrintSettings.DetailsFontSize), ptm.PrintSettings.DetailsFontSize);
            RefreshCrystalReport();
        }

        private void cmbPageFooterFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            ptm.PrintSettings.PageFooterFontSize = int.Parse((sender as ComboBox).Text, CultureInfo.InvariantCulture);
            rd.SetParameterValue(nameof(ptm.PrintSettings.PageFooterFontSize), ptm.PrintSettings.PageFooterFontSize);
            RefreshCrystalReport();
        }

        private void cmbDescriptionFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CanRefresh) return;
            ptm.PrintSettings.DescriptionFontSize = int.Parse((sender as ComboBox).Text, CultureInfo.InvariantCulture);
            rd.SetParameterValue(nameof(ptm.PrintSettings.DescriptionFontSize), ptm.PrintSettings.DescriptionFontSize);
            RefreshCrystalReport();
        }

        private void PrintTransactionInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void cmbUserDescriptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUserDescription.Text = cmbUserDescriptions.SelectedValue.ToString();
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
            ptm.PrintSettings.DefaultPaperSize = (sender as ComboBox).Text;
            if (ptm.PrintSettings.DefaultPaperSize == "A4")
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
            ptm.PrintSettings.DefaultPrintLayout = (sender as ComboBox).Text;
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
    }
}