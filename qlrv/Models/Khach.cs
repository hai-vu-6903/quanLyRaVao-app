using System;

namespace qlrv.Models
{
    public class Khach
    {
        public string CCCD { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string QueQuan { get; set; }
        public string DienThoai { get; set; }
        public string NguoiBaoLanh { get; set; }
        public string DonViNguoiBaoLanh { get; set; }
        public string MucDichVao { get; set; } // Thêm mới
        public string PhuongTien { get; set; } // Thêm mới
        public DateTime? ThoiGian { get; set; } // Thêm mới
        public string TrangThai { get; set; } // Thêm mới
    }
}