using System;
using Npgsql;
using System.Windows.Forms;
using ERP.DAL;

namespace ERP
{
    public partial class FrmSuaPhuongTien : Form
    {
        private string connectionString = DatabaseConfig.GetConnectionString();
        private string bienSoXeCu;

        // Constructor nhận vào biển số xe cần sửa
        public FrmSuaPhuongTien(string bienSo)
        {
            InitializeComponent();
            bienSoXeCu = bienSo;
        }

        private void FrmSuaPhuongTien_Load(object sender, EventArgs e)
        {
            // Hiển thị biển số xe lên ô textbox nhưng khóa lại (vì là khóa chính không cho đổi trực tiếp)
            txtBienSoXe.Text = bienSoXeCu;
            txtBienSoXe.ReadOnly = true;
            txtBienSoXe.BackColor = System.Drawing.SystemColors.Control;

            // Load dữ liệu hiện tại của xe lên form
            LoadThongTinXe();
        }

        private void LoadThongTinXe()
        {
            string query = "SELECT LoaiXe, TaiTrong, TenTaiXe, TrangThaiXe, KichHoat FROM PhuongTien WHERE BienSoXe = @BienSoXe";

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BienSoXe", bienSoXeCu);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtLoaiXe.Text = reader["LoaiXe"].ToString();
                            txtTaiTrong.Text = reader["TaiTrong"].ToString();
                            txtTenTaiXe.Text = reader["TenTaiXe"].ToString();
                            cmbTrangThaiXe.SelectedItem = reader["TrangThaiXe"].ToString();
                            chkKichHoat.Checked = Convert.ToBoolean(reader["KichHoat"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải thông tin phương tiện: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(txtLoaiXe.Text) ||
                string.IsNullOrWhiteSpace(txtTaiTrong.Text) ||
                string.IsNullOrWhiteSpace(txtTenTaiXe.Text) ||
                cmbTrangThaiXe.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ tất cả các thông tin phương tiện!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loaiXe = txtLoaiXe.Text.Trim();
            string tenTaiXe = txtTenTaiXe.Text.Trim();
            string trangThaiXe = cmbTrangThaiXe.SelectedItem.ToString();
            bool kichHoat = chkKichHoat.Checked;

            // 2. Kiểm tra định dạng Tải trọng
            if (!decimal.TryParse(txtTaiTrong.Text.Trim(), out decimal taiTrong) || taiTrong <= 0)
            {
                MessageBox.Show("Tải trọng phải là một số lớn hơn 0!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaiTrong.Focus();
                txtTaiTrong.SelectAll();
                return;
            }

            // 3. Câu lệnh SQL UPDATE
            string query = @"UPDATE PhuongTien 
                             SET LoaiXe = @LoaiXe, TaiTrong = @TaiTrong, TenTaiXe = @TenTaiXe, TrangThaiXe = @TrangThaiXe, KichHoat = @KichHoat 
                             WHERE BienSoXe = @BienSoXe";

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@LoaiXe", loaiXe);
                    cmd.Parameters.AddWithValue("@TaiTrong", taiTrong);
                    cmd.Parameters.AddWithValue("@TenTaiXe", tenTaiXe);
                    cmd.Parameters.AddWithValue("@TrangThaiXe", trangThaiXe);
                    cmd.Parameters.AddWithValue("@KichHoat", kichHoat);
                    cmd.Parameters.AddWithValue("@BienSoXe", bienSoXeCu);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Cập nhật thông tin phương tiện thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi cơ sở dữ liệu: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
