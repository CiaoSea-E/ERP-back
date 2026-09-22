using System;

namespace ERP.DTO
{
    public class DonVanChuyen
    {
        private string _trangThaiDon;

        public string ID_DonVC { get; set; }
        public string BienSoXe { get; set; }
        public string MaDVC { get; set; }
        public string ID_SP { get; set; }
        public string TenHang { get; set; }
        public int SoLuongGiao { get; set; }
        public DateTime ThoiGianKhoiHanh { get; set; }
        public string TrangThaiDon
        {
            get { return _trangThaiDon; }
            set
            {
                if (!string.IsNullOrEmpty(value) &&
                    value != "Khởi tạo" &&
                    value != "Đang vận chuyển" &&
                    value != "Hoàn thành" &&
                    value != "Đã hủy")
                {
                    throw new ArgumentException("Trạng thái đơn chỉ nhận 1 trong 4 giá trị: 'Khởi tạo', 'Đang vận chuyển', 'Hoàn thành', 'Đã hủy'.");
                }
                _trangThaiDon = value;
            }
        }
    }

    public class SanPhamComboItem
    {
        public string ID_SP { get; set; }
        public string TenHang { get; set; }

        public string DisplayText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TenHang))
                    return ID_SP;
                return $"{ID_SP} - {TenHang}";
            }
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
