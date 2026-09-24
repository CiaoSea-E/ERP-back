using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using ERP.DTO;

namespace ERP.DAL
{
    public class PhieuTraHangDAL
    {
        private readonly string connectionString;

        public PhieuTraHangDAL()
        {
            this.connectionString = DatabaseConfig.GetConnectionString();
        }

        public PhieuTraHangDAL(string connectionString)
        {
            this.connectionString = string.IsNullOrWhiteSpace(connectionString)
                ? DatabaseConfig.GetConnectionString()
                : connectionString;
        }

        public string GetNextID()
        {
            // Lấy số thứ tự lớn nhất từ các mã có dạng PTH + số
            const string query = @"
                SELECT COALESCE(MAX(CAST(SUBSTRING(id_phieutra FROM 4) AS INTEGER)), 0) + 1
                FROM phieutrahang
                WHERE id_phieutra ~ '^PTH[0-9]+$'";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                int nextNum = Convert.ToInt32(command.ExecuteScalar());
                return "PTH" + nextNum.ToString("D3");
            }
        }

        // 1. Lấy toàn bộ danh sách phiếu trả hàng
        public List<PhieuTraHang> GetAll()
        {
            List<PhieuTraHang> list = new List<PhieuTraHang>();
            const string query = @"SELECT ID_PhieuTra, NgayTra, MaDVC, ID_CTYC, TrangThai, BienSoXe
                                   FROM PhieuTraHang
                                   ORDER BY NgayTra DESC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PhieuTraHang
                        {
                            ID_PhieuTra = reader["ID_PhieuTra"] != DBNull.Value ? reader["ID_PhieuTra"].ToString() : string.Empty,
                            NgayTra = reader["NgayTra"] != DBNull.Value ? Convert.ToDateTime(reader["NgayTra"]) : DateTime.MinValue,
                            MaDVC = reader["MaDVC"] != DBNull.Value ? reader["MaDVC"].ToString() : string.Empty,
                            ID_CTYC = reader["ID_CTYC"] != DBNull.Value ? reader["ID_CTYC"].ToString() : string.Empty,
                            TrangThai = reader["TrangThai"] != DBNull.Value ? reader["TrangThai"].ToString() : string.Empty,
                            BienSoXe = reader["BienSoXe"] != DBNull.Value ? reader["BienSoXe"].ToString() : string.Empty
                        });
                    }
                }
            }
            return list;
        }

        // 2. Lấy phiếu trả hàng theo ID
        public PhieuTraHang GetByID(string id)
        {
            const string query = @"SELECT ID_PhieuTra, NgayTra, MaDVC, ID_CTYC, TrangThai, BienSoXe
                                   FROM PhieuTraHang
                                   WHERE ID_PhieuTra = @ID_PhieuTra";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = id ?? string.Empty });
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PhieuTraHang
                        {
                            ID_PhieuTra = reader["ID_PhieuTra"] != DBNull.Value ? reader["ID_PhieuTra"].ToString() : string.Empty,
                            NgayTra = reader["NgayTra"] != DBNull.Value ? Convert.ToDateTime(reader["NgayTra"]) : DateTime.MinValue,
                            MaDVC = reader["MaDVC"] != DBNull.Value ? reader["MaDVC"].ToString() : string.Empty,
                            ID_CTYC = reader["ID_CTYC"] != DBNull.Value ? reader["ID_CTYC"].ToString() : string.Empty,
                            TrangThai = reader["TrangThai"] != DBNull.Value ? reader["TrangThai"].ToString() : string.Empty,
                            BienSoXe = reader["BienSoXe"] != DBNull.Value ? reader["BienSoXe"].ToString() : string.Empty
                        };
                    }
                }
            }
            return null;
        }

        // 3. Thêm mới phiếu trả hàng
        public bool Insert(PhieuTraHang p)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string trangThai = ChuanHoaTrangThaiPhieu(p.TrangThai);

                        const string query1 = @"INSERT INTO PhieuTraHang (ID_PhieuTra, NgayTra, MaDVC, ID_CTYC, TrangThai, BienSoXe)
                                               VALUES (@ID_PhieuTra, @NgayTra, @MaDVC, @ID_CTYC, @TrangThai, @BienSoXe)";
                        using (NpgsqlCommand cmd1 = new NpgsqlCommand(query1, connection, transaction))
                        {
                            cmd1.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = (object)p.ID_PhieuTra ?? DBNull.Value });
                            cmd1.Parameters.Add(new NpgsqlParameter("@NgayTra", NpgsqlDbType.Timestamp) { Value = p.NgayTra != DateTime.MinValue ? p.NgayTra : DateTime.Now });
                            cmd1.Parameters.Add(new NpgsqlParameter("@MaDVC", NpgsqlDbType.Varchar) { Value = (object)p.MaDVC ?? DBNull.Value });
                            cmd1.Parameters.Add(new NpgsqlParameter("@ID_CTYC", NpgsqlDbType.Varchar) { Value = (object)p.ID_CTYC ?? DBNull.Value });
                            cmd1.Parameters.Add(new NpgsqlParameter("@TrangThai", NpgsqlDbType.Varchar) { Value = trangThai });
                            cmd1.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = (object)p.BienSoXe ?? DBNull.Value });
                            cmd1.ExecuteNonQuery();
                        }

                        if (trangThai != "Đã hủy" && trangThai != "Đã nhập kho")
                        {
                            CapNhatTrangThaiXe(connection, transaction, p.BienSoXe, "Đang vận chuyển");
                        }

                        DongBoYeuCauSauBanHang(connection, transaction, p.ID_CTYC);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 4. Cập nhật phiếu trả hàng
        public bool Update(PhieuTraHang p)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        PhieuTraHang cu = GetByID(connection, transaction, p.ID_PhieuTra);
                        if (cu == null)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        string trangThaiMoi = ChuanHoaTrangThaiPhieu(p.TrangThai);

                        const string query = @"UPDATE PhieuTraHang
                                               SET NgayTra = @NgayTra,
                                                   MaDVC = @MaDVC,
                                                   ID_CTYC = @ID_CTYC,
                                                   TrangThai = @TrangThai,
                                                   BienSoXe = @BienSoXe
                                               WHERE ID_PhieuTra = @ID_PhieuTra";
                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
                        {
                            command.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = (object)p.ID_PhieuTra ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@NgayTra", NpgsqlDbType.Timestamp) { Value = p.NgayTra != DateTime.MinValue ? p.NgayTra : DateTime.Now });
                            command.Parameters.Add(new NpgsqlParameter("@MaDVC", NpgsqlDbType.Varchar) { Value = (object)p.MaDVC ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@ID_CTYC", NpgsqlDbType.Varchar) { Value = (object)p.ID_CTYC ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@TrangThai", NpgsqlDbType.Varchar) { Value = trangThaiMoi });
                            command.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = (object)p.BienSoXe ?? DBNull.Value });
                            command.ExecuteNonQuery();
                        }

                        DongBoXeSauKhiDoiPhieu(connection, transaction, cu.BienSoXe, p.BienSoXe, cu.ID_PhieuTra, trangThaiMoi);

                        DongBoYeuCauSauBanHang(connection, transaction, p.ID_CTYC);
                        if (!string.Equals(cu.ID_CTYC, p.ID_CTYC, StringComparison.Ordinal))
                        {
                            DongBoYeuCauSauBanHang(connection, transaction, cu.ID_CTYC);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 5. Xóa phiếu trả hàng theo ID_PhieuTra
        public bool Delete(string id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string bienSoXe = null;
                        string idCTYC = null;

                        const string queryInfo = "SELECT BienSoXe, ID_CTYC FROM PhieuTraHang WHERE ID_PhieuTra = @ID_PhieuTra";
                        using (NpgsqlCommand cmdInfo = new NpgsqlCommand(queryInfo, connection, transaction))
                        {
                            cmdInfo.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = id ?? string.Empty });
                            using (var reader = cmdInfo.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bienSoXe = reader["BienSoXe"] != DBNull.Value ? reader["BienSoXe"].ToString() : null;
                                    idCTYC = reader["ID_CTYC"] != DBNull.Value ? reader["ID_CTYC"].ToString() : null;
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(idCTYC))
                        {
                            transaction.Rollback();
                            return false;
                        }

                        const string query1 = "DELETE FROM PhieuTraHang WHERE ID_PhieuTra = @ID_PhieuTra";
                        using (NpgsqlCommand cmd1 = new NpgsqlCommand(query1, connection, transaction))
                        {
                            cmd1.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = id ?? string.Empty });
                            cmd1.ExecuteNonQuery();
                        }

                        GiaiPhongXeNeuKhongConPhieu(connection, transaction, bienSoXe, id);

                        DongBoYeuCauSauBanHang(connection, transaction, idCTYC);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 6. Kiểm tra ID_CTYC đã tồn tại trong PhieuTraHang chưa (thu hồi trùng lặp)
        public bool CheckThuHoiDup(string idCTYC)
        {
            const string query = @"SELECT COUNT(*) FROM PhieuTraHang
                                   WHERE ID_CTYC = @ID_CTYC
                                     AND TrangThai <> N'Đã hủy'";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@ID_CTYC", NpgsqlDbType.Varchar) { Value = idCTYC ?? string.Empty });
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        // 7. Tìm kiếm có JOIN với bảng DiemVanChuyen
        public List<PhieuTraHang> Search(string keyword, DateTime tuNgay, DateTime denNgay, string trangThai)
        {
            List<PhieuTraHang> list = new List<PhieuTraHang>();
            string kw = (keyword ?? string.Empty).Trim();
            string kwLike = "%" + kw + "%";

            // Đảm bảo lọc trọn vẹn từ 00:00:00 của tuNgay đến 23:59:59 của denNgay
            DateTime start = tuNgay.Date;
            DateTime end = denNgay == DateTime.MaxValue ? DateTime.MaxValue : denNgay.Date.AddDays(1).AddTicks(-1);

            const string query = @"
                SELECT PT.ID_PhieuTra, PT.NgayTra, PT.MaDVC, PT.ID_CTYC, PT.TrangThai, PT.BienSoXe
                FROM PhieuTraHang PT
                LEFT JOIN DiemVanChuyen DVC ON PT.MaDVC = DVC.MaDVC
                WHERE PT.NgayTra >= @TuNgay AND PT.NgayTra <= @DenNgay
                  AND (@Keyword = '' OR PT.ID_PhieuTra LIKE @KwLike OR DVC.TenDVC LIKE @KwLike)
                  AND (@TrangThai = '' OR @TrangThai = N'Tất cả trạng thái' OR PT.TrangThai = @TrangThai
                       OR (@TrangThai = N'Đang thu hồi' AND PT.TrangThai = N'Đang lấy hàng'))
                ORDER BY PT.NgayTra DESC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@TuNgay", NpgsqlDbType.Timestamp) { Value = start });
                command.Parameters.Add(new NpgsqlParameter("@DenNgay", NpgsqlDbType.Timestamp) { Value = end });
                command.Parameters.Add(new NpgsqlParameter("@Keyword", NpgsqlDbType.Varchar) { Value = kw });
                command.Parameters.Add(new NpgsqlParameter("@KwLike", NpgsqlDbType.Varchar) { Value = kwLike });
                command.Parameters.Add(new NpgsqlParameter("@TrangThai", NpgsqlDbType.Varchar) { Value = trangThai ?? string.Empty });

                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PhieuTraHang
                        {
                            ID_PhieuTra = reader["ID_PhieuTra"] != DBNull.Value ? reader["ID_PhieuTra"].ToString() : string.Empty,
                            NgayTra = reader["NgayTra"] != DBNull.Value ? Convert.ToDateTime(reader["NgayTra"]) : DateTime.MinValue,
                            MaDVC = reader["MaDVC"] != DBNull.Value ? reader["MaDVC"].ToString() : string.Empty,
                            ID_CTYC = reader["ID_CTYC"] != DBNull.Value ? reader["ID_CTYC"].ToString() : string.Empty,
                            TrangThai = reader["TrangThai"] != DBNull.Value ? reader["TrangThai"].ToString() : string.Empty,
                            BienSoXe = reader["BienSoXe"] != DBNull.Value ? reader["BienSoXe"].ToString() : string.Empty
                        });
                    }
                }
            }
            return list;
        }

        // 8. Lấy danh sách ID_CTYC từ bảng ChiTietYeuCau (Chỉ lấy các yêu cầu đã được Bán hàng xác nhận 'Đang xử lý')
        public List<string> GetDanhSachCTYC()
        {
            List<string> list = new List<string>();
            const string query = @"SELECT c.ID_CTYC 
                                   FROM ChiTietYeuCau c
                                   JOIN YeuCauSauBanHang y ON c.ID_YC = y.ID_YC
                                   WHERE y.TrangThai = N'Đang xử lý'
                                     AND c.ID_CTYC NOT IN (
                                         SELECT ID_CTYC FROM PhieuTraHang
                                         WHERE ID_CTYC IS NOT NULL AND TrangThai <> N'Đã hủy'
                                     )
                                   ORDER BY c.ID_CTYC ASC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["ID_CTYC"] != DBNull.Value)
                        {
                            list.Add(reader["ID_CTYC"].ToString());
                        }
                    }
                }
            }
            return list;
        }

        // 9. Lấy danh sách MaDVC từ bảng DiemVanChuyen
        public List<string> GetDanhSachMaDVC()
        {
            List<string> list = new List<string>();
            const string query = "SELECT MaDVC FROM DiemVanChuyen ORDER BY MaDVC ASC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["MaDVC"] != DBNull.Value)
                        {
                            list.Add(reader["MaDVC"].ToString());
                        }
                    }
                }
            }
            return list;
        }

        // 9.5. Lấy danh sách Mã và Tên DVC
        public Dictionary<string, string> GetDanhSachDVC()
        {
            var dict = new Dictionary<string, string>();
            const string query = "SELECT MaDVC, TenDVC FROM DiemVanChuyen ORDER BY MaDVC ASC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["MaDVC"] != DBNull.Value)
                        {
                            string ma = reader["MaDVC"].ToString();
                            string ten = reader["TenDVC"] != DBNull.Value ? reader["TenDVC"].ToString() : string.Empty;
                            dict[ma] = ten;
                        }
                    }
                }
            }
            return dict;
        }

        // 10. Lấy danh sách BienSoXe từ bảng PhuongTien
        public List<string> GetDanhSachXeKhaDung()
        {
            List<string> list = new List<string>();
            const string query = "SELECT BienSoXe FROM PhuongTien ORDER BY BienSoXe ASC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["BienSoXe"] != DBNull.Value)
                        {
                            list.Add(reader["BienSoXe"].ToString());
                        }
                    }
                }
            }
            return list;
        }

        // 11. Lấy thông tin yêu cầu để hiển thị
        public ThongTinYeuCau GetThongTinYeuCau(string idCTYC)
        {
            const string query = @"
                SELECT hh.TenHang, ctyc.SoLuong, ctyc.TinhTrang as LyDo, kh.TenDoanhNghiep as KhachHang
                FROM ChiTietYeuCau ctyc
                JOIN SanPham sp ON ctyc.ID_SP = sp.ID_SP
                JOIN HangHoa hh ON sp.MaHang = hh.MaHang
                JOIN YeuCauSauBanHang yc ON ctyc.ID_YC = yc.ID_YC
                JOIN KhachHang kh ON yc.ID_KH = kh.ID_KH
                WHERE ctyc.ID_CTYC = @ID_CTYC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@ID_CTYC", NpgsqlDbType.Varchar) { Value = idCTYC ?? string.Empty });
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ThongTinYeuCau
                        {
                            TenHang = reader["TenHang"] != DBNull.Value ? reader["TenHang"].ToString() : string.Empty,
                            SoLuong = reader["SoLuong"] != DBNull.Value ? Convert.ToInt32(reader["SoLuong"]) : 0,
                            LyDo = reader["LyDo"] != DBNull.Value ? reader["LyDo"].ToString() : string.Empty,
                            KhachHang = reader["KhachHang"] != DBNull.Value ? reader["KhachHang"].ToString() : string.Empty
                        };
                    }
                }
            }
            return null;
        }
        // 12. Lấy danh sách Loại Xe Rảnh
        public List<string> GetDanhSachLoaiXeRanh()
        {
            List<string> list = new List<string>();
            const string query = @"SELECT DISTINCT LoaiXe FROM PhuongTien 
                                   WHERE TrangThaiXe IN ('Sẵn sàng', 'Đang rảnh')
                                   ORDER BY LoaiXe ASC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LoaiXe"] != DBNull.Value)
                        {
                            list.Add(reader["LoaiXe"].ToString());
                        }
                    }
                }
            }
            return list;
        }

        // 12. Kiểm tra xe có đang bận ở phiếu trả hàng khác không
        public bool CheckXeDangBan(string bienSoXe, string idPhieuLoaiTru = null)
        {
            if (string.IsNullOrWhiteSpace(bienSoXe)) return false;

            const string query = @"SELECT COUNT(*) FROM PhieuTraHang
                                   WHERE BienSoXe = @BienSoXe
                                     AND TrangThai IN (N'Chờ xử lý', N'Đang thu hồi', N'Đang lấy hàng')
                                     AND (@ID_PhieuTra = '' OR ID_PhieuTra <> @ID_PhieuTra)";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe.Trim() });
                command.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = idPhieuLoaiTru ?? string.Empty });
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        public DataTable GetAllPhuongTienRanh(string idPhieuHienTai = null)
        {
            var dt = new DataTable();
            const string query = @"
                SELECT BienSoXe, LoaiXe, TaiTrong, TenTaiXe 
                FROM PhuongTien 
                WHERE (TrangThaiXe IN ('Sẵn sàng', 'Đang rảnh') 
                       OR (@ID_PhieuTra <> '' AND BienSoXe IN (SELECT BienSoXe FROM PhieuTraHang WHERE ID_PhieuTra = @ID_PhieuTra)))
                  AND BienSoXe NOT IN (
                      SELECT BienSoXe FROM PhieuTraHang
                      WHERE BienSoXe IS NOT NULL 
                        AND TrangThai IN (N'Chờ xử lý', N'Đang thu hồi', N'Đang lấy hàng')
                        AND (@ID_PhieuTra = '' OR ID_PhieuTra <> @ID_PhieuTra)
                  )
                ORDER BY BienSoXe ASC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = idPhieuHienTai ?? string.Empty });
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        private static string ChuanHoaTrangThaiPhieu(string trangThai)
        {
            if (string.IsNullOrWhiteSpace(trangThai))
            {
                return "Chờ xử lý";
            }

            if (trangThai == "Đang lấy hàng")
            {
                return "Đang thu hồi";
            }

            return trangThai;
        }

        private static bool XeDangThuHoi(string trangThai)
        {
            return trangThai == "Đang thu hồi" || trangThai == "Đang lấy hàng";
        }

        private PhieuTraHang GetByID(NpgsqlConnection connection, NpgsqlTransaction transaction, string id)
        {
            const string query = @"SELECT ID_PhieuTra, NgayTra, MaDVC, ID_CTYC, TrangThai, BienSoXe
                                   FROM PhieuTraHang
                                   WHERE ID_PhieuTra = @ID_PhieuTra";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
            {
                command.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = id ?? string.Empty });
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PhieuTraHang
                        {
                            ID_PhieuTra = reader["ID_PhieuTra"] != DBNull.Value ? reader["ID_PhieuTra"].ToString() : string.Empty,
                            NgayTra = reader["NgayTra"] != DBNull.Value ? Convert.ToDateTime(reader["NgayTra"]) : DateTime.MinValue,
                            MaDVC = reader["MaDVC"] != DBNull.Value ? reader["MaDVC"].ToString() : string.Empty,
                            ID_CTYC = reader["ID_CTYC"] != DBNull.Value ? reader["ID_CTYC"].ToString() : string.Empty,
                            TrangThai = reader["TrangThai"] != DBNull.Value ? reader["TrangThai"].ToString() : string.Empty,
                            BienSoXe = reader["BienSoXe"] != DBNull.Value ? reader["BienSoXe"].ToString() : string.Empty
                        };
                    }
                }
            }

            return null;
        }

        private void CapNhatTrangThaiXe(NpgsqlConnection connection, NpgsqlTransaction transaction, string bienSoXe, string trangThaiXe)
        {
            if (string.IsNullOrWhiteSpace(bienSoXe))
            {
                return;
            }

            const string query = "UPDATE PhuongTien SET TrangThaiXe = @TrangThaiXe WHERE BienSoXe = @BienSoXe";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
            {
                command.Parameters.Add(new NpgsqlParameter("@TrangThaiXe", NpgsqlDbType.Varchar) { Value = trangThaiXe });
                command.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                command.ExecuteNonQuery();
            }
        }

        private void GiaiPhongXeNeuKhongConPhieu(NpgsqlConnection connection, NpgsqlTransaction transaction, string bienSoXe, string idPhieuLoaiTru)
        {
            if (string.IsNullOrWhiteSpace(bienSoXe))
            {
                return;
            }

            const string query = @"SELECT COUNT(*) FROM PhieuTraHang
                                   WHERE BienSoXe = @BienSoXe
                                     AND TrangThai IN (N'Chờ xử lý', N'Đang thu hồi', N'Đang lấy hàng')
                                     AND ID_PhieuTra <> @ID_PhieuTra";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
            {
                command.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                command.Parameters.Add(new NpgsqlParameter("@ID_PhieuTra", NpgsqlDbType.Varchar) { Value = idPhieuLoaiTru ?? string.Empty });
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    CapNhatTrangThaiXe(connection, transaction, bienSoXe, "Sẵn sàng");
                }
            }
        }

        private void DongBoXeSauKhiDoiPhieu(NpgsqlConnection connection, NpgsqlTransaction transaction, string bienSoCu, string bienSoMoi, string idPhieu, string trangThaiMoi)
        {
            if (trangThaiMoi != "Đã hủy" && trangThaiMoi != "Đã nhập kho")
            {
                CapNhatTrangThaiXe(connection, transaction, bienSoMoi, "Đang vận chuyển");
                if (!string.Equals(bienSoCu, bienSoMoi, StringComparison.OrdinalIgnoreCase))
                {
                    GiaiPhongXeNeuKhongConPhieu(connection, transaction, bienSoCu, idPhieu);
                }
            }
            else
            {
                GiaiPhongXeNeuKhongConPhieu(connection, transaction, bienSoMoi, idPhieu);
                if (!string.Equals(bienSoCu, bienSoMoi, StringComparison.OrdinalIgnoreCase))
                {
                    GiaiPhongXeNeuKhongConPhieu(connection, transaction, bienSoCu, idPhieu);
                }
            }
        }

        private void DongBoYeuCauSauBanHang(NpgsqlConnection connection, NpgsqlTransaction transaction, string idCTYC)
        {
            if (string.IsNullOrWhiteSpace(idCTYC))
            {
                return;
            }

            const string query = @"
                UPDATE YeuCauSauBanHang y
                SET TrangThai = CASE
                    WHEN EXISTS (
                        SELECT 1 FROM ChiTietYeuCau c
                        JOIN PhieuTraHang p ON p.ID_CTYC = c.ID_CTYC
                        WHERE c.ID_YC = y.ID_YC
                          AND p.TrangThai IN (N'Đang thu hồi', N'Đang lấy hàng', N'Đã nhập kho')
                    ) THEN N'Đã xử lý'
                    WHEN EXISTS (
                        SELECT 1 FROM ChiTietYeuCau c
                        JOIN PhieuTraHang p ON p.ID_CTYC = c.ID_CTYC
                        WHERE c.ID_YC = y.ID_YC
                          AND p.TrangThai = N'Chờ xử lý'
                    ) THEN N'Đang xử lý'
                    ELSE y.TrangThai
                END
                WHERE y.ID_YC = (SELECT ID_YC FROM ChiTietYeuCau WHERE ID_CTYC = @ID_CTYC)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
            {
                command.Parameters.Add(new NpgsqlParameter("@ID_CTYC", NpgsqlDbType.Varchar) { Value = idCTYC });
                command.ExecuteNonQuery();
            }
        }
    }
}



