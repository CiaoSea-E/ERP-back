using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ERP_BanHang
{
    /// <summary>
    /// Bộ điều hướng dùng chung cho phân hệ Bán Hàng (ERP_BanHang).
    /// Khắc phục hoàn toàn hiện tượng nhảy giao diện, chớp tắt và tự reload lại trang.
    /// </summary>
    public static class DieuHuongBanHang
    {
        private static bool _isNavigating = false;
        private static readonly Dictionary<Type, Form> _cachedForms = new Dictionary<Type, Form>();

        /// <summary>
        /// Đăng ký Form vào hệ thống điều hướng, kích hoạt DoubleBuffered và gắn sự kiện thoát [X].
        /// </summary>
        public static void DangKyForm(Form form)
        {
            if (form == null) return;
            Type t = form.GetType();
            _cachedForms[t] = form;

            // Kích hoạt DoubleBuffered để loại bỏ hiện tượng giật hình khi vẽ
            try
            {
                System.Reflection.PropertyInfo pi = typeof(Control).GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (pi != null)
                {
                    pi.SetValue(form, true, null);
                }
            }
            catch { }

            // Gắn sự kiện đóng khi bấm [X]
            form.FormClosing -= Form_FormClosing;
            form.FormClosing += Form_FormClosing;
        }

        /// <summary>
        /// Chuyển mượt mà từ Form hiện tại sang Form đích.
        /// Chặn tự reload khi click vào chính trang hiện tại và giữ nguyên vị trí, kích thước cửa sổ.
        /// </summary>
        public static void ChuyenDen<T>(Form formHienTai) where T : Form, new()
        {
            if (formHienTai == null) return;

            // 1. Chặn click vào chính trang đang đứng -> KHÔNG làm gì cả
            if (formHienTai is T)
            {
                return;
            }

            try
            {
                _isNavigating = true;

                Type targetType = typeof(T);
                Form targetForm;

                if (_cachedForms.ContainsKey(targetType) && !_cachedForms[targetType].IsDisposed)
                {
                    targetForm = _cachedForms[targetType];
                }
                else
                {
                    targetForm = new T();
                    _cachedForms[targetType] = targetForm;
                    DangKyForm(targetForm);
                }

                // 2. Cập nhật tên nhân viên nếu có
                CapNhatTenNhanVien(targetForm);

                // 3. Đồng bộ trạng thái cửa sổ và tọa độ
                if (formHienTai.WindowState == FormWindowState.Maximized)
                {
                    targetForm.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    targetForm.WindowState = FormWindowState.Normal;
                    targetForm.StartPosition = FormStartPosition.Manual;
                    targetForm.Location = formHienTai.Location;
                    targetForm.Size = formHienTai.Size;
                }

                // 4. Hiển thị Form mới lên trước (không bị chớp tắt / mất cửa sổ)
                targetForm.Show();
                targetForm.BringToFront();
                targetForm.Activate();

                // 5. Ẩn Form cũ sau khi Form mới đã hiển thị
                formHienTai.Hide();
            }
            finally
            {
                _isNavigating = false;
            }
        }

        private static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isNavigating && e.CloseReason == CloseReason.UserClosing)
            {
                // Người dùng chủ động bấm [X] góc trên bên phải
                ThoatTatCa();
            }
        }

        private static void CapNhatTenNhanVien(Form targetForm)
        {
            string userName = QlyDonHang.CurrentEmployeeName;
            if (string.IsNullOrEmpty(userName)) return;

            try
            {
                Control[] controls = targetForm.Controls.Find("lblUserName", true);
                if (controls.Length > 0 && controls[0] is Label lbl)
                {
                    lbl.Text = userName;
                }

                Control[] topControls = targetForm.Controls.Find("lblTopUser", true);
                if (topControls.Length > 0 && topControls[0] is Label lblTop)
                {
                    lblTop.Text = userName;
                }
            }
            catch { }
        }

        /// <summary>
        /// Đóng và giải phóng toàn bộ các Form trong phân hệ Bán Hàng khi thoát
        /// </summary>
        public static void ThoatTatCa()
        {
            _isNavigating = true;

            var list = _cachedForms.Values.ToList();
            foreach (var f in list)
            {
                try
                {
                    if (f != null && !f.IsDisposed)
                    {
                        f.Dispose();
                    }
                }
                catch { }
            }
            _cachedForms.Clear();

            try
            {
                Application.Exit();
            }
            catch { }
        }
    }
}
