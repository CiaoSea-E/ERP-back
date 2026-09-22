using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using ERP.DTO;

namespace ERP.DAL
{
    public static class DatabaseConfig
    {
        public const string ConnectionString =
            @"Host=ep-bitter-heart-b3yu3xlc-pooler.c-4.ap-southeast-1.aws.neon.tech;Database=erp_banhang;Username=neondb_owner;Password=npg_fVzi2bH5uYaj;SSL Mode=Require;";


        public static string GetConnectionString()
        {
            try
            {
                ConnectionStringSettings settingConn = ConfigurationManager.ConnectionStrings["ERP_Connection"];
                if (settingConn != null && !string.IsNullOrEmpty(settingConn.ConnectionString))
                {
                    return settingConn.ConnectionString;
                }

                ConnectionStringSettings settingBH = ConfigurationManager.ConnectionStrings["ERP_BanHang"];
                if (settingBH != null && !string.IsNullOrEmpty(settingBH.ConnectionString))
                {
                    return settingBH.ConnectionString;
                }
            }
            catch
            {
                // Dự phòng nếu không đọc được ConfigurationManager
            }
            return ConnectionString;
        }
    }

    public class DonVanChuyenDAL
    {
        private readonly string connectionString;

        public DonVanChuyenDAL()
        {
            this.connectionString = DatabaseConfig.GetConnectionString();
        }

        public DonVanChuyenDAL(string connectionString)
        {
            this.connectionString = string.IsNullOrEmpty(connectionString)
                ? DatabaseConfig.GetConnectionString()
                : connectionString;
        }

        // 1. Lấy toàn bộ danh sách đơn vận chuyển
        public List<DonVanChuyen> GetAll()
        {
            List<DonVanChuyen> list = new List<DonVanChuyen>();
            const string query = @"SELECT DVC.ID_DonVC, DVC.BienSoXe, DVC.MaDVC, DVC.ID_SP, DVC.SoLuongGiao, DVC.ThoiGianKhoiHanh, DVC.TrangThaiDon,
                                          COALESCE(HH.TenHang, SP.LoaiSP) AS TenHang
                                   FROM DonVanChuyen DVC
                                   LEFT JOIN SanPham SP ON DVC.ID_SP = SP.ID_SP
                                   LEFT JOIN HangHoa HH ON SP.MaHang = HH.MaHang
                                   ORDER BY DVC.ThoiGianKhoiHanh DESC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DonVanChuyen
                        {
                            ID_DonVC = reader["ID_DonVC"] != DBNull.Value ? reader["ID_DonVC"].ToString() : string.Empty,
                            BienSoXe = reader["BienSoXe"] != DBNull.Value ? reader["BienSoXe"].ToString() : string.Empty,
                            MaDVC = reader["MaDVC"] != DBNull.Value ? reader["MaDVC"].ToString() : string.Empty,
                            ID_SP = reader["ID_SP"] != DBNull.Value ? reader["ID_SP"].ToString() : string.Empty,
                            TenHang = reader["TenHang"] != DBNull.Value ? reader["TenHang"].ToString() : string.Empty,
                            SoLuongGiao = reader["SoLuongGiao"] != DBNull.Value ? Convert.ToInt32(reader["SoLuongGiao"]) : 0,
                            ThoiGianKhoiHanh = reader["ThoiGianKhoiHanh"] != DBNull.Value ? Convert.ToDateTime(reader["ThoiGianKhoiHanh"]) : DateTime.MinValue,
                            TrangThaiDon = reader["TrangThaiDon"] != DBNull.Value ? reader["TrangThaiDon"].ToString() : string.Empty
                        });
                    }
                }
            }
            return list;
        }

        public DonVanChuyen GetByID(string id)
        {
            const string query = @"SELECT DVC.ID_DonVC, DVC.BienSoXe, DVC.MaDVC, DVC.ID_SP, DVC.SoLuongGiao, DVC.ThoiGianKhoiHanh, DVC.TrangThaiDon,
                                          COALESCE(HH.TenHang, SP.LoaiSP) AS TenHang
                                   FROM DonVanChuyen DVC
                                   LEFT JOIN SanPham SP ON DVC.ID_SP = SP.ID_SP
                                   LEFT JOIN HangHoa HH ON SP.MaHang = HH.MaHang
                                   WHERE DVC.ID_DonVC = @ID_DonVC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@ID_DonVC", NpgsqlDbType.Varchar) { Value = id ?? string.Empty });
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DonVanChuyen
                        {
                            ID_DonVC = reader["ID_DonVC"] != DBNull.Value ? reader["ID_DonVC"].ToString() : string.Empty,
                            BienSoXe = reader["BienSoXe"] != DBNull.Value ? reader["BienSoXe"].ToString() : string.Empty,
                            MaDVC = reader["MaDVC"] != DBNull.Value ? reader["MaDVC"].ToString() : string.Empty,
                            ID_SP = reader["ID_SP"] != DBNull.Value ? reader["ID_SP"].ToString() : string.Empty,
                            TenHang = reader["TenHang"] != DBNull.Value ? reader["TenHang"].ToString() : string.Empty,
                            SoLuongGiao = reader["SoLuongGiao"] != DBNull.Value ? Convert.ToInt32(reader["SoLuongGiao"]) : 0,
                            ThoiGianKhoiHanh = reader["ThoiGianKhoiHanh"] != DBNull.Value ? Convert.ToDateTime(reader["ThoiGianKhoiHanh"]) : DateTime.MinValue,
                            TrangThaiDon = reader["TrangThaiDon"] != DBNull.Value ? reader["TrangThaiDon"].ToString() : string.Empty
                        };
                    }
                }
            }
            return null;
        }

        // 2. Thêm mới đơn vận chuyển
        public bool Insert(DonVanChuyen don)
        {
            const string query = @"INSERT INTO DonVanChuyen
                                    (ID_DonVC, BienSoXe, MaDVC, ID_SP, SoLuongGiao, ThoiGianKhoiHanh, TrangThaiDon)
                                   VALUES
                                    (@ID_DonVC, @BienSoXe, @MaDVC, @ID_SP, @SoLuongGiao, @ThoiGianKhoiHanh, @TrangThaiDon)";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
                        {
                            command.Parameters.Add(new NpgsqlParameter("@ID_DonVC", NpgsqlDbType.Varchar) { Value = (object)don.ID_DonVC ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = (object)don.BienSoXe ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@MaDVC", NpgsqlDbType.Varchar) { Value = (object)don.MaDVC ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@ID_SP", NpgsqlDbType.Varchar) { Value = (object)don.ID_SP ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@SoLuongGiao", NpgsqlDbType.Integer) { Value = don.SoLuongGiao });
                            command.Parameters.Add(new NpgsqlParameter("@ThoiGianKhoiHanh", NpgsqlDbType.Timestamp) { Value = don.ThoiGianKhoiHanh });
                            command.Parameters.Add(new NpgsqlParameter("@TrangThaiDon", NpgsqlDbType.Varchar) { Value = (object)don.TrangThaiDon ?? DBNull.Value });
                            command.ExecuteNonQuery();
                        }

                        if (don.TrangThaiDon == "Đang vận chuyển")
                        {
                            CapNhatTrangThaiXe(connection, transaction, don.BienSoXe, "Đang vận chuyển");
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

        // 3. Cập nhật đơn vận chuyển
        public bool Update(DonVanChuyen don)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string oldBienSo = null;
                        string oldTrangThai = null;
                        const string qOld = "SELECT BienSoXe, TrangThaiDon FROM DonVanChuyen WHERE ID_DonVC = @ID_DonVC";
                        using (NpgsqlCommand cmdOld = new NpgsqlCommand(qOld, connection, transaction))
                        {
                            cmdOld.Parameters.Add(new NpgsqlParameter("@ID_DonVC", NpgsqlDbType.Varchar) { Value = don.ID_DonVC });
                            using (NpgsqlDataReader r = cmdOld.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    oldBienSo = r["BienSoXe"] != DBNull.Value ? r["BienSoXe"].ToString() : null;
                                    oldTrangThai = r["TrangThaiDon"] != DBNull.Value ? r["TrangThaiDon"].ToString() : null;
                                }
                            }
                        }

                        const string query = @"UPDATE DonVanChuyen
                                               SET BienSoXe = @BienSoXe,
                                                   MaDVC = @MaDVC,
                                                   ID_SP = @ID_SP,
                                                   SoLuongGiao = @SoLuongGiao,
                                                   ThoiGianKhoiHanh = @ThoiGianKhoiHanh,
                                                   TrangThaiDon = @TrangThaiDon
                                               WHERE ID_DonVC = @ID_DonVC";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
                        {
                            command.Parameters.Add(new NpgsqlParameter("@ID_DonVC", NpgsqlDbType.Varchar) { Value = (object)don.ID_DonVC ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = (object)don.BienSoXe ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@MaDVC", NpgsqlDbType.Varchar) { Value = (object)don.MaDVC ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@ID_SP", NpgsqlDbType.Varchar) { Value = (object)don.ID_SP ?? DBNull.Value });
                            command.Parameters.Add(new NpgsqlParameter("@SoLuongGiao", NpgsqlDbType.Integer) { Value = don.SoLuongGiao });
                            command.Parameters.Add(new NpgsqlParameter("@ThoiGianKhoiHanh", NpgsqlDbType.Timestamp) { Value = don.ThoiGianKhoiHanh });
                            command.Parameters.Add(new NpgsqlParameter("@TrangThaiDon", NpgsqlDbType.Varchar) { Value = (object)don.TrangThaiDon ?? DBNull.Value });
                            command.ExecuteNonQuery();
                        }

                        if (don.TrangThaiDon == "Đang vận chuyển")
                        {
                            CapNhatTrangThaiXe(connection, transaction, don.BienSoXe, "Đang vận chuyển");
                            if (!string.IsNullOrEmpty(oldBienSo) && !string.Equals(oldBienSo, don.BienSoXe, StringComparison.OrdinalIgnoreCase))
                            {
                                GiaiPhongXeNeuRanh(connection, transaction, oldBienSo, don.ID_DonVC);
                            }
                        }
                        else
                        {
                            GiaiPhongXeNeuRanh(connection, transaction, don.BienSoXe, don.ID_DonVC);
                            if (!string.IsNullOrEmpty(oldBienSo) && !string.Equals(oldBienSo, don.BienSoXe, StringComparison.OrdinalIgnoreCase))
                            {
                                GiaiPhongXeNeuRanh(connection, transaction, oldBienSo, don.ID_DonVC);
                            }
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

        // 4. Xóa đơn vận chuyển theo ID_DonVC
        public bool Delete(string id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string oldBienSo = null;
                        string oldTrangThai = null;
                        const string qOld = "SELECT BienSoXe, TrangThaiDon FROM DonVanChuyen WHERE ID_DonVC = @ID_DonVC";
                        using (NpgsqlCommand cmdOld = new NpgsqlCommand(qOld, connection, transaction))
                        {
                            cmdOld.Parameters.Add(new NpgsqlParameter("@ID_DonVC", NpgsqlDbType.Varchar) { Value = id });
                            using (NpgsqlDataReader r = cmdOld.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    oldBienSo = r["BienSoXe"] != DBNull.Value ? r["BienSoXe"].ToString() : null;
                                    oldTrangThai = r["TrangThaiDon"] != DBNull.Value ? r["TrangThaiDon"].ToString() : null;
                                }
                            }
                        }

                        const string query = "DELETE FROM DonVanChuyen WHERE ID_DonVC = @ID_DonVC";
                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
                        {
                            command.Parameters.Add(new NpgsqlParameter("@ID_DonVC", NpgsqlDbType.Varchar) { Value = (object)id ?? DBNull.Value });
                            command.ExecuteNonQuery();
                        }

                        if (oldTrangThai == "Đang vận chuyển" && !string.IsNullOrEmpty(oldBienSo))
                        {
                            GiaiPhongXeNeuRanh(connection, transaction, oldBienSo, id);
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

        // 5. Tìm kiếm theo ID_DonVC, BienSoXe hoặc MaDVC (dùng LIKE)
        public List<DonVanChuyen> Search(string keyword)
        {
            List<DonVanChuyen> list = new List<DonVanChuyen>();
            const string query = @"SELECT DVC.ID_DonVC, DVC.BienSoXe, DVC.MaDVC, DVC.ID_SP, DVC.SoLuongGiao, DVC.ThoiGianKhoiHanh, DVC.TrangThaiDon,
                                          COALESCE(HH.TenHang, SP.LoaiSP) AS TenHang
                                   FROM DonVanChuyen DVC
                                   LEFT JOIN SanPham SP ON DVC.ID_SP = SP.ID_SP
                                   LEFT JOIN HangHoa HH ON SP.MaHang = HH.MaHang
                                   WHERE DVC.ID_DonVC LIKE @Keyword
                                      OR DVC.BienSoXe LIKE @Keyword
                                      OR DVC.MaDVC LIKE @Keyword
                                      OR DVC.ID_SP LIKE @Keyword
                                      OR HH.TenHang LIKE @Keyword
                                      OR SP.LoaiSP LIKE @Keyword
                                   ORDER BY DVC.ThoiGianKhoiHanh DESC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                string searchPattern = "%" + (keyword ?? string.Empty).Trim() + "%";
                command.Parameters.Add(new NpgsqlParameter("@Keyword", NpgsqlDbType.Varchar) { Value = searchPattern });

                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DonVanChuyen
                        {
                            ID_DonVC = reader["ID_DonVC"] != DBNull.Value ? reader["ID_DonVC"].ToString() : string.Empty,
                            BienSoXe = reader["BienSoXe"] != DBNull.Value ? reader["BienSoXe"].ToString() : string.Empty,
                            MaDVC = reader["MaDVC"] != DBNull.Value ? reader["MaDVC"].ToString() : string.Empty,
                            ID_SP = reader["ID_SP"] != DBNull.Value ? reader["ID_SP"].ToString() : string.Empty,
                            TenHang = reader["TenHang"] != DBNull.Value ? reader["TenHang"].ToString() : string.Empty,
                            SoLuongGiao = reader["SoLuongGiao"] != DBNull.Value ? Convert.ToInt32(reader["SoLuongGiao"]) : 0,
                            ThoiGianKhoiHanh = reader["ThoiGianKhoiHanh"] != DBNull.Value ? Convert.ToDateTime(reader["ThoiGianKhoiHanh"]) : DateTime.MinValue,
                            TrangThaiDon = reader["TrangThaiDon"] != DBNull.Value ? reader["TrangThaiDon"].ToString() : string.Empty
                        });
                    }
                }
            }
            return list;
        }

        // 6. Kiểm tra ID_DonVC đã tồn tại chưa (phục vụ kiểm tra A2)
        public bool IsExist(string id)
        {
            const string query = "SELECT COUNT(1) FROM DonVanChuyen WHERE ID_DonVC = @ID_DonVC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@ID_DonVC", NpgsqlDbType.Varchar) { Value = (object)id ?? DBNull.Value });

                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        // 6.1 Kiểm tra xe có đang bận vận chuyển ở đơn khác, phiếu trả hàng khác hoặc đang bảo trì không
        public bool KiemTraXeDangBan(string bienSoXe, string excludeIdDonVC, out string lyDoBan)
        {
            lyDoBan = string.Empty;
            if (string.IsNullOrWhiteSpace(bienSoXe))
            {
                return false;
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // 1. Kiểm tra bảng DonVanChuyen: có đơn khác đang 'Đang vận chuyển' không
                const string queryDonVC = @"SELECT ID_DonVC 
                                           FROM DonVanChuyen 
                                           WHERE BienSoXe = @BienSoXe 
                                             AND TrangThaiDon = 'Đang vận chuyển'
                                             AND (@ExcludeIdDonVC IS NULL OR ID_DonVC <> @ExcludeIdDonVC)
                                           LIMIT 1";
                using (NpgsqlCommand cmd = new NpgsqlCommand(queryDonVC, connection))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                    cmd.Parameters.Add(new NpgsqlParameter("@ExcludeIdDonVC", NpgsqlDbType.Varchar) { Value = (object)excludeIdDonVC ?? DBNull.Value });
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        lyDoBan = $"Xe đang thực hiện đơn vận chuyển [{result}] ở trạng thái 'Đang vận chuyển'";
                        return true;
                    }
                }

                // 2. Kiểm tra bảng PhieuTraHang: có phiếu trả hàng nào đang thực hiện không
                const string queryPhieuTra = @"SELECT ID_PhieuTra, TrangThai 
                                              FROM PhieuTraHang 
                                              WHERE BienSoXe = @BienSoXe 
                                                AND TrangThai IN ('Chờ xử lý', 'Đang thu hồi', 'Đang lấy hàng', 'Đang vận chuyển')
                                              LIMIT 1";
                using (NpgsqlCommand cmd = new NpgsqlCommand(queryPhieuTra, connection))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string idPhieu = reader["ID_PhieuTra"].ToString();
                            string tt = reader["TrangThai"].ToString();
                            lyDoBan = $"Xe đang bận thực hiện phiếu trả hàng [{idPhieu}] (Trạng thái: {tt})";
                            return true;
                        }
                    }
                }

                // 3. Kiểm tra bảng PhuongTien: Trạng thái và Kích hoạt
                const string queryPhuongTien = @"SELECT TrangThaiXe, KichHoat 
                                                FROM PhuongTien 
                                                WHERE BienSoXe = @BienSoXe
                                                LIMIT 1";
                using (NpgsqlCommand cmd = new NpgsqlCommand(queryPhuongTien, connection))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool kichHoat = reader["KichHoat"] == DBNull.Value || Convert.ToBoolean(reader["KichHoat"]);
                            string trangThaiXe = reader["TrangThaiXe"] != DBNull.Value ? reader["TrangThaiXe"].ToString() : string.Empty;

                            if (!kichHoat)
                            {
                                lyDoBan = "Phương tiện này hiện đang bị ngưng kích hoạt trong hệ thống";
                                return true;
                            }

                            if (string.Equals(trangThaiXe, "Bảo trì", StringComparison.OrdinalIgnoreCase))
                            {
                                lyDoBan = "Phương tiện này đang ở trạng thái 'Bảo trì'";
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        // Cập nhật trạng thái phương tiện trong bảng PhuongTien
        private void CapNhatTrangThaiXe(NpgsqlConnection connection, NpgsqlTransaction transaction, string bienSoXe, string trangThaiXe)
        {
            if (string.IsNullOrWhiteSpace(bienSoXe)) return;

            const string query = "UPDATE PhuongTien SET TrangThaiXe = @TrangThaiXe WHERE BienSoXe = @BienSoXe";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction))
            {
                command.Parameters.Add(new NpgsqlParameter("@TrangThaiXe", NpgsqlDbType.Varchar) { Value = trangThaiXe });
                command.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                command.ExecuteNonQuery();
            }
        }

        // Giải phóng xe về trạng thái 'Đang rảnh' nếu không còn đơn hoặc phiếu nào đang vận chuyển
        private void GiaiPhongXeNeuRanh(NpgsqlConnection connection, NpgsqlTransaction transaction, string bienSoXe, string excludeIdDonVC)
        {
            if (string.IsNullOrWhiteSpace(bienSoXe)) return;

            const string qDonVC = @"SELECT COUNT(1) FROM DonVanChuyen
                                   WHERE BienSoXe = @BienSoXe
                                     AND TrangThaiDon = 'Đang vận chuyển'
                                     AND (@ExcludeIdDonVC IS NULL OR ID_DonVC <> @ExcludeIdDonVC)";
            using (NpgsqlCommand cmd = new NpgsqlCommand(qDonVC, connection, transaction))
            {
                cmd.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                cmd.Parameters.Add(new NpgsqlParameter("@ExcludeIdDonVC", NpgsqlDbType.Varchar) { Value = (object)excludeIdDonVC ?? DBNull.Value });
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0) return;
            }

            const string qPhieuTra = @"SELECT COUNT(1) FROM PhieuTraHang
                                      WHERE BienSoXe = @BienSoXe
                                        AND TrangThai IN ('Chờ xử lý', 'Đang thu hồi', 'Đang lấy hàng', 'Đang vận chuyển')";
            using (NpgsqlCommand cmd = new NpgsqlCommand(qPhieuTra, connection, transaction))
            {
                cmd.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                int countPhieu = Convert.ToInt32(cmd.ExecuteScalar());
                if (countPhieu > 0) return;
            }

            const string qCheckXe = "SELECT TrangThaiXe FROM PhuongTien WHERE BienSoXe = @BienSoXe";
            using (NpgsqlCommand cmd = new NpgsqlCommand(qCheckXe, connection, transaction))
            {
                cmd.Parameters.Add(new NpgsqlParameter("@BienSoXe", NpgsqlDbType.Varchar) { Value = bienSoXe });
                object ttObj = cmd.ExecuteScalar();
                string ttXe = ttObj != null && ttObj != DBNull.Value ? ttObj.ToString() : "";
                if (ttXe != "Bảo trì")
                {
                    CapNhatTrangThaiXe(connection, transaction, bienSoXe, "Đang rảnh");
                }
            }
        }

        // 7. Lấy danh sách BienSoXe từ bảng PhuongTien có KichHoat = 1 VÀ TrangThaiXe là 'Đang rảnh' hoặc 'Sẵn sàng'
        //    VÀ không nằm trong đơn vận chuyển hay phiếu trả hàng nào đang hoạt động
        public List<string> GetDanhSachXeKhaDung()
        {
            List<string> list = new List<string>();
            const string query = @"SELECT BienSoXe
                                   FROM PhuongTien
                                   WHERE (KichHoat = true OR KichHoat IS NULL)
                                     AND (TrangThaiXe = 'Đang rảnh' OR TrangThaiXe = 'Sẵn sàng' OR TrangThaiXe IS NULL)
                                     AND BienSoXe NOT IN (
                                         SELECT BienSoXe
                                         FROM DonVanChuyen
                                         WHERE TrangThaiDon = 'Đang vận chuyển'
                                           AND BienSoXe IS NOT NULL
                                     )
                                     AND BienSoXe NOT IN (
                                         SELECT BienSoXe
                                         FROM PhieuTraHang
                                         WHERE TrangThai IN ('Chờ xử lý', 'Đang thu hồi', 'Đang lấy hàng', 'Đang vận chuyển')
                                           AND BienSoXe IS NOT NULL
                                     )
                                   ORDER BY BienSoXe ASC";

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

        // 8. Lấy danh sách MaDVC từ bảng DiemVanChuyen
        public List<string> GetDanhSachDVC()
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

        // 9. Lấy danh sách ID_SP từ bảng SanPham
        public List<string> GetDanhSachSanPham()
        {
            List<string> list = new List<string>();
            const string query = "SELECT ID_SP FROM SanPham ORDER BY ID_SP ASC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["ID_SP"] != DBNull.Value)
                        {
                            list.Add(reader["ID_SP"].ToString());
                        }
                    }
                }
            }
            return list;
        }

        // 9.1 Lấy danh sách SanPham kèm TenHang (từ SanPham join HangHoa)
        public List<SanPhamComboItem> GetDanhSachSanPhamWithTen()
        {
            List<SanPhamComboItem> list = new List<SanPhamComboItem>();
            const string query = @"SELECT SP.ID_SP, COALESCE(HH.TenHang, SP.LoaiSP) AS TenHang
                                   FROM SanPham SP
                                   LEFT JOIN HangHoa HH ON SP.MaHang = HH.MaHang
                                   ORDER BY SP.ID_SP ASC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SanPhamComboItem
                        {
                            ID_SP = reader["ID_SP"] != DBNull.Value ? reader["ID_SP"].ToString() : string.Empty,
                            TenHang = reader["TenHang"] != DBNull.Value ? reader["TenHang"].ToString() : string.Empty
                        });
                    }
                }
            }
            return list;
        }

        // Các hàm phụ trợ tương thích ngược với BLL hiện tại
        public DataTable GetTatCaDonVanChuyen()
        {
            const string query = @"
                SELECT DVC.ID_DonVC, DVC.BienSoXe, DVC.MaDVC,
                       COALESCE(DMC.TenDVC, DVC.MaDVC) AS TenDVC,
                       DVC.ID_SP, DVC.SoLuongGiao, DVC.ThoiGianKhoiHanh,
                       DVC.TrangThaiDon
                FROM DonVanChuyen DVC
                LEFT JOIN DiemVanChuyen DMC ON DVC.MaDVC = DMC.MaDVC
                ORDER BY DVC.ThoiGianKhoiHanh DESC";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
            {
                DataTable result = new DataTable();
                adapter.Fill(result);
                return result;
            }
        }

        public void ThemDon(DonVanChuyen don)
        {
            Insert(don);
        }
    }
}



