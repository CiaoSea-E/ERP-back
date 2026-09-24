using System;
using System.Windows.Forms;
using ERP.BLL;
using ERP.DTO;

namespace ERP
{
    public partial class FrmCapNhatTrangThai : Form
    {
        private readonly DonVanChuyenBLL bll = new DonVanChuyenBLL();
        private readonly string idDonVC;
        private readonly string trangThaiHienTai;

        public FrmCapNhatTrangThai(string id, string currentStatus)
        {
            InitializeComponent();
            UIThemeHelper.ApplyFormStyle(this);
            UIThemeHelper.ApplyActionButton(this.btnLuu, ButtonRole.Primary);
            UIThemeHelper.ApplyActionButton(this.btnHuy, ButtonRole.Secondary);
            idDonVC = id;
            trangThaiHienTai = currentStatus;

            lblInfo.Text = $"Đơn vận chuyển: [{idDonVC}]";
            lblCurrent.Text = $"Trạng thái hiện tại: {trangThaiHienTai}";

            if (cboTrangThaiMoi.Items.Contains(trangThaiHienTai))
            {
                cboTrangThaiMoi.SelectedItem = trangThaiHienTai;
            }
            else if (cboTrangThaiMoi.Items.Count > 0)
            {
                cboTrangThaiMoi.SelectedIndex = 0;
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cboTrangThaiMoi.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái mới!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string statusMoi = cboTrangThaiMoi.SelectedItem.ToString();
            if (statusMoi == trangThaiHienTai)
            {
                MessageBox.Show("Trạng thái mới trùng với trạng thái hiện tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                DonVanChuyen don = bll.LayDonTheoID(idDonVC);
                if (don != null)
                {
                    if (statusMoi == "Đang vận chuyển")
                    {
                        if (bll.KiemTraXeDangBan(don.BienSoXe, don.ID_DonVC, out string lyDo))
                        {
                            MessageBox.Show($"Không thể chuyển đơn sang trạng thái 'Đang vận chuyển'!\n\nLý do: {lyDo}.\n\nVui lòng chỉnh sửa đổi xe khác cho đơn hàng này trước khi chuyển trạng thái hoặc đợi xe hoàn thành nhiệm vụ.", "Cảnh báo trùng xe / Phương tiện bận", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    don.TrangThaiDon = statusMoi;
                    bll.SuaDon(don);
                    MessageBox.Show($"Cập nhật trạng thái Đơn VC [{idDonVC}] sang [{statusMoi}] thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy đơn vận chuyển trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
