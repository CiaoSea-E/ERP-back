using System;
using System.IO;
using System.Windows.Forms;
using HR_Management.GUI;

namespace HR_Management
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    try { File.AppendAllText(logPath, $"[{DateTime.Now}] {e.ExceptionObject}\n"); } catch { }
                };

                // Nếu được gọi với tham số --direct (ví dụ từ ERP_BanHang / ERP_Khach sau khi đăng nhập thành công)
                if (args != null && args.Length > 0 && args[0] == "--direct")
                {
                    if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
                    {
                        var bll = new BLL.HeThongBLL();
                        bll.SetCurrentUserByUsername(args[1]);
                    }
                    Application.Run(new FormMain());
                }
                else
                {
                    // Khởi động bình thường qua màn hình Chọn Phân Hệ
                    Application.Run(new LogInPhanHe());
                }
            }
            catch (Exception ex)
            {
                try { File.AppendAllText(logPath, $"[{DateTime.Now}] Main Exception: {ex}\n"); } catch { }
                MessageBox.Show("Lỗi khởi động: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
