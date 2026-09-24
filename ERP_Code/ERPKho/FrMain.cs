using System;
using System.Drawing;
using System.Windows.Forms;

namespace ERPKho
{
    public partial class FrMain : Form
    {
        private Button activeNavButton;

        public FrMain()
        {
            InitializeComponent();
        }

        private void FrMain_Load(object sender, EventArgs e)
        {
            CapNhatThongTinNguoiDung();
            KiemTraPhanQuyenMain();

            // Mở sẵn giao diện Quản lý nhập kho mặc định cho tất cả nhân viên khi đăng nhập
            OpenChildForm(new FrQLNhapKho(), btnQLNhapKho);
        }

        private void CapNhatThongTinNguoiDung()
        {
            lblUserName.Text = !string.IsNullOrEmpty(UserSession.TenNguoiDung) ? UserSession.TenNguoiDung : "Chưa đăng nhập";
            lblRole.Text = !string.IsNullOrEmpty(UserSession.ChucVu) ? UserSession.ChucVu : "N/A";
        }

        private void KiemTraPhanQuyenMain()
        {
            // Mở tất cả các nút trên Sidebar để mọi nhân viên đều truy cập và xem được giao diện
            btnQLNhapKho.Enabled = true;
            btnQLXuatKho.Enabled = true;
            btnQLLuutru.Enabled = true;
            btnQLTonKho.Enabled = true;
            btnQLLoHang.Enabled = true;
            btnQLTraHang.Enabled = true;
        }

        private void OpenChildForm(Form childForm, Button clickedButton)
        {
            if (activeNavButton != null)
            {
                activeNavButton.BackColor = System.Drawing.Color.Crimson;
                activeNavButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
            activeNavButton = clickedButton;
            activeNavButton.BackColor = System.Drawing.Color.FromArgb(13, 110, 253);
            activeNavButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            pnlMainContent.Controls.Clear();
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(childForm);
            childForm.Show();
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            // Cho phép tất cả người dùng mở giao diện các form con từ Sidebar,
            // Các nút thao tác nghiệp vụ bên trong (Thêm/Sửa/Xóa/Duyệt) sẽ do từng Form con tự chặn theo vai trò.
            if (btn == btnQLNhapKho)
            {
                OpenChildForm(new FrQLNhapKho(), btn);
            }
            else if (btn == btnQLLuutru)
            {
                OpenChildForm(new FrQLLuutruvavitri(), btn);
            }
            else if (btn == btnQLTonKho)
            {
                OpenChildForm(new FrQLTonkho(), btn);
            }
            else if (btn == btnQLLoHang)
            {
                OpenChildForm(new FrQLLohangHSD(), btn);
            }
            else if (btn == btnQLXuatKho)
            {
                OpenChildForm(new FrQLXuatkhovaLayhang(), btn);
            }
            else if (btn == btnQLTraHang)
            {
                OpenChildForm(new FrQLTraHangVaTieuHuy(), btn);
            }
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserSession.ClearSession();
                this.Hide();

                FrDangNhap frmLogin = new FrDangNhap();
                if (frmLogin.ShowDialog() == DialogResult.OK)
                {
                    CapNhatThongTinNguoiDung();
                    KiemTraPhanQuyenMain();
                    OpenChildForm(new FrQLNhapKho(), btnQLNhapKho);
                    this.Show();
                }
                else
                {
                    this.Close();
                }
            }
        }
    }
}