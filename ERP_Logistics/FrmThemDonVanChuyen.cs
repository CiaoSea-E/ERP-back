using System;
using System.Collections.Generic;
using System.Drawing;
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
            UIThemeHelper.ApplyFormStyle(this);
            UIThemeHelper.ApplyActionButton(this.btnLuu, ButtonRole.Primary);
            UIThemeHelper.ApplyActionButton(this.btnHuy, ButtonRole.Secondary);

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

                // 2. Nạp danh sách xe khả dụng kèm tải trọng
                List<XeKhaDungItem> dsXe = bll.LayDanhSachXeKhaDungKemTaiTrong();
                cboBienSoXe.Items.Clear();
                foreach (XeKhaDungItem xe in dsXe)
                {
                    cboBienSoXe.Items.Add(xe);
                }
                if (cboBienSoXe.Items.Count > 0)
                {
                    cboBienSoXe.SelectedIndex = 0;
                    CapNhatHienThiTaiTrongXe();
                }

                // 3. Nạp danh sách sản phẩm (Mã hàng kèm Tên hàng)
                List<SanPhamComboItem> dsSP = bll.LayDanhSachSanPhamWithTen();
                cboSanPham.Items.Clear();
                foreach (SanPhamComboItem sp in dsSP)
                {
                    cboSanPham.Items.Add(sp);
                }
                if (cboSanPham.Items.Count > 0) cboSanPham.SelectedIndex = 0;

                // 4. Nạp danh sách đơn hàng từ Bán Hàng đang chờ giao
                List<DonHangChoGiaoItem> dsDH = bll.LayDanhSachDonHangChoGiao();
                cboDonHang.Items.Clear();
                cboDonHang.Items.Add(new DonHangChoGiaoItem { ID_DH = "", TenKhachHang = "-- Không chọn đơn hàng (Giao tự do) --" });
                foreach (DonHangChoGiaoItem dh in dsDH)
                {
                    cboDonHang.Items.Add(dh);
                }
                cboDonHang.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CapNhatHienThiTaiTrongXe()
        {
            if (cboBienSoXe.SelectedItem is XeKhaDungItem xe)
            {
                lblTaiTrongXe.Text = $"🚛 Tải trọng tối đa: {xe.TaiTrongKg:N0} kg ({xe.LoaiXe})";
                KiemTraQuaTaiThoiGianThuc();
            }
            else
            {
                lblTaiTrongXe.Text = "🚛 Tải trọng xe: Vui lòng chọn xe";
            }
        }

        private void cboBienSoXe_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapNhatHienThiTaiTrongXe();
        }

        private void txtSoLuongGiao_TextChanged(object sender, EventArgs e)
        {
            // Tự động gợi ý trọng lượng = Số lượng * 10kg nếu người dùng chưa nhập tay số khác
            if (int.TryParse(txtSoLuongGiao.Text.Trim(), out int sl) && sl > 0)
            {
                if (string.IsNullOrWhiteSpace(txtTrongLuong.Text) || txtTrongLuong.Tag?.ToString() == "auto")
                {
                    txtTrongLuong.Text = (sl * 10).ToString();
                    txtTrongLuong.Tag = "auto";
                }
            }
            KiemTraQuaTaiThoiGianThuc();
        }

        private void txtTrongLuong_TextChanged(object sender, EventArgs e)
        {
            if (txtTrongLuong.Focused)
            {
                txtTrongLuong.Tag = "manual";
            }
            KiemTraQuaTaiThoiGianThuc();
        }

        private void KiemTraQuaTaiThoiGianThuc()
        {
            if (cboBienSoXe.SelectedItem is XeKhaDungItem xe)
            {
                if (decimal.TryParse(txtTrongLuong.Text.Trim(), out decimal tl) && tl > 0)
                {
                    if (tl > xe.TaiTrongKg)
                    {
                        decimal vuot = tl - xe.TaiTrongKg;
                        lblTaiTrongXe.ForeColor = Color.Red;
                        lblTaiTrongXe.Text = $"⚠️ QUÁ TẢI: {tl:N0} kg / {xe.TaiTrongKg:N0} kg (Vượt {vuot:N0} kg)!";
                        txtTrongLuong.ForeColor = Color.Red;
                    }
                    else
                    {
                        lblTaiTrongXe.ForeColor = Color.FromArgb(40, 167, 69);
                        lblTaiTrongXe.Text = $"✅ Tải trọng hợp lệ: {tl:N0} kg / {xe.TaiTrongKg:N0} kg ({xe.LoaiXe})";
                        txtTrongLuong.ForeColor = Color.Black;
                    }
                }
                else
                {
                    lblTaiTrongXe.ForeColor = Color.FromArgb(40, 167, 69);
                    lblTaiTrongXe.Text = $"🚛 Tải trọng tối đa: {xe.TaiTrongKg:N0} kg ({xe.LoaiXe})";
                    txtTrongLuong.ForeColor = Color.Black;
                }
            }
        }

        private void cboDonHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDonHang.SelectedItem is DonHangChoGiaoItem selectedDH && !string.IsNullOrWhiteSpace(selectedDH.ID_DH))
            {
                // Tự động gợi ý mã chuyến nếu trống hoặc đang dùng tiền tố gợi ý
                if (string.IsNullOrWhiteSpace(txtIDDonVC.Text) || txtIDDonVC.Text.StartsWith("VC_"))
                {
                    txtIDDonVC.Text = "VC_" + selectedDH.ID_DH;
                }

                // Tự động điền số lượng đặt
                txtSoLuongGiao.Text = selectedDH.SoLuongDat.ToString();

                // Tự động chọn sản phẩm tương ứng trong cboSanPham
                if (!string.IsNullOrEmpty(selectedDH.ID_SP))
                {
                    for (int i = 0; i < cboSanPham.Items.Count; i++)
                    {
                        if (cboSanPham.Items[i] is SanPhamComboItem sp && sp.ID_SP == selectedDH.ID_SP)
                        {
                            cboSanPham.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string maDon = txtIDDonVC.Text.Trim();
            string maDVC = cboMaDVC.SelectedItem?.ToString();
            string bienSo = (cboBienSoXe.SelectedItem as XeKhaDungItem)?.BienSoXe ?? cboBienSoXe.SelectedItem?.ToString();
            string sanPham = (cboSanPham.SelectedItem as SanPhamComboItem)?.ID_SP ?? cboSanPham.SelectedItem?.ToString();
            string strSoLuong = txtSoLuongGiao.Text.Trim();
            string strTrongLuong = txtTrongLuong.Text.Trim();
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

            if (maDon.Length > 20)
            {
                MessageBox.Show("Mã đơn vận chuyển không được vượt quá 20 ký tự!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIDDonVC.Focus();
                txtIDDonVC.SelectAll();
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

            decimal trongLuong = 0;
            if (!string.IsNullOrWhiteSpace(strTrongLuong) && (!decimal.TryParse(strTrongLuong, out trongLuong) || trongLuong < 0))
            {
                MessageBox.Show("Trọng lượng hàng phải là số hợp lệ (kg)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTrongLuong.Focus();
                txtTrongLuong.SelectAll();
                return;
            }

            // ==========================================================
            // KIỂM TRA QUÁ TẢI TRỌNG PHƯƠNG TIỆN
            // ==========================================================
            if (cboBienSoXe.SelectedItem is XeKhaDungItem xeItem && trongLuong > 0 && trongLuong > xeItem.TaiTrongKg)
            {
                decimal vuot = trongLuong - xeItem.TaiTrongKg;
                MessageBox.Show($"Phương tiện [{xeItem.BienSoXe}] có tải trọng tối đa {xeItem.TaiTrongKg:N0} kg.\nTổng trọng lượng hàng là {trongLuong:N0} kg (Vượt quá tải trọng {vuot:N0} kg)!\n\nVui lòng chọn phương tiện có tải trọng lớn hơn hoặc giảm lượng hàng hóa!", "Cảnh báo quá tải trọng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTrongLuong.Focus();
                txtTrongLuong.SelectAll();
                return;
            }

            // ==========================================================
            // LUỒNG NGOẠI LỆ A3: Sai định dạng thời gian
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
                string idDH = (cboDonHang.SelectedItem as DonHangChoGiaoItem)?.ID_DH;
                if (string.IsNullOrWhiteSpace(idDH)) idDH = null;

                DonVanChuyen don = new DonVanChuyen
                {
                    ID_DonVC = maDon,
                    ID_DH = idDH,
                    MaDVC = maDVC,
                    BienSoXe = bienSo,
                    ID_SP = sanPham,
                    SoLuongGiao = soLuong,
                    TrongLuong = trongLuong,
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
