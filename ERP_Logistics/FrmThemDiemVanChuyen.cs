using System;
using Npgsql;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ERP.DAL;

namespace ERP
{
    public partial class FrmThemDiemVanChuyen : Form
    {
        private string connectionString = DatabaseConfig.GetConnectionString();

        public FrmThemDiemVanChuyen()
        {
            InitializeComponent();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra các trường dữ liệu bắt buộc (Not Null)
            if (string.IsNullOrWhiteSpace(txtMaDVC.Text) ||
                string.IsNullOrWhiteSpace(txtTenDVC.Text) ||
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

            string maDVC = txtMaDVC.Text.Trim();
            string tenDVC = txtTenDVC.Text.Trim();
            string diaChiDVC = txtDiaChiDVC.Text.Trim();
            string tenNguoiDaiDien = txtTenNguoiDaiDien.Text.Trim();

            // 3. Câu lệnh SQL INSERT vào bảng DiemVanChuyen
            string query = @"INSERT INTO DiemVanChuyen 
                             (MaDVC, TenDVC, DiaChiDVC, SDT_DVC, TenNguoiDaiDien) 
                             VALUES 
                             (@MaDVC, @TenDVC, @DiaChiDVC, @SDT_DVC, @TenNguoiDaiDien)";

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    // Truyền tham số an toàn chống SQL Injection
                    cmd.Parameters.AddWithValue("@MaDVC", maDVC);
                    cmd.Parameters.AddWithValue("@TenDVC", tenDVC);
                    cmd.Parameters.AddWithValue("@DiaChiDVC", diaChiDVC);
                    cmd.Parameters.AddWithValue("@SDT_DVC", sdt);
                    cmd.Parameters.AddWithValue("@TenNguoiDaiDien", tenNguoiDaiDien);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Thêm điểm vận chuyển thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (PostgresException ex)
                {
                    if (ex.SqlState == "23505") // Lỗi trùng khóa chính (Primary Key)
                    {
                        MessageBox.Show($"Mã điểm vận chuyển '{maDVC}' đã tồn tại trong hệ thống. Vui lòng nhập mã khác!", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Lỗi cơ sở dữ liệu: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


