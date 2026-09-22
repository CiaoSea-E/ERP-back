using System;
using System.Collections.Generic;
using System.Data;
using HR_Management.DAL;
using HR_Management.DTO;

namespace HR_Management.BLL
{
    public class ThongKeBaoCaoBLL
    {
        private readonly ThongKeBaoCaoDAL _dal = new ThongKeBaoCaoDAL();

        public DashboardOverviewDTO GetOverview()
        {
            return _dal.GetOverview();
        }

        public DataTable GetDepartmentStats()
        {
            return _dal.GetDepartmentStats();
        }

        public DataTable GetEducationStats()
        {
            return _dal.GetEducationStats();
        }

        public List<ThongKeBaoCaoDTO> GetAllReports()
        {
            return _dal.GetAllReports();
        }

        public string GetNextId()
        {
            return _dal.GenerateNextId();
        }

        public bool CreateReport(ThongKeBaoCaoDTO dto, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(dto.TenBaoCao))
            {
                error = "Tên báo cáo không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.MaBaoCao))
            {
                dto.MaBaoCao = _dal.GenerateNextId();
            }

            try
            {
                return _dal.InsertReport(dto);
            }
            catch (Exception ex)
            {
                error = "Lỗi lưu báo cáo: " + ex.Message;
                return false;
            }
        }
    }
}
