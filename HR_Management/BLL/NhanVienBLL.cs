using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HR_Management.DAL;
using HR_Management.DTO;

namespace HR_Management.BLL
{
    public class NhanVienBLL
    {
        private readonly NhanVienDAL _dal = new NhanVienDAL();

        public List<NhanVienChiTietDTO> GetAll()
        {
            return _dal.GetAll();
        }

        public NhanVienChiTietDTO? GetById(string idNv)
        {
            if (string.IsNullOrWhiteSpace(idNv)) return null;
            return _dal.GetById(idNv);
        }

        public List<NhanVienChiTietDTO> Search(string keyword, string? maPhongBan, string? trangThai)
        {
            return _dal.Search(keyword, maPhongBan, trangThai);
        }

        public string GetNextId()
        {
            return _dal.GenerateNextId();
        }

        public bool Save(NhanVienChiTietDTO dto, bool isNew, out string error)
        {
            error = string.Empty;

            // 1. Kiểm tra Mã nhân viên
            if (string.IsNullOrWhiteSpace(dto.ID_NV))
            {
                error = "Mã nhân viên không được để trống!";
                return false;
            }

            if (isNew && _dal.CheckIdExists(dto.ID_NV.Trim()))
            {
                error = $"Mã nhân viên '{dto.ID_NV.Trim()}' đã tồn tại trong hệ thống!";
                return false;
            }

            // 2. Kiểm tra Họ và tên
            if (string.IsNullOrWhiteSpace(dto.TenNV))
            {
                error = "Họ và tên nhân viên không được để trống!";
                return false;
            }

            // 3. Kiểm tra Phòng ban
            if (string.IsNullOrWhiteSpace(dto.MaPhongBan))
            {
                error = "Vui lòng chọn phòng ban trực thuộc cho nhân viên!";
                return false;
            }

            // 4. Kiểm tra Chức vụ
            if (string.IsNullOrWhiteSpace(dto.ChucVu))
            {
                error = "Chức vụ của nhân viên không được để trống!";
                return false;
            }

            // 5. Bắt lỗi Số điện thoại: không được để trống, không được nhập chữ, phải đủ 10 số, không được trùng
            if (string.IsNullOrWhiteSpace(dto.SoDienThoai))
            {
                error = "Số điện thoại không được để trống!";
                return false;
            }

            string phone = dto.SoDienThoai.Trim();

            if (!Regex.IsMatch(phone, @"^[0-9]+$"))
            {
                error = "Số điện thoại chỉ được chứa các chữ số, không được chứa chữ cái hoặc ký tự đặc biệt!";
                return false;
            }

            if (phone.Length != 10)
            {
                error = $"Số điện thoại phải gồm đúng 10 chữ số (bạn đang nhập {phone.Length} số)!";
                return false;
            }

            if (!phone.StartsWith("0"))
            {
                error = "Số điện thoại không hợp lệ! Số điện thoại tại Việt Nam phải bắt đầu bằng chữ số 0 (ví dụ: 0912345678).";
                return false;
            }

            if (_dal.CheckPhoneExists(phone, isNew ? null : dto.ID_NV.Trim()))
            {
                error = $"Số điện thoại '{phone}' đã tồn tại trong hệ thống (đã đăng ký cho nhân viên khác)! Vui lòng kiểm tra lại.";
                return false;
            }

            // 6. Kiểm tra Email: không được để trống, đúng định dạng, không được trùng
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                error = "Địa chỉ email không được để trống!";
                return false;
            }

            string email = dto.Email.Trim();
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                error = "Địa chỉ email không đúng định dạng (ví dụ: nhanvien@congty.com)!";
                return false;
            }

            if (_dal.CheckEmailExists(email, isNew ? null : dto.ID_NV.Trim()))
            {
                error = $"Địa chỉ email '{email}' đã tồn tại trong hệ thống (thuộc nhân viên khác)!";
                return false;
            }

            // 7. Kiểm tra Địa chỉ liên hệ
            if (string.IsNullOrWhiteSpace(dto.DiaChi))
            {
                error = "Địa chỉ liên hệ không được để trống!";
                return false;
            }

            // 8. Kiểm tra CCCD (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(dto.SoCCCD))
            {
                string cccd = dto.SoCCCD.Trim();
                if (!Regex.IsMatch(cccd, @"^[0-9]+$"))
                {
                    error = "Số CCCD chỉ được chứa chữ số, không được chứa chữ cái hoặc ký tự đặc biệt!";
                    return false;
                }

                if (cccd.Length != 12)
                {
                    error = $"Số CCCD phải gồm đúng 12 chữ số (bạn đang nhập {cccd.Length} số)!";
                    return false;
                }

                if (_dal.CheckCccdExists(cccd, isNew ? null : dto.ID_NV.Trim()))
                {
                    error = $"Số CCCD '{cccd}' đã tồn tại trong hệ thống (thuộc nhân viên khác)! Vui lòng kiểm tra lại.";
                    return false;
                }
            }

            // 9. Kiểm tra Lương cơ bản
            if (dto.LuongCoBan < 0)
            {
                error = "Mức lương cơ bản không được âm!";
                return false;
            }

            if (isNew)
            {
                return _dal.InsertWithDetails(dto, out error);
            }
            else
            {
                return _dal.UpdateWithDetails(dto, out error);
            }
        }

        public bool HasRelatedTransactions(string idNv, out string details)
        {
            if (string.IsNullOrWhiteSpace(idNv))
            {
                details = string.Empty;
                return false;
            }
            return _dal.HasRelatedTransactions(idNv, out details);
        }

        public bool Deactivate(string idNv, out string error)
        {
            if (string.IsNullOrWhiteSpace(idNv))
            {
                error = "Vui lòng chọn nhân viên cần chuyển trạng thái!";
                return false;
            }
            return _dal.Deactivate(idNv, out error);
        }

        public bool Delete(string idNv, out string error)
        {
            if (string.IsNullOrWhiteSpace(idNv))
            {
                error = "Vui lòng chọn nhân viên cần xóa!";
                return false;
            }
            return _dal.Delete(idNv, out error);
        }
    }
}
