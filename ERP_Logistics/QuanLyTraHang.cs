using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ERP.BLL;
using ERP.DTO;

namespace ERP
{
    public partial class QuanLyTraHang : Form
    {
        private readonly PhieuTraHangBLL bll = new PhieuTraHangBLL();

        public QuanLyTraHang()
        {
            InitializeComponent();
        }

        private void QuanLyTraHang_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            dgvData.AutoGenerateColumns = false;

            LoadComboBoxData();
            LoadDataPhieuTraHang();
            dtpTuNgay.ValueChanged += (s, ev) => TimKiem();
            dtpDenNgay.ValueChanged += (s, ev) => TimKiem();
        }

        private void KhoiTaoCotBang()
        {
            dgvData.Columns.Clear();
            dgvData.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn colIDPT = new DataGridViewTextBoxColumn();
            colIDPT.Name = "colID_PhieuTra";
            colIDPT.HeaderText = "MÃ PHIẾU TRẢ";
            colIDPT.DataPropertyName = "ID_PhieuTra";
            colIDPT.FillWeight = 16;
            colIDPT.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIDPT.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIDPT.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            colIDPT.DefaultCellStyle.ForeColor = Color.FromArgb(13, 110, 253);
            dgvData.Columns.Add(colIDPT);

            DataGridViewTextBoxColumn colNgayTra = new DataGridViewTextBoxColumn();
            colNgayTra.Name = "colNgayTra";
            colNgayTra.HeaderText = "NGÀY TRẢ";
            colNgayTra.DataPropertyName = "NgayTra";
            colNgayTra.FillWeight = 20;
            colNgayTra.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNgayTra.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNgayTra.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            dgvData.Columns.Add(colNgayTra);

            // 3. Mã Điểm Vận Chuyển (MaDVC)
            DataGridViewTextBoxColumn colMaDVC = new DataGridViewTextBoxColumn();
            colMaDVC.Name = "colMaDVC";
            colMaDVC.HeaderText = "MÃ ĐIỂM VC";
            colMaDVC.DataPropertyName = "MaDVC";
            colMaDVC.FillWeight = 18;
            colMaDVC.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colMaDVC.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.Columns.Add(colMaDVC);

            // 3.5. Biển Số Xe (BienSoXe)
            DataGridViewTextBoxColumn colBienSoXe = new DataGridViewTextBoxColumn();
            colBienSoXe.Name = "colBienSoXe";
            colBienSoXe.HeaderText = "BIỂN SỐ XE";
            colBienSoXe.DataPropertyName = "BienSoXe";
            colBienSoXe.FillWeight = 18;
            colBienSoXe.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBienSoXe.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.Columns.Add(colBienSoXe);

            // 4. Mã Chi Tiết Yêu Cầu (ID_CTYC)
            DataGridViewTextBoxColumn colIDCTYC = new DataGridViewTextBoxColumn();
            colIDCTYC.Name = "colID_CTYC";
            colIDCTYC.HeaderText = "MÃ CHI TIẾT YC";
            colIDCTYC.DataPropertyName = "ID_CTYC";
            colIDCTYC.FillWeight = 20;
            colIDCTYC.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIDCTYC.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.Columns.Add(colIDCTYC);

            // 5. Trạng Thái (TrangThai)
            DataGridViewTextBoxColumn colTrangThai = new DataGridViewTextBoxColumn();
            colTrangThai.Name = "colTrangThai";
            colTrangThai.HeaderText = "TRẠNG THÁI";
            colTrangThai.DataPropertyName = "TrangThai";
            colTrangThai.FillWeight = 18;
            colTrangThai.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTrangThai.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTrangThai.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvData.Columns.Add(colTrangThai);

            dgvData.AllowUserToResizeColumns = true;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Đổ data vào ComboBox bộ lọc trạng thái
                string[] dsTrangThai = { "Chờ xử lý", "Đang thu hồi", "Đã nhập kho", "Đã hủy" };
                cmbTrangThai.Items.Clear();
                cmbTrangThai.Items.Add("Tất cả trạng thái");
                cmbTrangThai.Items.AddRange(dsTrangThai);
                cmbTrangThai.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataPhieuTraHang()
        {
            try
            {
                List<PhieuTraHang> list = bll.GetAll();
                BindData(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu phiếu trả hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindData(List<PhieuTraHang> list)
        {
            dgvData.DataSource = null;
            dgvData.DataSource = list;

            // Định dạng màu sắc trạng thái
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (row.Cells["colTrangThai"].Value != null)
                {
                    string tt = row.Cells["colTrangThai"].Value.ToString();
                    if (tt == "Chờ xử lý")
                    {
                        row.Cells["colTrangThai"].Style.ForeColor = Color.FromArgb(255, 152, 0); // Cam
                    }
                    else if (tt == "Đang thu hồi" || tt == "Đang lấy hàng")
                    {
                        row.Cells["colTrangThai"].Style.ForeColor = Color.FromArgb(13, 110, 253); // Xanh dương
                    }
                    else if (tt == "Đã nhập kho")
                    {
                        row.Cells["colTrangThai"].Style.ForeColor = Color.FromArgb(40, 167, 69); // Xanh lá
                    }
                    else if (tt == "Đã hủy")
                    {
                        row.Cells["colTrangThai"].Style.ForeColor = Color.FromArgb(220, 53, 69); // Đỏ
                    }
                }
            }
        }

        // ==========================================
        // 2. SỰ KIỆN CLICK TÌM KIẾM
        // ==========================================
        private void TimKiem()
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                if (keyword == "🔍 Tìm kiếm theo Mã phiếu trả, Tên ĐVC..." || keyword == "🔍 Tìm kiếm theo Mã phiếu trả, Mã YC, Khách hàng...")
                {
                    keyword = "";
                }

                DateTime tuNgay = dtpTuNgay.Checked ? dtpTuNgay.Value : DateTime.MinValue;
                DateTime denNgay = dtpDenNgay.Checked ? dtpDenNgay.Value : DateTime.MaxValue;
                string trangThai = cmbTrangThai.SelectedItem?.ToString() ?? "";

                var list = bll.TimKiem(keyword, tuNgay, denNgay, trangThai);
                if (list.Count == 0)
                {
                    // Tránh việc văng popup liên tục khi gõ, ta chỉ reset list
                    dgvData.DataSource = null;
                }
                else
                {
                    BindData(list);
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi Luồng A2
                MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text != "🔍 Tìm kiếm theo Mã phiếu trả, Tên ĐVC..." &&
                txtSearch.Text != "🔍 Tìm kiếm theo Mã phiếu trả, Mã YC, Khách hàng...")
            {
                TimKiem();
            }
        }

        private void cmbTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimKiem();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "🔍 Tìm kiếm theo Mã phiếu trả, Tên ĐVC..." ||
                txtSearch.Text == "🔍 Tìm kiếm theo Mã phiếu trả, Mã YC, Khách hàng...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "🔍 Tìm kiếm theo Mã phiếu trả, Tên ĐVC...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        // ==========================================
        // 3. SỰ KIỆN THÊM / SỬA / XÓA
        // ==========================================

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmPhieuTraHang frm = new FrmPhieuTraHang(null);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadDataPhieuTraHang();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow != null)
            {
                string id = dgvData.CurrentRow.Cells["colID_PhieuTra"].Value?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    FrmPhieuTraHang frm = new FrmPhieuTraHang(id);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        LoadDataPhieuTraHang();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn phiếu trả hàng cần sửa trên bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow != null)
            {
                string id = dgvData.CurrentRow.Cells["colID_PhieuTra"].Value?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu trả [{id}]?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        try
                        {
                            bll.XoaPhieu(id);
                            MessageBox.Show("Xóa phiếu trả hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataPhieuTraHang();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn phiếu trả hàng cần xóa trên bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            // Nút này đã bị xóa/thay thế nhưng nếu Form designer còn bind event thì gọi TimKiem.
            TimKiem();
        }

        // ==========================================
        // ĐIỀU HƯỚNG SIDEBAR
        // ==========================================
        private void btnQuanLyXe_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuanLyXe frmXe = new QuanLyXe();
            frmXe.ShowDialog();
            this.Close();
        }

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

        private void btnQuanLyDonVanChuyen_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuanLyDonVanChuyen frmDonVC = new QuanLyDonVanChuyen();
            frmDonVC.ShowDialog();
            this.Close();
        }
    }
}
