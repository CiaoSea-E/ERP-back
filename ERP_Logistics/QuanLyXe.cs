using System;
using System.Data;
using Npgsql;
using System.Drawing;
using System.Windows.Forms;
using ERP.DAL;

namespace ERP
{
    public partial class QuanLyXe : Form
    {
        private string connectionString = DatabaseConfig.GetConnectionString();
        private DataTable dtXe;

        public QuanLyXe()
        {
            InitializeComponent();
        }

        private void QuanLyXe_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            if (cboVehicleType.Items.Count > 0) cboVehicleType.SelectedIndex = 0;
            if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;

            KhoiTaoCotBang();
            LoadDataXe();
        }

        private void KhoiTaoCotBang()
        {
            dgvData.Columns.Clear();
            dgvData.AutoGenerateColumns = false;

            // 1. Biển Số Xe (Khóa chính BienSoXe)
            DataGridViewTextBoxColumn colBienSo = new DataGridViewTextBoxColumn();
            colBienSo.Name = "colBienSoXe";
            colBienSo.HeaderText = "BIỂN SỐ XE";
            colBienSo.DataPropertyName = "BienSoXe";
            colBienSo.FillWeight = 15;
            colBienSo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBienSo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBienSo.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            colBienSo.DefaultCellStyle.ForeColor = Color.FromArgb(13, 110, 253);
            dgvData.Columns.Add(colBienSo);

            // 2. Loại Xe (LoaiXe)
            DataGridViewTextBoxColumn colLoaiXe = new DataGridViewTextBoxColumn();
            colLoaiXe.Name = "colLoaiXe";
            colLoaiXe.HeaderText = "LOẠI XE";
            colLoaiXe.DataPropertyName = "LoaiXe";
            colLoaiXe.FillWeight = 18;
            colLoaiXe.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvData.Columns.Add(colLoaiXe);

            // 3. Tải Trọng (TaiTrong)
            DataGridViewTextBoxColumn colTaiTrong = new DataGridViewTextBoxColumn();
            colTaiTrong.Name = "colTaiTrong";
            colTaiTrong.HeaderText = "TẢI TRỌNG (TẤN)";
            colTaiTrong.DataPropertyName = "TaiTrong";
            colTaiTrong.FillWeight = 15;
            colTaiTrong.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTaiTrong.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTaiTrong.DefaultCellStyle.Format = "N2";
            dgvData.Columns.Add(colTaiTrong);

            // 4. Tên Tài Xế (TenTaiXe)
            DataGridViewTextBoxColumn colTaiXe = new DataGridViewTextBoxColumn();
            colTaiXe.Name = "colTenTaiXe";
            colTaiXe.HeaderText = "TÊN TÀI XẾ";
            colTaiXe.DataPropertyName = "TenTaiXe";
            colTaiXe.FillWeight = 22;
            colTaiXe.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvData.Columns.Add(colTaiXe);

            // 5. Trạng Thái Xe (TrangThaiXe)
            DataGridViewTextBoxColumn colTrangThai = new DataGridViewTextBoxColumn();
            colTrangThai.Name = "colTrangThaiXe";
            colTrangThai.HeaderText = "TRẠNG THÁI";
            colTrangThai.DataPropertyName = "TrangThaiXe";
            colTrangThai.FillWeight = 18;
            colTrangThai.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTrangThai.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTrangThai.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvData.Columns.Add(colTrangThai);

            // 6. Kích Hoạt (KichHoat - Hiển thị dạng CheckBox hoặc Text)
            DataGridViewCheckBoxColumn colKichHoat = new DataGridViewCheckBoxColumn();
            colKichHoat.Name = "colKichHoat";
            colKichHoat.HeaderText = "KÍCH HOẠT";
            colKichHoat.DataPropertyName = "KichHoat";
            colKichHoat.FillWeight = 12;
            colKichHoat.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colKichHoat.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.Columns.Add(colKichHoat);

            dgvData.AllowUserToResizeColumns = true;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadDataXe()
        {
            // Truy vấn lấy dữ liệu chính xác từ bảng PhuongTien
            string query = "SELECT BienSoXe, LoaiXe, TaiTrong, TenTaiXe, TrangThaiXe, KichHoat FROM PhuongTien ORDER BY BienSoXe ASC";

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                    dtXe = new DataTable();
                    da.Fill(dtXe);

                    dgvData.DataSource = dtXe;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải danh sách phương tiện: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // TÌM KIẾM VÀ LỌC DỮ LIỆU
        // ==========================================

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LocDuLieu();
        }

        private void cboVehicleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocDuLieu();
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocDuLieu();
        }

        private void LocDuLieu()
        {
            if (dtXe == null) return;

            string keyword = txtSearch.Text.Trim().Replace("'", "''");
            if (keyword == "🔍 Tìm kiếm chủ xe, biển số, SĐT..." || keyword == "🔍 Tìm kiếm phương tiện, tài xế...") keyword = "";

            DataView dv = dtXe.DefaultView;
            string filter = "1=1";

            if (!string.IsNullOrEmpty(keyword))
            {
                filter += $" AND (BienSoXe LIKE '%{keyword}%' OR LoaiXe LIKE '%{keyword}%' OR TenTaiXe LIKE '%{keyword}%')";
            }

            // Lọc theo loại xe nếu ComboBox chọn loại xe cụ thể
            if (cboVehicleType.SelectedIndex > 0)
            {
                string selectedType = cboVehicleType.SelectedItem.ToString();
                filter += $" AND LoaiXe = '{selectedType}'";
            }

            // Lọc theo trạng thái xe
            if (cboStatus.SelectedIndex > 0)
            {
                string selectedStatus = cboStatus.SelectedItem.ToString();
                filter += $" AND TrangThaiXe = '{selectedStatus}'";
            }

            dv.RowFilter = filter;
            dgvData.DataSource = dv;
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "🔍 Tìm kiếm chủ xe, biển số, SĐT..." || txtSearch.Text == "🔍 Tìm kiếm phương tiện, tài xế...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "🔍 Tìm kiếm phương tiện, tài xế...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        // ==========================================
        // THÊM, SỬA, XÓA PHƯƠNG TIỆN
        // ==========================================

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmThemPhuongTien frmThem = new FrmThemPhuongTien();

            // Nếu thêm dữ liệu thành công (DialogResult.OK), hệ thống sẽ tự động load lại bảng dữ liệu
            if (frmThem.ShowDialog() == DialogResult.OK)
            {
                LoadDataXe();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow != null)
            {
                string bienSo = dgvData.CurrentRow.Cells["colBienSoXe"].Value?.ToString();

                if (!string.IsNullOrEmpty(bienSo))
                {
                    // Gọi Form Sửa Phương Tiện và truyền biển số xe vào
                    FrmSuaPhuongTien frmSua = new FrmSuaPhuongTien(bienSo);

                    // Nếu cập nhật thành công, load lại danh sách phương tiện
                    if (frmSua.ShowDialog() == DialogResult.OK)
                    {
                        LoadDataXe();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn xe cần chỉnh sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem người dùng đã chọn dòng nào trên bảng chưa
            if (dgvData.CurrentRow != null)
            {
                // Lấy Biển số xe từ dòng đang chọn
                string bienSoXe = dgvData.CurrentRow.Cells["colBienSoXe"].Value?.ToString();
                string loaiXe = dgvData.CurrentRow.Cells["colLoaiXe"].Value?.ToString();

                // 2. Hiển thị hộp thoại xác nhận trước khi xóa
                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa phương tiện biển số '{bienSoXe}' ({loaiXe}) không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM PhuongTien WHERE BienSoXe = @BienSoXe";

                    using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@BienSoXe", bienSoXe);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Xóa phương tiện thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadDataXe();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy phương tiện cần xóa trong cơ sở dữ liệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (PostgresException ex)
                        {
                            // Lỗi mã 547: Bị vướng khóa ngoại (Foreign Key) nếu phương tiện này đã được gán vào đơn vận chuyển hoặc bảng khác
                            if ((ex.SqlState == "23503" || ex.SqlState == "23514"))
                            {
                                MessageBox.Show("Không thể xóa phương tiện này vì đã phát sinh dữ liệu liên quan (như đơn vận chuyển) trong hệ thống.", "Lỗi ràng buộc dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            }
            else
            {
                MessageBox.Show("Vui lòng chọn phương tiện cần xóa trên bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // ĐIỀU HƯỚNG SIDEBAR
        // ==========================================

        private void btnQuanLyNhaCungCap_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuanLyNhaCungCap frmNCC = new QuanLyNhaCungCap();
            frmNCC.ShowDialog();
            this.Close();
        }

        private void btnQuanLyDiemVanChuyen_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuanLyDiemVanChuyen frmDVC = new QuanLyDiemVanChuyen();
            frmDVC.ShowDialog();
            this.Close();
        }

        private void btnQuanLyTraHang_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuanLyTraHang frmTraHang = new QuanLyTraHang();
            frmTraHang.ShowDialog();
            this.Close();
        }

        private void btnQuanLyDonVanChuyen_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuanLyDonVanChuyen frmDonVC = new QuanLyDonVanChuyen();
            frmDonVC.ShowDialog();
            this.Close();
        }
    }
}


