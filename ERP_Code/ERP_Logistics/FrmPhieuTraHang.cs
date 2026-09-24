using System;
using System.Windows.Forms;
using ERP.BLL;
using ERP.DTO;
using System.Collections.Generic;

namespace ERP
{
    public partial class FrmPhieuTraHang : Form
    {
        private string idPhieuTra;
        private readonly PhieuTraHangBLL bll = new PhieuTraHangBLL();
        private bool isSyncingDVC = false;
        private System.Data.DataTable dtPhuongTienRanh;
        private bool isFilteringXe = false;

        public FrmPhieuTraHang(string idPhieuTra = null)
        {
            InitializeComponent();
            this.idPhieuTra = idPhieuTra;
        }

        private void FrmPhieuTraHang_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();

            if (!string.IsNullOrEmpty(idPhieuTra))
            {
                this.Text = "Cập nhật Lệnh Điều Xe";
                txtID_PhieuTra.Text = idPhieuTra;
                txtID_PhieuTra.Enabled = false; 

                try
                {
                    PhieuTraHang p = bll.GetByID(idPhieuTra);
                    if (p != null)
                    {
                        dtpNgayTra.Value = p.NgayTra;
                        
                        if (!cboID_CTYC.Items.Contains(p.ID_CTYC) && !string.IsNullOrEmpty(p.ID_CTYC))
                            cboID_CTYC.Items.Add(p.ID_CTYC);
                        cboID_CTYC.SelectedItem = p.ID_CTYC;
                        
                        if (!cboMaDVC.Items.Contains(p.MaDVC) && !string.IsNullOrEmpty(p.MaDVC))
                        {
                            cboMaDVC.Items.Add(p.MaDVC);
                            cboTenDVC.Items.Add(p.MaDVC); // Fallback to MaDVC if TenDVC is unknown
                        }
                        cboMaDVC.SelectedItem = p.MaDVC;
                        
                        if (!cboTrangThai.Items.Contains(p.TrangThai) && !string.IsNullOrEmpty(p.TrangThai))
                            cboTrangThai.Items.Add(p.TrangThai);
                        cboTrangThai.SelectedItem = p.TrangThai;
                        
                        // Initialize correctly for editing
                        isFilteringXe = true;
                        
                        string bx = p.BienSoXe;
                        if (!string.IsNullOrEmpty(bx))
                        {
                            foreach (System.Data.DataRow row in dtPhuongTienRanh.Rows)
                            {
                                if (row["BienSoXe"].ToString() == bx)
                                {
                                    cboLoaiXe.SelectedItem = row["LoaiXe"].ToString();
                                    cboTrongTai.SelectedItem = row["TaiTrong"].ToString();
                                    cboBienSoXe.SelectedItem = bx;
                                    txtTaiXe.Text = row["TenTaiXe"].ToString();
                                    break;
                                }
                            }
                            if (cboBienSoXe.SelectedItem == null)
                            {
                                cboBienSoXe.Items.Add(bx);
                                cboBienSoXe.SelectedItem = bx;
                            }
                        }
                        
                        isFilteringXe = false;
                        KhoaThongTinTheoTrangThai(p.TrangThai);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải thông tin lệnh điều xe: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this.Text = "Thêm mới Lệnh Điều Xe";
                
                try
                {
                    txtID_PhieuTra.Text = bll.SinhMaPhieu();
                }
                catch
                {
                    txtID_PhieuTra.Text = "PTH001";
                }
                txtID_PhieuTra.Enabled = false;

                dtpNgayTra.Value = DateTime.Now;
                if (cboTrangThai.Items.Count > 0)
                    cboTrangThai.SelectedIndex = 0;
                KhoaThongTinTheoTrangThai("Chờ xử lý");
            }
        }

        private void KhoaThongTinTheoTrangThai(string trangThai)
        {
            bool choSuaThongTin = string.IsNullOrEmpty(idPhieuTra) || trangThai == "Chờ xử lý";
            bool daKetThuc = trangThai == "Đã nhập kho" || trangThai == "Đã hủy";

            cboID_CTYC.Enabled = choSuaThongTin;
            cboMaDVC.Enabled = choSuaThongTin;
            cboTenDVC.Enabled = choSuaThongTin;
            dtpNgayTra.Enabled = choSuaThongTin;
            cboLoaiXe.Enabled = choSuaThongTin;
            cboTrongTai.Enabled = choSuaThongTin;
            cboBienSoXe.Enabled = choSuaThongTin;
            cboTrangThai.Enabled = !daKetThuc;
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Load CTYC
                cboID_CTYC.Items.Clear();
                List<string> dsCTYC = bll.GetDanhSachCTYC();
                foreach (var item in dsCTYC) cboID_CTYC.Items.Add(item);
                if (cboID_CTYC.Items.Count > 0) cboID_CTYC.SelectedIndex = 0;

                // Load DVC (Mã và Tên)
                cboMaDVC.Items.Clear();
                cboTenDVC.Items.Clear();
                Dictionary<string, string> dsDVC = bll.GetDanhSachDVC();
                foreach (var kvp in dsDVC)
                {
                    cboMaDVC.Items.Add(kvp.Key);
                    cboTenDVC.Items.Add(kvp.Value);
                }
                
                if (cboMaDVC.Items.Count > 0)
                {
                    cboMaDVC.SelectedIndex = 0;
                    cboTenDVC.SelectedIndex = 0;
                }

                // Load PhuongTien
                dtPhuongTienRanh = bll.GetAllPhuongTienRanh(idPhieuTra);
                
                cboLoaiXe.Items.Clear();
                cboTrongTai.Items.Clear();
                cboBienSoXe.Items.Clear();
                
                HashSet<string> loaiXeSet = new HashSet<string>();
                HashSet<string> trongTaiSet = new HashSet<string>();
                HashSet<string> bienSoSet = new HashSet<string>();
                
                foreach (System.Data.DataRow row in dtPhuongTienRanh.Rows)
                {
                    loaiXeSet.Add(row["LoaiXe"].ToString());
                    trongTaiSet.Add(row["TaiTrong"].ToString());
                    bienSoSet.Add(row["BienSoXe"].ToString());
                }
                
                foreach (var item in loaiXeSet) cboLoaiXe.Items.Add(item);
                foreach (var item in trongTaiSet) cboTrongTai.Items.Add(item);
                foreach (var item in bienSoSet) cboBienSoXe.Items.Add(item);

                // Load TrangThai
                cboTrangThai.Items.Clear();
                cboTrangThai.Items.AddRange(new string[] { "Chờ xử lý", "Đang thu hồi", "Đã nhập kho", "Đã hủy" });
                cboTrangThai.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboMaDVC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSyncingDVC) return;
            isSyncingDVC = true;
            if (cboMaDVC.SelectedIndex >= 0 && cboMaDVC.SelectedIndex < cboTenDVC.Items.Count)
            {
                cboTenDVC.SelectedIndex = cboMaDVC.SelectedIndex;
            }
            isSyncingDVC = false;
        }

        private void cboTenDVC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSyncingDVC) return;
            isSyncingDVC = true;
            if (cboTenDVC.SelectedIndex >= 0 && cboTenDVC.SelectedIndex < cboMaDVC.Items.Count)
            {
                cboMaDVC.SelectedIndex = cboTenDVC.SelectedIndex;
            }
            isSyncingDVC = false;
        }

        private void cboID_CTYC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboID_CTYC.SelectedItem != null)
            {
                string idCTYC = cboID_CTYC.SelectedItem.ToString();
                try
                {
                    ThongTinYeuCau tt = bll.GetThongTinYeuCau(idCTYC);
                    if (tt != null)
                    {
                        txtTenSP.Text = tt.TenHang;
                        txtSoLuong.Text = tt.SoLuong.ToString();
                        txtLyDo.Text = tt.LyDo;
                        txtKhachHang.Text = tt.KhachHang;
                    }
                    else
                    {
                        txtTenSP.Text = "";
                        txtSoLuong.Text = "";
                        txtLyDo.Text = "";
                        txtKhachHang.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải thông tin chi tiết yêu cầu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FilterXe()
        {
            if (isFilteringXe || dtPhuongTienRanh == null) return;
            isFilteringXe = true;
            
            string selectedLoaiXe = cboLoaiXe.SelectedItem?.ToString();
            string selectedTrongTai = cboTrongTai.SelectedItem?.ToString();
            string selectedBienSo = cboBienSoXe.SelectedItem?.ToString();
            
            HashSet<string> loaiXeSet = new HashSet<string>();
            HashSet<string> trongTaiSet = new HashSet<string>();
            HashSet<string> bienSoSet = new HashSet<string>();
            
            string taiXe = "";
            
            foreach (System.Data.DataRow row in dtPhuongTienRanh.Rows)
            {
                bool matchLoai = string.IsNullOrEmpty(selectedLoaiXe) || row["LoaiXe"].ToString() == selectedLoaiXe;
                bool matchTrongTai = string.IsNullOrEmpty(selectedTrongTai) || row["TaiTrong"].ToString() == selectedTrongTai;
                bool matchBienSo = string.IsNullOrEmpty(selectedBienSo) || row["BienSoXe"].ToString() == selectedBienSo;
                
                if (matchLoai && matchTrongTai && matchBienSo)
                {
                    loaiXeSet.Add(row["LoaiXe"].ToString());
                    trongTaiSet.Add(row["TaiTrong"].ToString());
                    bienSoSet.Add(row["BienSoXe"].ToString());
                    taiXe = row["TenTaiXe"].ToString();
                }
            }
            
            // Re-populate combos
            cboLoaiXe.Items.Clear();
            foreach (var item in loaiXeSet) cboLoaiXe.Items.Add(item);
            if (!string.IsNullOrEmpty(selectedLoaiXe) && loaiXeSet.Contains(selectedLoaiXe)) cboLoaiXe.SelectedItem = selectedLoaiXe;
            
            cboTrongTai.Items.Clear();
            foreach (var item in trongTaiSet) cboTrongTai.Items.Add(item);
            if (!string.IsNullOrEmpty(selectedTrongTai) && trongTaiSet.Contains(selectedTrongTai)) cboTrongTai.SelectedItem = selectedTrongTai;
            
            cboBienSoXe.Items.Clear();
            foreach (var item in bienSoSet) cboBienSoXe.Items.Add(item);
            if (!string.IsNullOrEmpty(selectedBienSo) && bienSoSet.Contains(selectedBienSo)) cboBienSoXe.SelectedItem = selectedBienSo;
            
            if (!string.IsNullOrEmpty(selectedLoaiXe) && !string.IsNullOrEmpty(selectedTrongTai) && !string.IsNullOrEmpty(selectedBienSo))
            {
                txtTaiXe.Text = taiXe;
            }
            else
            {
                txtTaiXe.Text = "";
            }
            
            isFilteringXe = false;
        }

        private void cboLoaiXe_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterXe();
        }
        
        private void cboTrongTai_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterXe();
        }
        
        private void cboBienSoXe_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterXe();
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                PhieuTraHang p = new PhieuTraHang
                {
                    ID_PhieuTra = txtID_PhieuTra.Text.Trim(),
                    NgayTra = dtpNgayTra.Value,
                    MaDVC = cboMaDVC.SelectedItem?.ToString() ?? cboMaDVC.Text.Trim(),
                    ID_CTYC = cboID_CTYC.SelectedItem?.ToString() ?? cboID_CTYC.Text.Trim(),
                    TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Chờ xử lý",
                    BienSoXe = cboBienSoXe.SelectedItem?.ToString() ?? cboBienSoXe.Text.Trim()
                };

                if (string.IsNullOrEmpty(idPhieuTra))
                {
                    // Thêm mới
                    bll.ThemPhieu(p);
                    MessageBox.Show("Thêm lệnh điều xe thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Sửa
                    bll.SuaPhieu(p);
                    MessageBox.Show("Cập nhật lệnh điều xe thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

