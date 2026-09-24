using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using HR_Management.BLL;
using HR_Management.DAL;

namespace HR_Management.GUI
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class FormPhanHeChung : Form
    {
        private readonly string _tenPhanHe;
        private Color _themeColor;
        private Color _darkColor;
        private Panel pnlContent = null!;
        private Label lblCurrentPageTitle = null!;

        public FormPhanHeChung(string tenPhanHe)
        {
            _tenPhanHe = tenPhanHe;
            ConfigThemeByPhanHe();
            InitializeComponent();
            LoadDefaultView();
        }

        private void ConfigThemeByPhanHe()
        {
            string p = (_tenPhanHe ?? "").ToLowerInvariant();
            if (p.Contains("kho"))
            {
                _themeColor = Color.FromArgb(5, 150, 105);   // Emerald Green
                _darkColor = Color.FromArgb(4, 120, 87);
            }
            else if (p.Contains("logistic"))
            {
                _themeColor = Color.FromArgb(2, 132, 199);   // Ocean Blue
                _darkColor = Color.FromArgb(3, 105, 161);
            }
            else if (p.Contains("tài chính") || p.Contains("taichinh"))
            {
                _themeColor = Color.FromArgb(124, 58, 237);  // Purple
                _darkColor = Color.FromArgb(109, 40, 217);
            }
            else if (p.Contains("sản xuất") || p.Contains("sanxuat"))
            {
                _themeColor = Color.FromArgb(225, 29, 72);   // Rose/Crimson
                _darkColor = Color.FromArgb(190, 18, 60);
            }
            else
            {
                _themeColor = Color.FromArgb(37, 99, 235);   // Blue
                _darkColor = Color.FromArgb(29, 78, 216);
            }
        }

        private void InitializeComponent()
        {
            this.Text = $"HỆ THỐNG QUẢN LÝ {_tenPhanHe.ToUpper()} - ERP ACECOOK";
            this.Size = new Size(1280, 780);
            this.MinimumSize = new Size(1024, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ThemeColor.BodyBg;
            this.Font = ThemeColor.BodyFont;
            this.WindowState = FormWindowState.Maximized;

            // ============ 1. SIDEBAR ============
            Panel pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = _themeColor
            };

            // Logo Header
            Panel pnlLogoHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                BackColor = Color.Transparent,
                Padding = new Padding(16, 14, 16, 10)
            };

            Panel pnlLogoCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 8)
            };

            Label lblLogoText = new Label
            {
                Text = "ACECOOK ERP",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = _themeColor,
                Dock = DockStyle.Top,
                Height = 26
            };

            Label lblSlogan = new Label
            {
                Text = $"Phân hệ {_tenPhanHe}",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Top,
                Height = 20
            };

            pnlLogoCard.Controls.Add(lblSlogan);
            pnlLogoCard.Controls.Add(lblLogoText);
            pnlLogoHeader.Controls.Add(pnlLogoCard);

            // Menu Items
            Panel pnlMenuItems = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0, 8, 0, 8)
            };

            Button btn1 = CreateSidebarBtn($"📊  Tổng quan {_tenPhanHe}", 0);
            btn1.BackColor = _darkColor;
            Button btn2 = CreateSidebarBtn($"📋  Dữ liệu nghiệp vụ", 52);
            Button btn3 = CreateSidebarBtn($"📈  Báo cáo & Thống kê", 104);

            btn1.Click += (s, e) => LoadDefaultView();
            btn2.Click += (s, e) => LoadDataView();
            btn3.Click += (s, e) => LoadReportView();

            pnlMenuItems.Controls.Add(btn1);
            pnlMenuItems.Controls.Add(btn2);
            pnlMenuItems.Controls.Add(btn3);

            // Footer
            Panel pnlSidebarFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 140,
                BackColor = _darkColor,
                Padding = new Padding(16, 12, 16, 12)
            };

            string userName = HeThongBLL.CurrentUser?.TenNV ?? HeThongBLL.CurrentUser?.TenDangNhap ?? "Admin";
            string role = HeThongBLL.CurrentUser?.VaiTro ?? "Quản trị viên";

            Label lblUserName = new Label
            {
                Text = "👤 " + userName,
                Font = ThemeColor.BodyFontBold,
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 22
            };

            Label lblRole = new Label
            {
                Text = "Vai trò: " + role,
                Font = ThemeColor.SmallFont,
                ForeColor = Color.FromArgb(241, 245, 249),
                Dock = DockStyle.Top,
                Height = 18
            };

            Button btnLogout = new Button
            {
                Text = "🚪 Đăng xuất / Đổi phân hệ",
                Font = ThemeColor.BodyFontBold,
                Dock = DockStyle.Bottom,
                Height = 38,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có muốn đăng xuất và quay lại màn hình chọn phân hệ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Restart();
                }
            };

            pnlSidebarFooter.Controls.Add(lblUserName);
            pnlSidebarFooter.Controls.Add(lblRole);
            pnlSidebarFooter.Controls.Add(btnLogout);

            pnlSidebar.Controls.Add(pnlMenuItems);
            pnlSidebar.Controls.Add(pnlSidebarFooter);
            pnlSidebar.Controls.Add(pnlLogoHeader);

            // ============ 2. TOPBAR ============
            Panel pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.White,
                Padding = new Padding(24, 0, 24, 0)
            };

            FlowLayoutPanel flpTitle = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 18, 0, 0)
            };

            Label lblBreadcrumb = new Label
            {
                Text = $"ERP Acecook  ›  {_tenPhanHe}  ›  ",
                Font = ThemeColor.SmallFontBold,
                ForeColor = ThemeColor.TextMuted,
                AutoSize = true
            };

            lblCurrentPageTitle = new Label
            {
                Text = $"Tổng quan {_tenPhanHe}",
                Font = ThemeColor.SubHeaderFont,
                ForeColor = ThemeColor.TextPrimary,
                AutoSize = true
            };

            flpTitle.Controls.Add(lblBreadcrumb);
            flpTitle.Controls.Add(lblCurrentPageTitle);

            FlowLayoutPanel flpRight = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 16, 0, 0)
            };

            Label lblClock = new Label
            {
                Text = $"🕒 {DateTime.Now:dd/MM/yyyy  |  HH:mm:ss}",
                Font = ThemeColor.SmallFont,
                ForeColor = ThemeColor.TextSecondary,
                AutoSize = true,
                Margin = new Padding(0, 4, 16, 0)
            };

            var timerClock = new System.Windows.Forms.Timer { Interval = 1000 };
            timerClock.Tick += (s, e) => lblClock.Text = $"🕒 {DateTime.Now:dd/MM/yyyy  |  HH:mm:ss}";
            timerClock.Start();
            this.FormClosed += (s, e) => { timerClock.Stop(); timerClock.Dispose(); };

            Label lblUserBadge = new Label
            {
                Text = $"👤 {userName} [{role}]",
                Font = ThemeColor.SmallFontBold,
                ForeColor = _themeColor,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(8, 4, 8, 4),
                AutoSize = true
            };

            flpRight.Controls.Add(lblClock);
            flpRight.Controls.Add(lblUserBadge);

            pnlTopBar.Controls.Add(flpTitle);
            pnlTopBar.Controls.Add(flpRight);

            // ============ 3. CONTENT AREA ============
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColor.BodyBg,
                Padding = new Padding(20)
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlTopBar);
            this.Controls.Add(pnlSidebar);
        }

        private Button CreateSidebarBtn(string text, int top)
        {
            Button btn = new Button
            {
                Text = "  " + text,
                Location = new Point(12, top),
                Size = new Size(256, 44),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = ThemeColor.BodyFontBold,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadDefaultView()
        {
            lblCurrentPageTitle.Text = $"Tổng quan phân hệ {_tenPhanHe}";
            pnlContent.Controls.Clear();

            // Thẻ KPI chào mừng
            Panel banner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.White,
                Padding = new Padding(24, 18, 24, 18)
            };

            Label lblBannerTitle = new Label
            {
                Text = $"✨ Chào mừng đến với Phân hệ {_tenPhanHe}",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = _themeColor,
                Dock = DockStyle.Top,
                Height = 36
            };

            Label lblBannerSub = new Label
            {
                Text = $"Bạn đang đăng nhập dưới quyền '{HeThongBLL.CurrentUser?.QuyenHan}' của tài khoản {HeThongBLL.CurrentUser?.TenDangNhap}. Tất cả chức năng nghiệp vụ của {_tenPhanHe} đã sẵn sàng vận hành.",
                Font = ThemeColor.BodyFont,
                ForeColor = ThemeColor.TextSecondary,
                Dock = DockStyle.Top,
                Height = 24
            };

            banner.Controls.Add(lblBannerSub);
            banner.Controls.Add(lblBannerTitle);

            // Grid dữ liệu mẫu theo phân hệ
            Panel pnlTable = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 16, 0, 0)
            };

            Label lblGridTitle = new Label
            {
                Text = $"Danh sách dữ liệu hoạt động - Phân hệ {_tenPhanHe}",
                Font = ThemeColor.SubHeaderFont,
                ForeColor = ThemeColor.TextPrimary,
                Dock = DockStyle.Top,
                Height = 32
            };

            DataGridView dgv = CreateStyledGrid();
            dgv.Dock = DockStyle.Fill;

            try
            {
                string sql = GetQueryForPhanHe();
                if (!string.IsNullOrEmpty(sql))
                {
                    dgv.DataSource = DatabaseHelper.ExecuteQuery(sql);
                }
            }
            catch { }

            pnlTable.Controls.Add(dgv);
            pnlTable.Controls.Add(lblGridTitle);

            pnlContent.Controls.Add(pnlTable);
            pnlContent.Controls.Add(banner);
        }

        private void LoadDataView()
        {
            lblCurrentPageTitle.Text = $"Dữ liệu chi tiết - {_tenPhanHe}";
            pnlContent.Controls.Clear();

            DataGridView dgv = CreateStyledGrid();
            dgv.Dock = DockStyle.Fill;

            try
            {
                string sql = GetQueryForPhanHe();
                if (!string.IsNullOrEmpty(sql))
                {
                    dgv.DataSource = DatabaseHelper.ExecuteQuery(sql);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }

            pnlContent.Controls.Add(dgv);
        }

        private void LoadReportView()
        {
            lblCurrentPageTitle.Text = $"Báo cáo & Thống kê - {_tenPhanHe}";
            pnlContent.Controls.Clear();

            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(30)
            };

            Label lblRpt = new Label
            {
                Text = $"📊 Báo cáo định kỳ phân hệ {_tenPhanHe}\n\n- Dữ liệu được đồng bộ trực tiếp từ SQL Server (ERP_BanHang).\n- Trạng thái kết nối cơ sở dữ liệu: Hoạt động bình thường.\n- Người lập báo cáo: {HeThongBLL.CurrentUser?.TenNV ?? "Admin"}",
                Font = new Font("Segoe UI", 12F),
                ForeColor = ThemeColor.TextPrimary,
                Dock = DockStyle.Fill
            };

            card.Controls.Add(lblRpt);
            pnlContent.Controls.Add(card);
        }

        private string GetQueryForPhanHe()
        {
            string p = (_tenPhanHe ?? "").ToLowerInvariant();
            if (p.Contains("kho"))
            {
                return "SELECT TOP 50 ID_HangHoa AS [Mã Hàng], TenHangHoa AS [Tên Hàng Hóa], DonViTinh AS [ĐVT], DonGia AS [Đơn Giá] FROM HangHoa";
            }
            else if (p.Contains("sản xuất") || p.Contains("sanxuat"))
            {
                return "SELECT TOP 50 ID_HangHoa AS [Mã SP], TenHangHoa AS [Tên Sản Phẩm], DonViTinh AS [Quy Cách], DonGia AS [Giá Thành] FROM HangHoa";
            }
            else if (p.Contains("tài chính") || p.Contains("taichinh"))
            {
                return "SELECT TOP 50 ID_HoaDon AS [Mã Hóa Đơn], ID_DH AS [Đơn Hàng], CONVERT(VARCHAR(10), NgayLap, 103) AS [Ngày Lập], FORMAT(TongTien, 'N0') + ' đ' AS [Tổng Tiền], TrangThaiTT AS [Trạng Thái TT] FROM HoaDon";
            }
            else if (p.Contains("logistic"))
            {
                return "SELECT TOP 50 ID_DH AS [Mã Đơn Vận], ID_KH AS [Khách Nhận], CONVERT(VARCHAR(10), NgayTao, 103) AS [Ngày Gửi], TrangThai AS [Trạng Thái Vận Chuyển] FROM DonHang";
            }
            return "SELECT TOP 50 maTaiKhoan, ID_NV, tenDangNhap, vaiTro, quyenHan, trangThai FROM HeThong";
        }

        private DataGridView CreateStyledGrid()
        {
            DataGridView dgv = new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 40 },
                EnableHeadersVisualStyles = false
            };

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(51, 65, 85);
            dgv.ColumnHeadersDefaultCellStyle.Font = ThemeColor.BodyFontBold;
            dgv.ColumnHeadersHeight = 44;

            dgv.DefaultCellStyle.Font = ThemeColor.BodyFont;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);

            return dgv;
        }
    }
}
