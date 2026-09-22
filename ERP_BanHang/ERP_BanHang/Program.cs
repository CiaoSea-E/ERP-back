using System;
using System.Windows.Forms;
using OfficeOpenXml;

namespace ERP_BanHang
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Thiet lap LicenseContext cho EPPlus Excel
            try { ExcelPackage.LicenseContext = LicenseContext.NonCommercial; } catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new QlyDonHang());
        }
    }
}
