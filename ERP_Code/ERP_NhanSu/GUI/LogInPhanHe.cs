using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HR_Management.GUI
{
    public partial class LogInPhanHe : Form
    {
        public LogInPhanHe()
        {
            InitializeComponent();
        }

        private System.Windows.Forms.Timer? _clockTimer;

        private void LogInPhanHe_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            UpdateClock();

            // Khởi động đồng hồ thời gian thực cập nhật mỗi giây
            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (s, ev) => UpdateClock();
            _clockTimer.Start();
            this.FormClosed += (s, ev) => { _clockTimer?.Stop(); _clockTimer?.Dispose(); };

            // Đăng ký hiệu ứng hover cho các ô phân hệ
            SetupHoverEffect(pnlSanXuat, Color.FromArgb(255, 245, 246), Color.FromArgb(254, 226, 226));
            SetupHoverEffect(pnlBanHang, Color.FromArgb(255, 252, 240), Color.FromArgb(254, 243, 199));
            SetupHoverEffect(pnlLogistics, Color.FromArgb(240, 248, 255), Color.FromArgb(224, 242, 254));
            SetupHoverEffect(pnlKho, Color.FromArgb(242, 252, 248), Color.FromArgb(209, 250, 229));
            SetupHoverEffect(pnlNhanSu, Color.FromArgb(250, 245, 255), Color.FromArgb(243, 232, 255));
            SetupHoverEffect(pnlTaiChinh, Color.FromArgb(255, 253, 245), Color.FromArgb(254, 249, 195));
        }

        private void UpdateClock()
        {
            var viCulture = new System.Globalization.CultureInfo("vi-VN");
            string timeStr = DateTime.Now.ToString("dddd, 'ngày' dd/MM/yyyy · HH:mm:ss", viCulture);
            if (!string.IsNullOrEmpty(timeStr))
                timeStr = char.ToUpper(timeStr[0]) + timeStr.Substring(1);
            lblThoiGian.Text = "🕒 " + timeStr;
        }

        private void SetupHoverEffect(Panel pnl, Color normalColor, Color hoverColor)
        {
            Action onEnter = () => pnl.BackColor = hoverColor;
            Action onLeave = () => pnl.BackColor = normalColor;

            pnl.MouseEnter += (s, e) => onEnter();
            pnl.MouseLeave += (s, e) => onLeave();

            foreach (Control c in pnl.Controls)
            {
                c.MouseEnter += (s, e) => onEnter();
                c.MouseLeave += (s, e) => onLeave();
            }
        }

        private void pnlSanXuat_Click(object sender, EventArgs e)
        {
            MoFormDangNhap("Sản xuất");
        }

        private void pnlBanHang_Click(object sender, EventArgs e)
        {
            MoFormDangNhap("Bán hàng");
        }

        private void pnlLogistics_Click(object sender, EventArgs e)
        {
            MoFormDangNhap("Logistics");
        }

        private void pnlKho_Click(object sender, EventArgs e)
        {
            MoFormDangNhap("Kho");
        }

        private void pnlNhanSu_Click(object sender, EventArgs e)
        {
            MoFormDangNhap("Nhân sự");
        }

        private void pnlTaiChinh_Click(object sender, EventArgs e)
        {
            MoFormDangNhap("Tài chính");
        }

        // Hàm mở Form Đăng Nhập cho từng phân hệ
        private void MoFormDangNhap(string tenPhanHe)
        {
            
            using (FormDangNhap login = new FormDangNhap(tenPhanHe))
            {
                DialogResult result = login.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string phanHeLower = (tenPhanHe ?? "").Trim().ToLowerInvariant();
                    bool laBanHang = phanHeLower.Contains("bán hàng") || phanHeLower.Contains("banhang") || phanHeLower.Contains("sales");

                    if (laBanHang)
                    {
                        // Mở Form QlyDonHang thật (ERP_BanHang) qua Process.Start
                        MoPhanHeBanHang();
                    }
                    else
                    {
                        // Mở giao diện tương ứng với phân hệ đã chọn
                        Form formMain;
                        if (phanHeLower.Contains("nhân sự") || phanHeLower.Contains("nhansu") || phanHeLower.Contains("hr"))
                        {
                            formMain = new FormMain();
                        }
                        else
                        {
                            formMain = new FormPhanHeChung(tenPhanHe ?? "Phân hệ");
                        }

                        formMain.FormClosed += (s, args) => this.Show();
                        this.Hide();
                        formMain.Show();
                    }
                }
                else
                {
                    // Nếu bấm 'Quay lại' hoặc đóng form đăng nhập -> hiển thị lại màn hình chọn phân hệ
                    this.Show();
                }
            }
        }

        /// <summary>
        /// Mở phân hệ Bán hàng thật (QlyDonHang) từ project ERP_BanHang qua Process.Start.
        /// Vì ERP_BanHang chạy .NET Framework 4.8 và HR_Management chạy .NET 10,
        /// không thể tham chiếu trực tiếp giữa 2 project.
        /// </summary>
        private void MoPhanHeBanHang()
        {
            try
            {
                this.Hide();

                string username = BLL.HeThongBLL.CurrentUser?.TenDangNhap ??
                                  BLL.HeThongBLL.CurrentUser?.TenNV ?? "Admin";

                ERP_BanHang.QlyDonHang.CurrentEmployeeName = username;
                ERP_BanHang.QlyDonHang.CurrentRole = "Nhân viên";

                using (ERP_BanHang.QlyDonHang frmBanHang = new ERP_BanHang.QlyDonHang())
                {
                    frmBanHang.ShowDialog();
                }

                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở Phân hệ Bán hàng: " + ex.Message,
                                "Lỗi khởi động", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        /// <summary>
        /// Tìm file ERP_Khach.exe bằng đường dẫn tương đối (hoạt động trên mọi máy tính).
        /// </summary>
        private string? TimDuongDanErpKhachExe()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Các đường dẫn tương đối phổ biến từ HR_Management/bin/Debug/net10.0-windows/
            string[] candidates = new[]
            {
                // ERP_BanHang nằm cùng cấp với HR_Management trong thư mục ERP
                Path.Combine(baseDir, @"..\..\..\..\ERP_BanHang\ERP_Khach\bin\Debug\ERP_Khach.exe"),
                Path.Combine(baseDir, @"..\..\..\..\ERP_BanHang\ERP_Khach\bin\Release\ERP_Khach.exe"),
                // Từ thư mục ERP
                Path.Combine(baseDir, @"..\..\..\ERP_BanHang\ERP_Khach\bin\Debug\ERP_Khach.exe"),
                Path.Combine(baseDir, @"..\..\ERP_BanHang\ERP_Khach\bin\Debug\ERP_Khach.exe"),
                Path.Combine(baseDir, @"..\ERP_BanHang\ERP_Khach\bin\Debug\ERP_Khach.exe"),
                // Cùng thư mục chạy
                Path.Combine(baseDir, "ERP_Khach.exe"),
            };

            foreach (string candidate in candidates)
            {
                try
                {
                    string fullPath = Path.GetFullPath(candidate);
                    if (File.Exists(fullPath)) return fullPath;
                }
                catch { }
            }

            // Tìm ngược lên các cấp thư mục cha
            try
            {
                DirectoryInfo? dir = new DirectoryInfo(baseDir);
                for (int i = 0; i < 6 && dir != null; i++)
                {
                    string checkPath = Path.Combine(dir.FullName, "ERP_BanHang", "ERP_Khach", "bin", "Debug", "ERP_Khach.exe");
                    if (File.Exists(checkPath)) return checkPath;

                    checkPath = Path.Combine(dir.FullName, "ERP_BanHang", "ERP_Khach", "bin", "Release", "ERP_Khach.exe");
                    if (File.Exists(checkPath)) return checkPath;

                    dir = dir.Parent;
                }
            }
            catch { }

            return null;
        }
    }
}
