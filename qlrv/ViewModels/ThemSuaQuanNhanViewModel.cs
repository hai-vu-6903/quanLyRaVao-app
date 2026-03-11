using Microsoft.Data.SqlClient;
using qlrv.Models;
using qlrv.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using qlrv.Helpers;

namespace qlrv.ViewModels
{
    public class ThemSuaQuanNhanViewModel : BaseViewModel
    {
        private readonly bool _isThemMoi;
        public bool IsLoaiRaVaoEnabled => _isThemMoi;

        private readonly Action _onClose;

        #region ===== FORM PROPERTIES =====

        public string Title { get; }

        public string CCCD { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; } = DateTime.Now;
        public string CapBac { get; set; }
        public string ChucVu { get; set; }
        public string DonVi { get; set; }
        public string QueQuan { get; set; }

        public string LoaiRaVao { get; set; }
        public string PhuongTien { get; set; }
        public string BienSo { get; set; }
        public string NguoiTrucGac { get; set; }
        public string DonViNguoiTruc { get; set; }

        public List<string> GioiTinhList { get; } = new() { "Nam", "Nữ" };
        public List<string> LoaiRaVaoList { get; } = new() { "Vào", "Ra" };
        public List<string> PhuongTienList { get; }
            = new() { "Không", "Xe máy", "Ô tô", "Xe quân sự" };

        #endregion

        #region ===== COMMANDS =====

        public RelayCommand LuuCommand { get; }
        public RelayCommand HuyCommand { get; }

        #endregion

        public ThemSuaQuanNhanViewModel(bool isThemMoi, dynamic quanNhan, Action onClose)
        {
            _isThemMoi = isThemMoi;
            _onClose = onClose;

            Title = _isThemMoi ? "THÊM QUÂN NHÂN" : "SỬA QUÂN NHÂN";

            if (!_isThemMoi && quanNhan != null)
            {
                LoadQuanNhan(quanNhan);
                LoadLichSuMoiNhat(quanNhan.CCCD);
            }

            LuuCommand = new RelayCommand(_ => Luu());
            HuyCommand = new RelayCommand(_ => _onClose?.Invoke());
        }

        private void LoadQuanNhan(dynamic qn)
        {
            CCCD = qn.CCCD;
            HoTen = qn.HoTen;
            GioiTinh = qn.GioiTinh;
            NgaySinh = qn.NgaySinh;
            CapBac = qn.CapBac;
            ChucVu = qn.ChucVu;
            DonVi = qn.DonVi;
            QueQuan = qn.QueQuan;
        }

        private void Luu()
        {
            if (string.IsNullOrWhiteSpace(CCCD) || string.IsNullOrWhiteSpace(HoTen))
            {
                MessageBox.Show("CCCD và Họ tên không được để trống");
                return;
            }

            using var conn = DatabaseService.GetConnection();
            conn.Open();

            if (_isThemMoi)
                ThemMoi(conn);
            else
                CapNhat(conn);

            _onClose?.Invoke();
        }

        private void ThemMoi(SqlConnection conn)
        {
            var cmd = new SqlCommand(@"
                INSERT INTO QuanNhan(CCCD,HoTen,GioiTinh,NgaySinh,QueQuan,CapBac,ChucVu,DonVi)
                VALUES(@CCCD,@HoTen,@GioiTinh,@NgaySinh,@QueQuan,@CapBac,@ChucVu,@DonVi)", conn);

            cmd.Parameters.AddWithValue("@CCCD", CCCD);
            cmd.Parameters.AddWithValue("@HoTen", HoTen);
            cmd.Parameters.AddWithValue("@GioiTinh", GioiTinh ?? "");
            cmd.Parameters.AddWithValue("@NgaySinh", NgaySinh);
            cmd.Parameters.AddWithValue("@QueQuan", QueQuan ?? "");
            cmd.Parameters.AddWithValue("@CapBac", CapBac ?? "");
            cmd.Parameters.AddWithValue("@ChucVu", ChucVu ?? "");
            cmd.Parameters.AddWithValue("@DonVi", DonVi ?? "");

            cmd.ExecuteNonQuery();

            var ls = new SqlCommand(@"
                INSERT INTO LichSuRaVao
                (CCCD,LoaiDoiTuong,LoaiRaVao,ThoiGian,PhuongTien,BienSo,NguoiTrucGac,DonViNguoiTruc)
                VALUES(@CCCD,N'Quân Nhân',@LoaiRaVao,GETDATE(),
                       @PhuongTien,@BienSo,@NguoiTrucGac,@DonViNguoiTruc)", conn);

            ls.Parameters.AddWithValue("@CCCD", CCCD);
            ls.Parameters.AddWithValue("@LoaiRaVao", LoaiRaVao ?? "Vào");
            ls.Parameters.AddWithValue("@PhuongTien", PhuongTien ?? "Không");
            ls.Parameters.AddWithValue("@BienSo", BienSo ?? "");
            ls.Parameters.AddWithValue("@NguoiTrucGac", NguoiTrucGac ?? "");
            ls.Parameters.AddWithValue("@DonViNguoiTruc", DonViNguoiTruc ?? "");

            ls.ExecuteNonQuery();
        }

        private void CapNhat(SqlConnection conn)
        {
            var cmd = new SqlCommand(@"
                UPDATE QuanNhan SET
                    HoTen=@HoTen,GioiTinh=@GioiTinh,NgaySinh=@NgaySinh,
                    QueQuan=@QueQuan,CapBac=@CapBac,
                    ChucVu=@ChucVu,DonVi=@DonVi
                WHERE CCCD=@CCCD", conn);

            cmd.Parameters.AddWithValue("@CCCD", CCCD);
            cmd.Parameters.AddWithValue("@HoTen", HoTen);
            cmd.Parameters.AddWithValue("@GioiTinh", GioiTinh ?? "");
            cmd.Parameters.AddWithValue("@NgaySinh", NgaySinh);
            cmd.Parameters.AddWithValue("@QueQuan", QueQuan ?? "");
            cmd.Parameters.AddWithValue("@CapBac", CapBac ?? "");
            cmd.Parameters.AddWithValue("@ChucVu", ChucVu ?? "");
            cmd.Parameters.AddWithValue("@DonVi", DonVi ?? "");

            cmd.ExecuteNonQuery();
        }

        private void LoadLichSuMoiNhat(string cccd)
        {
            using var conn = DatabaseService.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        SELECT TOP 1 *
        FROM LichSuRaVao
        WHERE CCCD = @CCCD
          AND LoaiDoiTuong = N'Quân Nhân'
        ORDER BY ThoiGian DESC", conn);

            cmd.Parameters.AddWithValue("@CCCD", cccd);

            var rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                LoaiRaVao = rd["LoaiRaVao"].ToString();
                PhuongTien = rd["PhuongTien"].ToString();
                BienSo = rd["BienSo"].ToString();
                NguoiTrucGac = rd["NguoiTrucGac"].ToString();
                DonViNguoiTruc = rd["DonViNguoiTruc"].ToString();

                OnPropertyChanged(nameof(LoaiRaVao));
                OnPropertyChanged(nameof(PhuongTien));
                OnPropertyChanged(nameof(BienSo));
                OnPropertyChanged(nameof(NguoiTrucGac));
                OnPropertyChanged(nameof(DonViNguoiTruc));
            }
        }


        public void TimQuanNhanTheoCCCD()
        {
            if (string.IsNullOrWhiteSpace(CCCD))
                return;

            using var conn = DatabaseService.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT * FROM QuanNhan WHERE CCCD = @CCCD", conn);
            cmd.Parameters.AddWithValue("@CCCD", CCCD);

            var rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                HoTen = rd["HoTen"].ToString();
                GioiTinh = rd["GioiTinh"].ToString();
                NgaySinh = (DateTime)rd["NgaySinh"];
                QueQuan = rd["QueQuan"].ToString();
                CapBac = rd["CapBac"].ToString();
                ChucVu = rd["ChucVu"].ToString();
                DonVi = rd["DonVi"].ToString();

                OnPropertyChanged(nameof(HoTen));
                OnPropertyChanged(nameof(GioiTinh));
                OnPropertyChanged(nameof(NgaySinh));
                OnPropertyChanged(nameof(QueQuan));
                OnPropertyChanged(nameof(CapBac));
                OnPropertyChanged(nameof(ChucVu));
                OnPropertyChanged(nameof(DonVi));
            }
        }

    }
}