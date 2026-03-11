using System;

namespace qlrv.Models
{
    public class QuanNhan
    {
        public string CCCD { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string QueQuan { get; set; }
        public string CapBac { get; set; }
        public string ChucVu { get; set; }
        public string DonVi { get; set; }
        public DateTime? ThoiGianVao { get; set; } // Thêm mới
        public DateTime? ThoiGianRa { get; set; } // Thêm mới
        public string PhuongTien { get; set; } // Thêm mới
        public string TrangThai { get; set; } // Thêm mới
    }
}