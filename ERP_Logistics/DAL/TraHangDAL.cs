using System.Data;
using Npgsql;

namespace ERP.DAL
{
    public class TraHangDAL
    {
        private readonly string connectionString;

        public TraHangDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DataTable GetDanhSachTraHang()
        {
            const string query = @"
                SELECT PT.ID_PhieuTra, YC.ID_YC, HH.TenHang, CTYC.SoLuong,
                       KH.TenDoanhNghiep, YC.TrangThai
                FROM PhieuTraHang PT
                INNER JOIN ChiTietYeuCau CTYC ON PT.ID_CTYC = CTYC.ID_CTYC
                INNER JOIN YeuCauSauBanHang YC ON CTYC.ID_YC = YC.ID_YC
                INNER JOIN SanPham SP ON CTYC.ID_SP = SP.ID_SP
                INNER JOIN HangHoa HH ON SP.MaHang = HH.MaHang
                INNER JOIN KhachHang KH ON YC.ID_KH = KH.ID_KH
                ORDER BY PT.ID_PhieuTra DESC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
            {
                DataTable result = new DataTable();
                adapter.Fill(result);
                return result;
            }
        }

        public void CapNhatTrangThaiTra(int idYeuCau, string trangThai)
        {
            const string query = "UPDATE YeuCauSauBanHang SET TrangThai = @TrangThai WHERE ID_YC = @ID_YC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TrangThai", trangThai);
                command.Parameters.AddWithValue("@ID_YC", idYeuCau);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}

