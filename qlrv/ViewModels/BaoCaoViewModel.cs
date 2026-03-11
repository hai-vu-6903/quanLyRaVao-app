using Microsoft.Data.SqlClient;
using qlrv.Models;
using qlrv.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using ClosedXML.Excel;
using qlrv.Helpers;

namespace qlrv.ViewModels
{
    public class BaoCaoViewModel : BaseViewModel
    {
        public ObservableCollection<dynamic> LichSu { get; set; } = new ObservableCollection<dynamic>();

        private DateTime? _tuNgay;
        public DateTime? TuNgay
        {
            get => _tuNgay;
            set { _tuNgay = value; OnPropertyChanged(); }
        }

        private DateTime? _denNgay;
        public DateTime? DenNgay
        {
            get => _denNgay;
            set { _denNgay = value; OnPropertyChanged(); }
        }

        public RelayCommand LocCommand { get; set; }
        public RelayCommand XemTatCaCommand { get; set; }
        public RelayCommand XuatExcelCommand { get; set; }

        public BaoCaoViewModel()
        {
            LocCommand = new RelayCommand(_ => LoadLichSu());

            XemTatCaCommand = new RelayCommand(_ =>
            {
                TuNgay = null;
                DenNgay = null;
                LoadLichSu();
            });

            XuatExcelCommand = new RelayCommand(_ => XuatExcel());

            LoadLichSu();
        }

        private void LoadLichSu()
        {
            LichSu.Clear();

            using (var conn = DatabaseService.GetConnection())
            {
                conn.Open();

                string sql = @"
SELECT
    ls.CCCD,
    ls.LoaiDoiTuong,
    ls.LoaiRaVao,
    ls.ThoiGian,
    ls.PhuongTien,
    ls.BienSo,
    ls.NguoiTrucGac,
    ls.DonViNguoiTruc,

    COALESCE(qn.HoTen, kh.HoTen, N'')       AS HoTen,
    COALESCE(qn.QueQuan, kh.QueQuan, N'')  AS QueQuan,
    COALESCE(qn.CapBac, N'')               AS CapBac,
    COALESCE(qn.ChucVu, N'')               AS ChucVu,
    COALESCE(kh.DienThoai, N'')            AS DienThoai,
    COALESCE(kh.NguoiBaoLanh, N'')          AS NguoiBaoLanh

FROM LichSuRaVao ls

LEFT JOIN QuanNhan qn
    ON LTRIM(RTRIM(ls.CCCD)) = LTRIM(RTRIM(qn.CCCD))
    AND ls.LoaiDoiTuong = N'Quân Nhân'

LEFT JOIN Khach kh
    ON LTRIM(RTRIM(ls.CCCD)) = LTRIM(RTRIM(kh.CCCD))
    AND ls.LoaiDoiTuong = N'Khách'

WHERE 1 = 1";

                if (TuNgay.HasValue)
                    sql += " AND ls.ThoiGian >= @TuNgay";

                if (DenNgay.HasValue)
                    sql += " AND ls.ThoiGian <= @DenNgay";

                sql += " ORDER BY ls.ThoiGian DESC";

                var cmd = new SqlCommand(sql, conn);

                if (TuNgay.HasValue)
                    cmd.Parameters.AddWithValue("@TuNgay", TuNgay.Value);

                if (DenNgay.HasValue)
                    cmd.Parameters.AddWithValue("@DenNgay", DenNgay.Value);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LichSu.Add(new
                    {
                        CCCD = reader["CCCD"].ToString(),
                        HoTen = reader["HoTen"].ToString(),
                        LoaiDoiTuong = reader["LoaiDoiTuong"].ToString(),
                        LoaiRaVao = reader["LoaiRaVao"].ToString(),
                        ThoiGian = (DateTime)reader["ThoiGian"],
                        PhuongTien = reader["PhuongTien"].ToString(),
                        BienSo = reader["BienSo"].ToString(),
                        NguoiTrucGac = reader["NguoiTrucGac"].ToString(),
                        DonViNguoiTruc = reader["DonViNguoiTruc"].ToString(),
                        QueQuan = reader["QueQuan"].ToString(),
                        DienThoai = reader["DienThoai"].ToString(),
                        NguoiBaoLanh = reader["NguoiBaoLanh"].ToString(),
                        CapBac = reader["CapBac"].ToString(),
                        ChucVu = reader["ChucVu"].ToString()
                    });
                }
            }
        }

        private void XuatExcel()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "BaoCaoRaVao.xlsx"
            };

            if (sfd.ShowDialog() != true) return;

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Báo cáo");

                string[] headers =
                {
                    "CCCD","Họ tên","Quê quán","SĐT","Loại đối tượng",
                    "Cấp bậc","Chức vụ","Ra/Vào","Thời gian","Người bảo lãnh",
                    "Phương tiện","Biển số","Người trực","Đơn vị trực"
                    
                };

                for (int i = 0; i < headers.Length; i++)
                    ws.Cell(1, i + 1).Value = headers[i];

                int row = 2;
                foreach (dynamic x in LichSu)
                {
                    ws.Cell(row, 1).Value = x.CCCD;
                    ws.Cell(row, 2).Value = x.HoTen;
                    ws.Cell(row, 3).Value = x.QueQuan;
                    ws.Cell(row, 4).Value = x.DienThoai;
                    ws.Cell(row, 5).Value = x.LoaiDoiTuong;
                    ws.Cell(row, 6).Value = x.CapBac;
                    ws.Cell(row, 7).Value = x.ChucVu;
                    ws.Cell(row, 8).Value = x.LoaiRaVao;
                    ws.Cell(row, 9).Value = x.ThoiGian;
                    ws.Cell(row, 10).Value = x.NguoiBaoLanh;
                    ws.Cell(row, 11).Value = x.PhuongTien;
                    ws.Cell(row, 12).Value = x.BienSo;
                    ws.Cell(row, 13).Value = x.NguoiTrucGac;
                    ws.Cell(row, 14).Value = x.DonViNguoiTruc;
                    row++;
                }

                wb.SaveAs(sfd.FileName);
                MessageBox.Show("Xuất Excel thành công!");
            }
        }
    }
}