using System;
using System.Windows.Forms;

namespace ERPKho1
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (string.IsNullOrEmpty(UserSession.MaNguoiDung))
            {
                FrDangNhap frmLogin = new FrDangNhap();
                if (frmLogin.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new FrMain());
                }
            }
            else
            {
                Application.Run(new FrMain());
            }
        }
    }
}
