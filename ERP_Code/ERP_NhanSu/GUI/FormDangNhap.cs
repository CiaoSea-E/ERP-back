using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HR_Management.BLL;
using HR_Management.DAL;

namespace HR_Management.GUI
{
    public partial class FormDangNhap : Form
    {
        private readonly HeThongBLL _bllHeThong = new HeThongBLL();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TenPhanHe { get; set; } = "Hệ thống";

        public FormDangNhap() : this("Nhân sự")
        {
        }

        public FormDangNhap(string phanHe)
        {
            InitializeComponent();
            TenPhanHe = phanHe;
            lblSubTitle.Text = $"Đăng nhập phân hệ: {phanHe}";
            this.Text = $"Đăng Nhập - Phân hệ {phanHe}";
        }

        private void FormDangNhap_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            txtTaiKhoan.Focus();

            try
            {
                if (DatabaseHelper.TestConnection(out _))
                {
                    _bllHeThong.EnsureDefaultAdmin();
                }
            }
            catch
            {
                // Bỏ qua nếu chưa kết nối cơ sở dữ liệu
            }
        }

        // Ẩn / Hiện Mật Khẩu
        private void chkHienThiMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            txtMatKhau.UseSystemPasswordChar = !chkHienThiMatKhau.Checked;
        }

        // Nút Đăng Nhập
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string taiKhoan = txtTaiKhoan.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            // Kiểm tra rỗng cơ bản giao diện
            if (string.IsNullOrEmpty(taiKhoan))
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaiKhoan.Focus();
                return;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }

            // Xác thực tài khoản với database
            try
            {
                var account = _bllHeThong.Login(taiKhoan, matKhau, out string error);
                if (account != null)
                {
                    // Kiểm tra quyền hạn tương ứng với phân hệ được chọn
                    if (!_bllHeThong.KiemTraQuyenPhanHe(account, TenPhanHe, out string errQuyen))
                    {
                        MessageBox.Show(errQuyen, "Từ chối truy cập phân hệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(string.IsNullOrEmpty(error) ? "Tài khoản hoặc mật khẩu không chính xác!" : error, 
                                    "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMatKhau.Focus();
                    txtMatKhau.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nút Quay lại / Thoát
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
