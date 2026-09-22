using System;
using System.Collections.Generic;
using System.Data;
using HR_Management.DTO;
using System.Data.SqlClient;

namespace HR_Management.DAL
{
    public class DashboardOverviewDTO
    {
        public int TongNhanVien { get; set; }
        public int TongPhongBan { get; set; }
        public int HopDongHieuLuc { get; set; }
        public int HopDongSapHetHan { get; set; }
        public int NhanVienMoiThangNay { get; set; }
    }

    public class ThongKeBaoCaoDAL
    {
        public DashboardOverviewDTO GetOverview()
        {
            DashboardOverviewDTO dto = new DashboardOverviewDTO();

            try
            {
                // 1. Tổng nhân viên (đang hoạt động / chưa nghỉ việc)
                dto.TongNhanVien = Convert.ToInt32(DatabaseHelper.ExecuteScalar(@"
                    SELECT COUNT(*) FROM NhanVien 
                    WHERE TrangThai IS NULL 
                       OR (TrangThai NOT LIKE '%nghỉ%' AND TrangThai NOT LIKE '%NGHI%')"));

                // 2. Tổng phòng ban đang hoạt động (hỗ trợ cả HOATDONG và Hoạt động)
                dto.TongPhongBan = Convert.ToInt32(DatabaseHelper.ExecuteScalar(@"
                    SELECT COUNT(*) FROM PhongBan 
                    WHERE trangThai IS NULL 
                       OR UPPER(trangThai) LIKE '%HOAT%' 
                       OR trangThai = 'Hoạt động' 
                       OR (trangThai NOT LIKE '%khóa%' AND trangThai NOT LIKE '%ngừng%')"));

                // 3. Hợp đồng hiệu lực
                dto.HopDongHieuLuc = Convert.ToInt32(DatabaseHelper.ExecuteScalar(@"
                    SELECT COUNT(*) FROM HopDong 
                    WHERE trangThai = 'Hiệu lực' 
                       OR UPPER(trangThai) LIKE '%HIEU%' 
                       OR UPPER(trangThai) LIKE '%HOAT%'"));

                // 4. Hợp đồng sắp hết hạn (<30 ngày)
                dto.HopDongSapHetHan = Convert.ToInt32(DatabaseHelper.ExecuteScalar(@"
                    SELECT COUNT(*) FROM HopDong 
                    WHERE ngayKetThuc IS NOT NULL 
                      AND ngayKetThuc >= CAST(GETDATE() AS DATE) 
                      AND ngayKetThuc <= DATEADD(day, 30, CAST(GETDATE() AS DATE))
                      AND (trangThai = 'Hiệu lực' OR UPPER(trangThai) LIKE '%HIEU%' OR UPPER(trangThai) LIKE '%HOAT%')"));
            }
            catch
            {
                // Fallback nếu có lỗi
            }
            return dto;
        }
        public DataTable GetDepartmentStats()
        {
            string query = @"
                SELECT p.maPhongBan AS [Mã PB], p.tenPhongBan AS [Tên Phòng Ban], 
                       p.truongPhong AS [Trưởng Phòng],
                       COUNT(nv.ID_NV) AS [Số Nhân Viên],
                       ISNULL(AVG(nv.LuongCoBan), 0) AS [Lương TB]
                FROM PhongBan p
                LEFT JOIN NhanVien nv ON p.maPhongBan = nv.maPhongBan
                GROUP BY p.maPhongBan, p.tenPhongBan, p.truongPhong
                ORDER BY [Số Nhân Viên] DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        public DataTable GetEducationStats()
        {
            string query = @"
                SELECT ISNULL(tt.trinhDo, N'Chưa cập nhật') AS [Trình Độ], 
                       COUNT(nv.ID_NV) AS [Số Lượng]
                FROM NhanVien nv
                LEFT JOIN ThongTinNhanVien tt ON nv.ID_NV = tt.ID_NV
                GROUP BY tt.trinhDo
                ORDER BY [Số Lượng] DESC";

            return DatabaseHelper.ExecuteQuery(query);
        }

        public List<ThongKeBaoCaoDTO> GetAllReports()
        {
            List<ThongKeBaoCaoDTO> list = new List<ThongKeBaoCaoDTO>();
            string query = @"
                SELECT bc.maBaoCao, bc.tenBaoCao, bc.loaiBaoCao, bc.ngayLap, bc.ID_NV, nv.TenNV AS NguoiLap, bc.noiDung
                FROM ThongKeBaoCao bc
                LEFT JOIN NhanVien nv ON bc.ID_NV = nv.ID_NV
                ORDER BY bc.ngayLap DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ThongKeBaoCaoDTO
                {
                    MaBaoCao = row["maBaoCao"]?.ToString() ?? "",
                    TenBaoCao = row["tenBaoCao"]?.ToString() ?? "",
                    LoaiBaoCao = row["loaiBaoCao"]?.ToString() ?? "",
                    NgayLap = DatabaseHelper.ToDateTime(row["ngayLap"]),
                    ID_NV = row["ID_NV"] != DBNull.Value ? row["ID_NV"].ToString() : "",
                    NguoiLap = row.Table.Columns.Contains("NguoiLap") && row["NguoiLap"] != DBNull.Value ? row["NguoiLap"].ToString() : "",
                    NoiDung = row["noiDung"] != DBNull.Value ? row["noiDung"].ToString() : ""
                });
            }
            return list;
        }

        public bool InsertReport(ThongKeBaoCaoDTO dto)
        {
            string query = @"
                INSERT INTO ThongKeBaoCao (maBaoCao, tenBaoCao, loaiBaoCao, ngayLap, ID_NV, noiDung)
                VALUES (@maBaoCao, @tenBaoCao, @loaiBaoCao, @ngayLap, @ID_NV, @noiDung)";

            SqlParameter[] param = {
                new SqlParameter("@maBaoCao", dto.MaBaoCao),
                new SqlParameter("@tenBaoCao", dto.TenBaoCao),
                new SqlParameter("@loaiBaoCao", dto.LoaiBaoCao),
                new SqlParameter("@ngayLap", dto.NgayLap),
                new SqlParameter("@ID_NV", (object?)dto.ID_NV ?? DBNull.Value),
                new SqlParameter("@noiDung", (object?)dto.NoiDung ?? DBNull.Value)
            };

            return DatabaseHelper.ExecuteNonQuery(query, param) > 0;
        }

        public string GenerateNextId()
        {
            string query = "SELECT MAX(CAST(SUBSTRING(maBaoCao, 3, LEN(maBaoCao) - 2) AS INT)) FROM ThongKeBaoCao WHERE maBaoCao LIKE 'BC%' AND ISNUMERIC(SUBSTRING(maBaoCao, 3, LEN(maBaoCao) - 2)) = 1";
            object? result = DatabaseHelper.ExecuteScalar(query);
            int nextNumber = 1;
            if (result != null && result != DBNull.Value)
            {
                nextNumber = Convert.ToInt32(result) + 1;
            }
            return $"BC{nextNumber:D3}";
        }
    }
}
