using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qlrv.Models
{
    public class LichSuRaVao
    {
        public int ID { get; set; }
        public string CCCD { get; set; }
        public string LoaiDoiTuong { get; set; } // QuanNhan / Khach
        public string LoaiRaVao { get; set; } // Ra / Vao
        public DateTime ThoiGian { get; set; }
        public string PhuongTien { get; set; }
        public string BienSo { get; set; }
        public string NguoiTrucGac { get; set; }
        public string DonViNguoiTruc { get; set; }
    }
}
