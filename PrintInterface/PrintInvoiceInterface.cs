using System;
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
            var FilePath = args[0];
            var xmlSerializer = new XmlSerializer(pim.GetType());
            string xmlString = File.ReadAllText(FilePath);
            StringReader stringReader = new StringReader(xmlString);
            pim = xmlSerializer.Deserialize(stringReader) as PrintInvoiceModel;
        }
    }
}