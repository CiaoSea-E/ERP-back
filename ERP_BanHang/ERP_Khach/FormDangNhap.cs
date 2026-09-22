using ERP_BanHang; // Tham chiếu sang Project Bán Hàng

using Npgsql; // Thư viện PostgreSQL cho Neon Data
using System;
using System.Configuration; 
using System.Windows.Forms;

namespace ERP_Khach
{
    public partial class FormDangNhap : Form
    {
        // Chuỗi kết nối Neon Postgres (Thay chuỗi kết nối của bạn vào đây)
        private string connectionString = ConfigurationManager.ConnectionStrings["ERP_Connection"].ConnectionString;
   

       
       
        private string _targetPhanHe = "Bán hàng";

        public FormDangNhap(string phanHe = "Bán hàng")
        {
            InitializeComponent();
            _targetPhanHe = phanHe;
            this.Text = $"ĐĂNG NHẬP HỆ THỐNG - PHÂN HỆ {phanHe.ToUpper()}";
        }

        private void FormDangNhap_Load(object sender, EventArgs e)
        {
            txtTaiKhoan.Focus();
        }

        // ==========================================
        // XỬ LÝ ĐĂNG NHẬP VÀ PHÂN QUYỀN
        // ==========================================
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtTaiKhoan.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaiKhoan.Focus();
                return;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }

            // Gọi hàm kiểm tra tài khoản & lấy vai trò
            KiemTraVaChuyenForm(tenDangNhap, matKhau);
        }

        private void KiemTraVaChuyenForm(string tenDangNhap, string matKhau)
        {
            // Query JOIN bảng hethong với nhanvien thông qua id_nv
            string query = @"
                SELECT 
                    h.id_nv,
                    h.vaitro,
                    n.tennv,
                    n.chucvu,
                    n.phongban
                FROM hethong h
                INNER JOIN nhanvien n ON h.id_nv = n.id_nv
                WHERE LOWER(h.tendangnhap) = LOWER(@TenDangNhap) 
                  AND h.matkhau = @MatKhau";

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                    cmd.Parameters.AddWithValue("@MatKhau", matKhau);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Đăng nhập đúng tài khoản -> Lấy các thông tin nhân viên
                            string idNV = reader["id_nv"]?.ToString();
                            string tenNV = reader["tennv"]?.ToString();
                            string chucVu = reader["chucvu"]?.ToString();
                            string vaiTro = reader["vaitro"]?.ToString();

                            // XÉT PHÂN HỆ ĐƯỢC CHỌN
                            if (_targetPhanHe.Equals("Bán hàng", StringComparison.OrdinalIgnoreCase))
                            {
                                if (KiemTraQuyenBanHang(chucVu, vaiTro))
                                {
                                    MessageBox.Show($"Đăng nhập thành công!\nMã NV: {idNV}\nHọ tên: {tenNV}\nChức vụ: {chucVu}\nQuyền: Cho phép truy cập Phân hệ Bán Hàng.",
                                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    QlyDonHang.CurrentEmployeeId = idNV;
                                    QlyDonHang.CurrentEmployeeName = tenNV;
                                    QlyDonHang.CurrentRole = string.IsNullOrEmpty(chucVu) ? vaiTro : chucVu;

                                    this.Hide();

                                    // Mở Form Bán Hàng thuộc Project ERP_BanHang
                                    using (QlyDonHang frmBanHang = new QlyDonHang())
                                    {
                                        frmBanHang.ShowDialog();
                                    }

                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show($"Tài khoản của nhân viên [{tenNV}] (Chức vụ: {chucVu}) không có quyền truy cập vào Phân hệ Bán Hàng!",
                                                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                            }
                            else if (_targetPhanHe.Equals("Nhân sự", StringComparison.OrdinalIgnoreCase))
                            {
                                if (KiemTraQuyenNhanSu(chucVu, vaiTro))
                                {
                                    MessageBox.Show($"Đăng nhập thành công!\nMã NV: {idNV}\nHọ tên: {tenNV}\nChức vụ: {chucVu}\nQuyền: Cho phép truy cập Phân hệ Nhân Sự.",
                                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    this.Hide();

                                    // Gán tài khoản hiện tại vào BLL của Nhân sự
                                    HR_Management.BLL.HeThongBLL.CurrentUser = new HR_Management.DTO.HeThongDTO
                                    {
                                        ID_NV = idNV,
                                        TenNV = tenNV,
                                        VaiTro = string.IsNullOrEmpty(chucVu) ? vaiTro : chucVu,
                                        TenDangNhap = tenDangNhap
                                    };

                                    // Mở trực tiếp FormMain của HR_Management
                                    using (HR_Management.GUI.FormMain frmNhanSu = new HR_Management.GUI.FormMain())
                                    {
                                        frmNhanSu.ShowDialog();
                                    }

                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show($"Tài khoản của nhân viên [{tenNV}] (Chức vụ: {chucVu}) không có quyền truy cập vào Phân hệ Nhân Sự!",
                                                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Phân hệ {_targetPhanHe} hiện đang trong quá trình phát triển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            // Sai tên đăng nhập hoặc mật khẩu
                            MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMatKhau.Clear();
                            txtMatKhau.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối CSDL Neon: " + ex.Message, "Lỗi PostgreSQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool KiemTraQuyenBanHang(string chucVu, string vaiTro)
        {
            if (string.IsNullOrEmpty(chucVu)) chucVu = "";
            if (string.IsNullOrEmpty(vaiTro)) vaiTro = "";

            string cvLower = chucVu.Trim().ToLower();
            string vtLower = vaiTro.Trim().ToLower();

            bool laAdmin = cvLower.Contains("admin") || vtLower.Contains("quản trị") || vtLower.Contains("admin");
            if (laAdmin) return true;

            if (cvLower.Contains("kho") ||
                cvLower.Contains("kế toán") ||
                cvLower.Contains("nhân sự") ||
                cvLower.Contains("giao hàng") ||
                cvLower.Contains("logistic"))
            {
                return false;
            }

            bool laNhanVienBanHang = cvLower.Equals("nhân viên bán hàng") ||
                                     cvLower.Equals("nhân viên kinh doanh") ||
                                     cvLower.Equals("quản lý bán hàng") ||
                                     cvLower.Equals("trưởng phòng bán hàng");

            return laNhanVienBanHang;
        }

        private bool KiemTraQuyenNhanSu(string chucVu, string vaiTro)
        {
            if (string.IsNullOrEmpty(chucVu)) chucVu = "";
            if (string.IsNullOrEmpty(vaiTro)) vaiTro = "";

            string cvLower = chucVu.Trim().ToLower();
            string vtLower = vaiTro.Trim().ToLower();

            bool laAdmin = cvLower.Contains("admin") || vtLower.Contains("quản trị") || vtLower.Contains("admin");
            if (laAdmin) return true;

            if (cvLower.Contains("kho") ||
                cvLower.Contains("kế toán") ||
                cvLower.Contains("bán hàng") ||
                cvLower.Contains("giao hàng") ||
                cvLower.Contains("logistic"))
            {
                return false;
            }

            bool laNhanVienNhanSu = cvLower.Equals("nhân viên nhân sự") ||
                                    cvLower.Equals("quản lý nhân sự") ||
                                    cvLower.Equals("trưởng phòng nhân sự") ||
                                    vtLower.Contains("nhân sự");

            return laNhanVienNhanSu;
        }
        private void chkHienThiMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            txtMatKhau.UseSystemPasswordChar = !chkHienThiMatKhau.Checked;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}