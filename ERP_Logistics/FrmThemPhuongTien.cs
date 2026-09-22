using System;
using Npgsql;
using System.Windows.Forms;
using ERP.DAL;

namespace ERP
{
    public partial class FrmThemPhuongTien : Form
    {
        private string connectionString = DatabaseConfig.GetConnectionString();

        public FrmThemPhuongTien()
        {
            InitializeComponent();
        }

        private void FrmThemPhuongTien_Load(object sender, EventArgs e)
        {
            // Mặc định chọn sẵn trạng thái và kích hoạt khi mở form
            if (cmbTrangThaiXe.Items.Count > 0)
                cmbTrangThaiXe.SelectedIndex = 0;

            chkKichHoat.Checked = true;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra các trường bắt buộc không được để trống
            if (string.IsNullOrWhiteSpace(txtBienSoXe.Text) ||
                string.IsNullOrWhiteSpace(txtLoaiXe.Text) ||
                string.IsNullOrWhiteSpace(txtTaiTrong.Text) ||
                string.IsNullOrWhiteSpace(txtTenTaiXe.Text) ||
                cmbTrangThaiXe.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ tất cả các thông tin phương tiện!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string bienSoXe = txtBienSoXe.Text.Trim();
            string loaiXe = txtLoaiXe.Text.Trim();
            string tenTaiXe = txtTenTaiXe.Text.Trim();
            string trangThaiXe = cmbTrangThaiXe.SelectedItem.ToString();
            bool kichHoat = chkKichHoat.Checked;

            // 2. Kiểm tra định dạng Tải trọng phải là số hợp lệ
            if (!decimal.TryParse(txtTaiTrong.Text.Trim(), out decimal taiTrong) || taiTrong <= 0)
            {
                MessageBox.Show("Tải trọng phải là một số lớn hơn 0!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaiTrong.Focus();
                txtTaiTrong.SelectAll();
                return;
            }

            // 3. Câu lệnh SQL INSERT
            string query = @"INSERT INTO PhuongTien (BienSoXe, LoaiXe, TaiTrong, TenTaiXe, TrangThaiXe, KichHoat) 
                             VALUES (@BienSoXe, @LoaiXe, @TaiTrong, @TenTaiXe, @TrangThaiXe, @KichHoat)";

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@BienSoXe", bienSoXe);
                    cmd.Parameters.AddWithValue("@LoaiXe", loaiXe);
                    cmd.Parameters.AddWithValue("@TaiTrong", taiTrong);
                    cmd.Parameters.AddWithValue("@TenTaiXe", tenTaiXe);
                    cmd.Parameters.AddWithValue("@TrangThaiXe", trangThaiXe);
                    cmd.Parameters.AddWithValue("@KichHoat", kichHoat);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Thêm phương tiện thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (PostgresException ex)
                {
                    // Lỗi mã 2627: Trùng khóa chính (Biển số xe đã tồn tại)
                    if (ex.SqlState == "23505")
                    {
                        MessageBox.Show("Biển số xe này đã tồn tại trong hệ thống. Vui lòng nhập biển số khác!", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtBienSoXe.Focus();
                        txtBienSoXe.SelectAll();
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


