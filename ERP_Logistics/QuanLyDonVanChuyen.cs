using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ERP.BLL;
using ERP.DTO;

namespace ERP
{
    public partial class QuanLyDonVanChuyen : Form
    {
        private readonly DonVanChuyenBLL bll = new DonVanChuyenBLL();
        public Button btnExport;

        public QuanLyDonVanChuyen()
        {
            InitializeComponent();
        }

        private void QuanLyDonVanChuyen_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            InitExportButton();
            KhoiTaoCotBang();

            // 1. Gọi BLL.LayDanhSach() → bind vào DataGridView
            LoadDataDonVanChuyen();

            // 2. Nạp trạng thái vào ComboBox lọc
            LoadTrangThaiComboBox();
        }

        private void InitExportButton()
        {
            if (btnExport != null) return;

            // Nút Xuất File trên thanh công cụ
            btnExport = new Button();
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.BackColor = Color.FromArgb(108, 117, 125);
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExport.ForeColor = Color.White;
            btnExport.Location = new Point(370, 10);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(140, 35);
            btnExport.TabIndex = 9;
            btnExport.Text = "📥 Xuất File";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += new EventHandler(this.btnExport_Click);
            pnlActionTool.Controls.Add(btnExport);
        }

        private void KhoiTaoCotBang()
        {
            dgvData.Columns.Clear();
            dgvData.AutoGenerateColumns = false;

            // 1. Mã Đơn Vận Chuyển (ID_DonVC)
            DataGridViewTextBoxColumn colID = new DataGridViewTextBoxColumn();
            colID.Name = "colID_DonVC";
            colID.HeaderText = "MÃ ĐƠN VC";
            colID.DataPropertyName = "ID_DonVC";
            colID.FillWeight = 14;
            colID.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colID.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            colID.DefaultCellStyle.ForeColor = Color.FromArgb(13, 110, 253);
            dgvData.Columns.Add(colID);

            // 2. Biển Số Xe (BienSoXe)
            DataGridViewTextBoxColumn colXe = new DataGridViewTextBoxColumn();
            colXe.Name = "colBienSoXe";
            colXe.HeaderText = "BIỂN SỐ XE";
            colXe.DataPropertyName = "BienSoXe";
            colXe.FillWeight = 14;
            colXe.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colXe.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.Columns.Add(colXe);

            // 3. Mã Điểm Vận Chuyển (MaDVC)
            DataGridViewTextBoxColumn colMaDVC = new DataGridViewTextBoxColumn();
            colMaDVC.Name = "colMaDVC";
            colMaDVC.HeaderText = "MÃ ĐIỂM VC";
            colMaDVC.DataPropertyName = "MaDVC";
            colMaDVC.FillWeight = 16;
            colMaDVC.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colMaDVC.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.Columns.Add(colMaDVC);

            // 4. Mã Sản Phẩm (ID_SP)
            DataGridViewTextBoxColumn colSP = new DataGridViewTextBoxColumn();
            colSP.Name = "colID_SP";
            colSP.HeaderText = "MÃ SP";
            colSP.DataPropertyName = "ID_SP";
            colSP.FillWeight = 10;
            colSP.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colSP.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.Columns.Add(colSP);

            // 4.1 Tên Hàng Hóa (TenHang)
            DataGridViewTextBoxColumn colTenHang = new DataGridViewTextBoxColumn();
            colTenHang.Name = "colTenHang";
            colTenHang.HeaderText = "TÊN HÀNG HÓA";
            colTenHang.DataPropertyName = "TenHang";
            colTenHang.FillWeight = 20;
            colTenHang.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            colTenHang.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvData.Columns.Add(colTenHang);

            // 5. Số Lượng Giao (SoLuongGiao)
            DataGridViewTextBoxColumn colSL = new DataGridViewTextBoxColumn();
            colSL.Name = "colSoLuongGiao";
            colSL.HeaderText = "SL GIAO";
            colSL.DataPropertyName = "SoLuongGiao";
            colSL.FillWeight = 10;
            colSL.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colSL.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.Columns.Add(colSL);

            // 6. Thời Gian Khởi Hành (ThoiGianKhoiHanh)
            DataGridViewTextBoxColumn colNgay = new DataGridViewTextBoxColumn();
            colNgay.Name = "colThoiGianKhoiHanh";
            colNgay.HeaderText = "THỜI GIAN KHỞI HÀNH";
            colNgay.DataPropertyName = "ThoiGianKhoiHanh";
            colNgay.FillWeight = 18;
            colNgay.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNgay.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNgay.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            dgvData.Columns.Add(colNgay);

            // 7. Trạng Thái Đơn (TrangThaiDon)
            DataGridViewTextBoxColumn colTrangThai = new DataGridViewTextBoxColumn();
            colTrangThai.Name = "colTrangThaiDon";
            colTrangThai.HeaderText = "TRẠNG THÁI";
            colTrangThai.DataPropertyName = "TrangThaiDon";
            colTrangThai.FillWeight = 14;
            colTrangThai.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTrangThai.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTrangThai.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvData.Columns.Add(colTrangThai);

            dgvData.AllowUserToResizeColumns = true;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ==========================================
        // TẢI DỮ LIỆU VÀ NẠP DANH MỤC
        // ==========================================

        private void LoadDataDonVanChuyen()
        {
            try
            {
                List<DonVanChuyen> list = bll.LayDanhSach();
                BindData(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách đơn vận chuyển: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindData(List<DonVanChuyen> list)
        {
            dgvData.DataSource = null;
            dgvData.DataSource = list;

            // Định dạng màu sắc trực quan theo từng trạng thái
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (row.Cells["colTrangThaiDon"].Value != null)
                {
                    string tt = row.Cells["colTrangThaiDon"].Value.ToString();
                    if (tt == "Khởi tạo")
                    {
                        row.Cells["colTrangThaiDon"].Style.ForeColor = Color.FromArgb(13, 110, 253);
                    }
                    else if (tt == "Đang vận chuyển")
                    {
                        row.Cells["colTrangThaiDon"].Style.ForeColor = Color.FromArgb(255, 152, 0);
                    }
                    else if (tt == "Hoàn thành")
                    {
                        row.Cells["colTrangThaiDon"].Style.ForeColor = Color.FromArgb(40, 167, 69);
                    }
                    else if (tt == "Đã hủy")
                    {
                        row.Cells["colTrangThaiDon"].Style.ForeColor = Color.FromArgb(220, 53, 69);
                    }
                }
            }
        }

        private void LoadTrangThaiComboBox()
        {
            string[] trangThais = { "Khởi tạo", "Đang vận chuyển", "Hoàn thành", "Đã hủy" };

            cmbTrangThai.Items.Clear();
            cmbTrangThai.Items.Add("Tất cả trạng thái");
            cmbTrangThai.Items.AddRange(trangThais);
            cmbTrangThai.SelectedIndex = 0;
        }

        // ==========================================
        // CÁC NÚT HÀNH ĐỘNG CRUD
        // ==========================================

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmThemDonVanChuyen frm = new FrmThemDonVanChuyen();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadDataDonVanChuyen();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow != null)
            {
                string id = dgvData.CurrentRow.Cells["colID_DonVC"].Value?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    FrmSuaDonVanChuyen frm = new FrmSuaDonVanChuyen(id);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        LoadDataDonVanChuyen();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn vận chuyển cần chỉnh sửa trên bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow != null)
            {
                string id = dgvData.CurrentRow.Cells["colID_DonVC"].Value?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn vận chuyển '{id}' không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        try
                        {
                            bll.XoaDon(id);
                            MessageBox.Show("Xóa đơn vận chuyển thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataDonVanChuyen();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi xóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn vận chuyển cần xóa trên bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow != null)
            {
                string id = dgvData.CurrentRow.Cells["colID_DonVC"].Value?.ToString();
                string statusHienTai = dgvData.CurrentRow.Cells["colTrangThaiDon"].Value?.ToString();

                if (!string.IsNullOrEmpty(id))
                {
                    using (FrmCapNhatTrangThai frm = new FrmCapNhatTrangThai(id, statusHienTai))
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadDataDonVanChuyen();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn vận chuyển cần cập nhật trạng thái!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // TÌM KIẾM VÀ LỌC DỮ LIỆU
        // ==========================================

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LocDuLieu();
        }

        private void cmbTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocDuLieu();
        }

        private void LocDuLieu()
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword == "🔍 Tìm kiếm theo Mã đơn, Mã xe, Địa điểm...") keyword = string.Empty;

            try
            {
                List<DonVanChuyen> list = string.IsNullOrEmpty(keyword)
                    ? bll.LayDanhSach()
                    : bll.TimKiem(keyword);

                if (cmbTrangThai.SelectedIndex > 0)
                {
                    string selectedStatus = cmbTrangThai.SelectedItem.ToString();
                    list = list.FindAll(d => d.TrangThaiDon == selectedStatus);
                }

                BindData(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "🔍 Tìm kiếm theo Mã đơn, Mã xe, Địa điểm...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "🔍 Tìm kiếm theo Mã đơn, Mã xe, Địa điểm...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        // ==========================================
        // NÚT XUẤT FILE
        // ==========================================

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files|*.csv|Excel Files|*.xlsx";
                sfd.FileName = "DonVanChuyen_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();

                        // Tiêu đề cột
                        List<string> headers = new List<string>();
                        foreach (DataGridViewColumn col in dgvData.Columns)
                        {
                            if (col.Visible)
                            {
                                headers.Add("\"" + col.HeaderText.Replace("\"", "\"\"") + "\"");
                            }
                        }
                        sb.AppendLine(string.Join(",", headers));

                        // Dữ liệu dòng
                        foreach (DataGridViewRow row in dgvData.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                List<string> cells = new List<string>();
                                foreach (DataGridViewColumn col in dgvData.Columns)
                                {
                                    if (col.Visible)
                                    {
                                        object val = row.Cells[col.Index].Value;
                                        string strVal = val != null ? val.ToString() : "";
                                        cells.Add("\"" + strVal.Replace("\"", "\"\"") + "\"");
                                    }
                                }
                                sb.AppendLine(string.Join(",", cells));
                            }
                        }

                        // Ghi file kèm BOM UTF-8 để mở tiếng Việt không lỗi font
                        System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Xuất file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Xuất file thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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

        private void btnQuanLyTraHang_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuanLyTraHang frmTraHang = new QuanLyTraHang();
            frmTraHang.ShowDialog();
            this.Close();
        }
    }
}