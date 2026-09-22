using System;

namespace HR_Management.DTO
{
    public class ThongKeBaoCaoDTO
    {
        public string MaBaoCao { get; set; } = string.Empty;
        public string TenBaoCao { get; set; } = string.Empty;
        public string LoaiBaoCao { get; set; } = string.Empty;
        public DateTime NgayLap { get; set; } = DateTime.Today;
        public string? ID_NV { get; set; }
        public string? NguoiLap { get; set; } // Hiển thị trên UI
        public string? NoiDung { get; set; }
    }
}
