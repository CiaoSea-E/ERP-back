using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ERP.BLL;
using ERP.DTO;

namespace ERP
{
    public partial class FrmThemDonVanChuyen : Form
    {
        private readonly DonVanChuyenBLL bll = new DonVanChuyenBLL();

        public FrmThemDonVanChuyen()
        {
            InitializeComponent();
        }

        private void FrmThemDonVanChuyen_Load(object sender, EventArgs e)
        {
            dtpThoiGianKhoiHanh.Value = DateTime.Now.AddHours(1); // Mặc định 1 tiếng sau thời điểm hiện tại
            txtTrangThai.Text = "Khởi tạo";

            LoadDanhMuc();
        }

        private void LoadDanhMuc()
        {
            try
            {
                // 1. Nạp danh sách điểm vận chuyển
                List<string> dsDVC = bll.LayDanhSachDVC();
                cboMaDVC.Items.Clear();
                foreach (string dvc in dsDVC)
                {
                    cboMaDVC.Items.Add(dvc);
                }
                if (cboMaDVC.Items.Count > 0) cboMaDVC.SelectedIndex = 0;

                // 2. Nạp danh sách xe khả dụng (Đang rảnh / Sẵn sàng)
                List<string> dsXe = bll.LayDanhSachXeKhaDung();
                cboBienSoXe.Items.Clear();
                foreach (string xe in dsXe)
                {
                    cboBienSoXe.Items.Add(xe);
                }
                if (cboBienSoXe.Items.Count > 0) cboBienSoXe.SelectedIndex = 0;

                // 3. Nạp danh sách sản phẩm (Mã hàng kèm Tên hàng)
                List<SanPhamComboItem> dsSP = bll.LayDanhSachSanPhamWithTen();
                cboSanPham.Items.Clear();
                foreach (SanPhamComboItem sp in dsSP)
                {
                    cboSanPham.Items.Add(sp);
                }
                if (cboSanPham.Items.Count > 0) cboSanPham.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string maDon = txtIDDonVC.Text.Trim();
            string maDVC = cboMaDVC.SelectedItem?.ToString();
            string bienSo = cboBienSoXe.SelectedItem?.ToString();
            string sanPham = (cboSanPham.SelectedItem as SanPhamComboItem)?.ID_SP ?? cboSanPham.SelectedItem?.ToString();
            string strSoLuong = txtSoLuongGiao.Text.Trim();
            DateTime thoiGianGiao = dtpThoiGianKhoiHanh.Value;

            // ==========================================================
            // LUỒNG NGOẠI LỆ A1: Bỏ trống thông tin bắt buộc
            // ==========================================================
            if (string.IsNullOrWhiteSpace(maDon))
            {
                MessageBox.Show("Vui lòng nhập Mã đơn vận chuyển (Mã chuyến)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIDDonVC.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(maDVC))
            {
                MessageBox.Show("Vui lòng chọn Điểm giao nhận (Tuyến đường)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaDVC.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(bienSo))
            {
                MessageBox.Show("Vui lòng chọn Phương tiện / Đơn vị vận chuyển!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboBienSoXe.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(sanPham))
            {
                MessageBox.Show("Vui lòng chọn Hàng hóa / Sản phẩm!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboSanPham.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(strSoLuong) || !int.TryParse(strSoLuong, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng giao phải là số nguyên dương lớn hơn 0!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoLuongGiao.Focus();
                txtSoLuongGiao.SelectAll();
                return;
            }

            // ==========================================================
            // LUỒNG NGOẠI LỆ A3: Sai định dạng thời gian
            // Thời gian giao dự kiến/khởi hành nhỏ hơn thời gian hiện tại
            // ==========================================================
            if (thoiGianGiao < DateTime.Now.AddMinutes(-2)) // Cho phép sai lệch 2 phút thao tác form
            {
                MessageBox.Show("Thời gian khởi hành / giao dự kiến không được nhỏ hơn thời gian hiện tại!", "Sai định dạng thời gian", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpThoiGianKhoiHanh.Focus();
                return;
            }

            // ==========================================================
            // LUỒNG NGOẠI LỆ A2: Trùng lặp mã chuyến xe / mã đơn
            // ==========================================================
            try
            {
                DonVanChuyen tonTai = bll.LayDonTheoID(maDon);
                if (tonTai != null)
                {
                    MessageBox.Show($"Mã chuyến vận hàng '{maDon}' đã tồn tại trong hệ thống. Vui lòng nhập mã khác!", "Trùng lặp mã chuyến xe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtIDDonVC.Focus();
                    txtIDDonVC.SelectAll();
                    return;
                }
            }
            catch
            {
                // Bỏ qua nếu có lỗi đọc tạm thời, sẽ được bll.ThemDon bắt lại
            }

            // ==========================================================
            // LUỒNG CHÍNH: Lưu dữ liệu
            // ==========================================================
            try
            {
                DonVanChuyen don = new DonVanChuyen
                {
                    ID_DonVC = maDon,
                    MaDVC = maDVC,
                    BienSoXe = bienSo,
                    ID_SP = sanPham,
                    SoLuongGiao = soLuong,
                    ThoiGianKhoiHanh = thoiGianGiao,
                    TrangThaiDon = "Khởi tạo"
                };

                bll.ThemDon(don);

                MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu đơn vận chuyển: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
