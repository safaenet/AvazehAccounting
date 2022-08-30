using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintInterface
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string[] args = Environment.GetCommandLineArgs();
            if (args == null || args.Length < 3)
            {
                MessageBox.Show("پارامترهای لازم وارد نشده اند یا به درستی وارد نشده اند", "خطای پارامتر ورودی", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else if (args[1] == "invoice")
                Application.Run(new PrintInvoiceInterface());
            else Application.Exit();
        }
    }
}