using System;
using Npgsql;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ERP.DAL;

namespace ERP
{
    public partial class FrmSuaDiemVanChuyen : Form
    {
        private string connectionString = DatabaseConfig.GetConnectionString();
        private string maDvcToEdit;

        // Constructor nhận vào Mã DVC cần sửa từ form danh sách
        public FrmSuaDiemVanChuyen(string maDvc)
        {
            InitializeComponent();
            maDvcToEdit = maDvc;
            txtMaDVC.Text = maDvcToEdit;
            txtMaDVC.ReadOnly = true; // Khóa mã DVC không cho sửa khóa chính
            txtMaDVC.BackColor = System.Drawing.SystemColors.Control;
        }

        private void FrmSuaDiemVanChuyen_Load(object sender, EventArgs e)
        {
            LoadThongTinDiemVanChuyen();
        }

        // Đổ dữ liệu cũ lên form để chỉnh sửa
        private void LoadThongTinDiemVanChuyen()
        {
            string query = "SELECT * FROM DiemVanChuyen WHERE MaDVC = @MaDVC";
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MaDVC", maDvcToEdit);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTenDVC.Text = reader["TenDVC"].ToString();
                            txtDiaChiDVC.Text = reader["DiaChiDVC"].ToString();
                            txtSDT_DVC.Text = reader["SDT_DVC"].ToString();
                            txtTenNguoiDaiDien.Text = reader["TenNguoiDaiDien"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải thông tin điểm vận chuyển: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(txtTenDVC.Text) ||
                string.IsNullOrWhiteSpace(txtDiaChiDVC.Text) ||
                string.IsNullOrWhiteSpace(txtSDT_DVC.Text) ||
                string.IsNullOrWhiteSpace(txtTenNguoiDaiDien.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ tất cả các thông tin bắt buộc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sdt = txtSDT_DVC.Text.Trim();

            // 2. Kiểm tra số điện thoại phải đúng 10 chữ số
            if (!Regex.IsMatch(sdt, @"^\d{10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! Số điện thoại phải bao gồm đúng 10 chữ số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT_DVC.Focus();
                txtSDT_DVC.SelectAll();
                return;
            }

            string tenDVC = txtTenDVC.Text.Trim();
            string diaChiDVC = txtDiaChiDVC.Text.Trim();
            string tenNguoiDaiDien = txtTenNguoiDaiDien.Text.Trim();

            // 3. Câu lệnh SQL UPDATE
            string query = @"UPDATE DiemVanChuyen SET 
                             TenDVC = @TenDVC, 
                             DiaChiDVC = @DiaChiDVC, 
                             SDT_DVC = @SDT_DVC, 
                             TenNguoiDaiDien = @TenNguoiDaiDien 
                             WHERE MaDVC = @MaDVC";

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@MaDVC", maDvcToEdit);
                    cmd.Parameters.AddWithValue("@TenDVC", tenDVC);
                    cmd.Parameters.AddWithValue("@DiaChiDVC", diaChiDVC);
                    cmd.Parameters.AddWithValue("@SDT_DVC", sdt);
                    cmd.Parameters.AddWithValue("@TenNguoiDaiDien", tenNguoiDaiDien);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Cập nhật điểm vận chuyển thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
