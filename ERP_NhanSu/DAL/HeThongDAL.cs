using System;
using System.Collections.Generic;
using System.Data;
using HR_Management.BLL;
using HR_Management.DTO;
using System.Data.SqlClient;

namespace HR_Management.DAL
{
    public class HeThongDAL
    {
        public HeThongDTO? Authenticate(string username, string password)
        {
            string query = @"
                SELECT ht.maTaiKhoan, ht.ID_NV, nv.TenNV, ht.tenDangNhap, 
                       ht.matKhau, ht.vaiTro, ht.trangThai, ht.lanDangNhapCuoi, ht.quyenHan
                FROM HeThong ht
                LEFT JOIN NhanVien nv ON ht.ID_NV = nv.ID_NV
                WHERE ht.tenDangNhap = @username AND ht.matKhau = @password";

            SqlParameter[] param = {
                new SqlParameter("@username", username),
                new SqlParameter("@password", password)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, param);
            if (dt.Rows.Count > 0)
            {
                var user = MapRowToHeThong(dt.Rows[0]);
                if (HeThongBLL.IsActiveStatus(user.TrangThai))
                {
                    UpdateLastLogin(user.MaTaiKhoan);
                }
                return user;
            }
            return null;
        }

        public List<HeThongDTO> GetAll()
        {
            List<HeThongDTO> list = new List<HeThongDTO>();
            string query = @"
                SELECT ht.maTaiKhoan, ht.ID_NV, nv.TenNV, ht.tenDangNhap, 
                       ht.matKhau, ht.vaiTro, ht.trangThai, ht.lanDangNhapCuoi, ht.quyenHan
                FROM HeThong ht
                LEFT JOIN NhanVien nv ON ht.ID_NV = nv.ID_NV
                ORDER BY ht.maTaiKhoan";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToHeThong(row));
            }
            return list;
        }

        public bool Insert(HeThongDTO ht)
        {
            string query = @"
                INSERT INTO HeThong (maTaiKhoan, ID_NV, tenDangNhap, matKhau, vaiTro, trangThai, quyenHan)
                VALUES (@maTaiKhoan, @ID_NV, @tenDangNhap, @matKhau, @vaiTro, @trangThai, @quyenHan)";

            SqlParameter[] param = {
                new SqlParameter("@maTaiKhoan", ht.MaTaiKhoan),
                new SqlParameter("@ID_NV", ht.ID_NV),
                new SqlParameter("@tenDangNhap", ht.TenDangNhap),
                new SqlParameter("@matKhau", ht.MatKhau),
                new SqlParameter("@vaiTro", (object?)ht.VaiTro ?? "Nhân viên"),
                new SqlParameter("@trangThai", ht.TrangThai),
                new SqlParameter("@quyenHan", (object?)ht.QuyenHan ?? "ALL")
            };

            return DatabaseHelper.ExecuteNonQuery(query, param) > 0;
        }

        public bool Update(HeThongDTO ht)
        {
            string query;
            SqlParameter[] param;

            if (!string.IsNullOrWhiteSpace(ht.MatKhau))
            {
                query = @"
                    UPDATE HeThong
                    SET tenDangNhap = @tenDangNhap,
                        matKhau = @matKhau,
                        vaiTro = @vaiTro,
                        trangThai = @trangThai,
                        quyenHan = @quyenHan
                    WHERE maTaiKhoan = @maTaiKhoan";

                param = new SqlParameter[] {
                    new SqlParameter("@maTaiKhoan", ht.MaTaiKhoan),
                    new SqlParameter("@tenDangNhap", ht.TenDangNhap),
                    new SqlParameter("@matKhau", ht.MatKhau),
                    new SqlParameter("@vaiTro", (object?)ht.VaiTro ?? "Nhân viên"),
                    new SqlParameter("@trangThai", ht.TrangThai),
                    new SqlParameter("@quyenHan", (object?)ht.QuyenHan ?? "ALL")
                };
            }
            else
            {
                query = @"
                    UPDATE HeThong
                    SET tenDangNhap = @tenDangNhap,
                        vaiTro = @vaiTro,
                        trangThai = @trangThai,
                        quyenHan = @quyenHan
                    WHERE maTaiKhoan = @maTaiKhoan";

                param = new SqlParameter[] {
                    new SqlParameter("@maTaiKhoan", ht.MaTaiKhoan),
                    new SqlParameter("@tenDangNhap", ht.TenDangNhap),
                    new SqlParameter("@vaiTro", (object?)ht.VaiTro ?? "Nhân viên"),
                    new SqlParameter("@trangThai", ht.TrangThai),
                    new SqlParameter("@quyenHan", (object?)ht.QuyenHan ?? "ALL")
                };
            }

            return DatabaseHelper.ExecuteNonQuery(query, param) > 0;
        }

        public bool Delete(string maTaiKhoan)
        {
            string query = "DELETE FROM HeThong WHERE maTaiKhoan = @maTaiKhoan";
            SqlParameter[] param = { new SqlParameter("@maTaiKhoan", maTaiKhoan) };
            return DatabaseHelper.ExecuteNonQuery(query, param) > 0;
        }

        public bool UpdatePassword(string maTaiKhoan, string newPassword)
        {
            string query = "UPDATE HeThong SET matKhau = @matKhau WHERE maTaiKhoan = @maTaiKhoan";
            SqlParameter[] param = {
                new SqlParameter("@maTaiKhoan", maTaiKhoan),
                new SqlParameter("@matKhau", newPassword)
            };
            return DatabaseHelper.ExecuteNonQuery(query, param) > 0;
        }

        public bool ToggleStatus(string maTaiKhoan, string newStatus)
        {
            string query = "UPDATE HeThong SET trangThai = @trangThai WHERE maTaiKhoan = @maTaiKhoan";
            SqlParameter[] param = {
                new SqlParameter("@maTaiKhoan", maTaiKhoan),
                new SqlParameter("@trangThai", newStatus)
            };
            return DatabaseHelper.ExecuteNonQuery(query, param) > 0;
        }

        public void UpdateLastLogin(string maTaiKhoan)
        {
            string query = "UPDATE HeThong SET lanDangNhapCuoi = GETDATE() WHERE maTaiKhoan = @maTaiKhoan";
            SqlParameter[] param = { new SqlParameter("@maTaiKhoan", maTaiKhoan) };
            DatabaseHelper.ExecuteNonQuery(query, param);
        }

        public string GenerateNextId()
        {
            string query = "SELECT MAX(CAST(SUBSTRING(maTaiKhoan, 3, LEN(maTaiKhoan) - 2) AS INT)) FROM HeThong WHERE maTaiKhoan LIKE 'TK%' AND ISNUMERIC(SUBSTRING(maTaiKhoan, 3, LEN(maTaiKhoan) - 2)) = 1";
            object? result = DatabaseHelper.ExecuteScalar(query);
            int nextNumber = 1;
            if (result != null && result != DBNull.Value)
            {
                nextNumber = Convert.ToInt32(result) + 1;
            }
            return $"TK{nextNumber:D3}";
        }

        public void EnsureDefaultAdmin()
        {
            string query = "SELECT COUNT(*) FROM HeThong";
            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
            if (count == 0)
            {
                string getFirstNVSql = "SELECT TOP 1 ID_NV FROM NhanVien ORDER BY ID_NV";
                object? firstNv = DatabaseHelper.ExecuteScalar(getFirstNVSql);
                string idNv = firstNv != null ? firstNv.ToString()! : "NV001";

                string insertAdmin = @"
                    INSERT INTO HeThong (maTaiKhoan, ID_NV, tenDangNhap, matKhau, vaiTro, trangThai, lanDangNhapCuoi, quyenHan)
                    VALUES ('TK001', @idNv, 'admin', 'admin', N'Quản trị viên', N'Hoạt động', GETDATE(), 'ALL')";

                SqlParameter[] param = { new SqlParameter("@idNv", idNv) };
                DatabaseHelper.ExecuteNonQuery(insertAdmin, param);
            }
        }

        public bool CheckUsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM HeThong WHERE tenDangNhap = @username";
            SqlParameter[] param = { new SqlParameter("@username", username) };
            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, param));
            return count > 0;
        }

        private HeThongDTO MapRowToHeThong(DataRow row)
        {
            return new HeThongDTO
            {
                MaTaiKhoan = row["maTaiKhoan"]?.ToString() ?? "",
                ID_NV = row["ID_NV"]?.ToString() ?? "",
                TenNV = row.Table.Columns.Contains("TenNV") && row["TenNV"] != DBNull.Value ? row["TenNV"].ToString() : "",
                TenDangNhap = row["tenDangNhap"]?.ToString() ?? "",
                MatKhau = row["matKhau"]?.ToString() ?? "",
                VaiTro = row["vaiTro"] != DBNull.Value ? row["vaiTro"].ToString() : "Nhân viên",
                TrangThai = row["trangThai"]?.ToString() ?? "Hoạt động",
                LanDangNhapCuoi = DatabaseHelper.ToNullableDateTime(row["lanDangNhapCuoi"]),
                QuyenHan = row["quyenHan"] != DBNull.Value ? row["quyenHan"].ToString() : "ALL"
            };
        }
    }
}
