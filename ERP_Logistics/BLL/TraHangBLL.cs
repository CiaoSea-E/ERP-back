using System;
using System.Data;
using ERP.DAL;

namespace ERP.BLL
{
    public class TraHangBLL
    {
        private readonly TraHangDAL dal;

        public TraHangBLL(string connectionString)
        {
            dal = new TraHangDAL(connectionString);
        }

        public DataTable GetDanhSachTraHang()
        {
            return dal.GetDanhSachTraHang();
        }

        public void CapNhatTrangThaiTra(int idYeuCau, string trangThai)
        {
            if (idYeuCau <= 0) throw new ArgumentException("Mã yêu cầu không hợp lệ.");
            if (String.IsNullOrWhiteSpace(trangThai)) throw new ArgumentException("Trạng thái không được để trống.");
            dal.CapNhatTrangThaiTra(idYeuCau, trangThai);
        }
    }
}
