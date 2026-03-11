using Microsoft.Data.SqlClient;
using qlrv.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using qlrv.Helpers;

namespace qlrv.ViewModels
{
    public class ThemSuaKhachViewModel : BaseViewModel
    {
        private readonly bool _isThemMoi;
        public bool IsLoaiRaVaoEnabled => _isThemMoi;

        private readonly Action _onClose;

        #region ===== PROPERTIES =====
        public string Title => _isThemMoi ? "THÊM KHÁCH" : "SỬA KHÁCH";

        public string CCCD { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; } = DateTime.Now;
        public string DienThoai { get; set; }
        public string QueQuan { get; set; }
        public string NguoiBaoLanh { get; set; }
        public string DonViNguoiBaoLanh { get; set; }
        public string MucDichVao { get; set; }

        public string LoaiRaVao { get; set; } = "Vào";
        public string PhuongTien { get; set; }
        public string BienSo { get; set; }
        public string NguoiTrucGac { get; set; }
        public string DonViNguoiTruc { get; set; }
        #endregion

        #region ===== LIST =====
        public List<string> GioiTinhList { get; } = new() { "Nam", "Nữ" };
        public List<string> LoaiRaVaoList { get; } = new() { "Vào", "Ra" };
        public List<string> PhuongTienList { get; } = new() { "Không", "Xe máy", "Ô tô" };
        #endregion

        #region ===== COMMAND =====
        public RelayCommand LuuCommand { get; }
        public RelayCommand HuyCommand { get; }
        #endregion

        public ThemSuaKhachViewModel(bool isThemMoi, dynamic khach, Action onClose)
        {
            _isThemMoi = isThemMoi;
            _onClose = onClose;

            if (!_isThemMoi && khach != null)
            {
                CCCD = khach.CCCD;
                HoTen = khach.HoTen;
                GioiTinh = khach.GioiTinh;
                NgaySinh = khach.NgaySinh;
                DienThoai = khach.DienThoai;
                QueQuan = khach.QueQuan;
                NguoiBaoLanh = khach.NguoiBaoLanh;
                DonViNguoiBaoLanh = khach.DonViNguoiBaoLanh;
                MucDichVao = khach.MucDichVao;

                LoadLichSuMoiNhat(CCCD);
            }

            LuuCommand = new RelayCommand(_ => Luu());
            HuyCommand = new RelayCommand(_ => _onClose?.Invoke());
        }

        #region ===== SAVE =====
        private void Luu()
        {
            if (string.IsNullOrWhiteSpace(CCCD) || string.IsNullOrWhiteSpace(HoTen))
            {
                MessageBox.Show("Thiếu CCCD hoặc Họ tên");
                return;
            }

            using var conn = DatabaseService.GetConnection();
            conn.Open();

            if (_isThemMoi)
            {
                // 1. Thêm vào Khach
                new SqlCommand(@"
            INSERT INTO Khach VALUES
            (@CCCD,@HoTen,@GioiTinh,@NgaySinh,@QueQuan,
             @DienThoai,@NguoiBaoLanh,@DonVi,@MucDich)", conn)
                {
                    Parameters =
            {
                new("@CCCD",CCCD),
                new("@HoTen",HoTen),
                new("@GioiTinh",GioiTinh),
                new("@NgaySinh",NgaySinh),
                new("@QueQuan",QueQuan),
                new("@DienThoai",DienThoai),
                new("@NguoiBaoLanh",NguoiBaoLanh),
                new("@DonVi",DonViNguoiBaoLanh),
                new("@MucDich",MucDichVao)
            }
                }.ExecuteNonQuery();

                // 2. Thêm bản ghi đầu tiên vào LichSuRaVao
                new SqlCommand(@"
            INSERT INTO LichSuRaVao
            (CCCD, LoaiDoiTuong, LoaiRaVao, PhuongTien, BienSo, NguoiTrucGac, DonViNguoiTruc, ThoiGian)
            VALUES
            (@CCCD, N'Khách', @LoaiRaVao, @PhuongTien, @BienSo, @NguoiTrucGac, @DonViNguoiTruc, @ThoiGian)", conn)
                {
                    Parameters =
            {
                new("@CCCD", CCCD),
                new("@LoaiRaVao", LoaiRaVao),
                new("@PhuongTien", PhuongTien),
                new("@BienSo", BienSo ?? ""),
                new("@NguoiTrucGac", NguoiTrucGac ?? ""),
                new("@DonViNguoiTruc", DonViNguoiTruc ?? ""),
                new("@ThoiGian", DateTime.Now)
            }
                }.ExecuteNonQuery();
            }
            else
            {
                // Cập nhật Khach
                new SqlCommand(@"
            UPDATE Khach SET
            HoTen=@HoTen,GioiTinh=@GioiTinh,NgaySinh=@NgaySinh,
            QueQuan=@QueQuan,DienThoai=@DienThoai,
            NguoiBaoLanh=@NguoiBaoLanh,
            DonViNguoiBaoLanh=@DonVi,
            MucDichVao=@MucDich
            WHERE CCCD=@CCCD", conn)
                {
                    Parameters =
            {
                new("@CCCD",CCCD),
                new("@HoTen",HoTen),
                new("@GioiTinh",GioiTinh),
                new("@NgaySinh",NgaySinh),
                new("@QueQuan",QueQuan),
                new("@DienThoai",DienThoai),
                new("@NguoiBaoLanh",NguoiBaoLanh),
                new("@DonVi",DonViNguoiBaoLanh),
                new("@MucDich",MucDichVao)
            }
                }.ExecuteNonQuery();

                // Nếu muốn sửa ra/vào → thêm bản ghi mới vào LichSuRaVao
                new SqlCommand(@"
            INSERT INTO LichSuRaVao
            (CCCD, LoaiDoiTuong, LoaiRaVao, PhuongTien, BienSo, NguoiTrucGac, DonViNguoiTruc, ThoiGian)
            VALUES
            (@CCCD, N'Khách', @LoaiRaVao, @PhuongTien, @BienSo, @NguoiTrucGac, @DonViNguoiTruc, @ThoiGian)", conn)
                {
                    Parameters =
            {
                new("@CCCD", CCCD),
                new("@LoaiRaVao", LoaiRaVao),
                new("@PhuongTien", PhuongTien),
                new("@BienSo", BienSo ?? ""),
                new("@NguoiTrucGac", NguoiTrucGac ?? ""),
                new("@DonViNguoiTruc", DonViNguoiTruc ?? ""),
                new("@ThoiGian", DateTime.Now)
            }
                }.ExecuteNonQuery();
            }

            _onClose?.Invoke();
        }

        #endregion

        private void LoadLichSuMoiNhat(string cccd)
        {
            using var conn = DatabaseService.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT TOP 1 * FROM LichSuRaVao
                WHERE CCCD=@CCCD AND LoaiDoiTuong=N'Khách'
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
            }
        }

        public void TimKhachTheoCCCD()
        {
            if (string.IsNullOrEmpty(CCCD))
                return;

            using (var conn = DatabaseService.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Khach WHERE CCCD = @CCCD", conn);
                cmd.Parameters.AddWithValue("@CCCD", CCCD);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    HoTen = reader["HoTen"].ToString();
                    GioiTinh = reader["GioiTinh"].ToString();
                    NgaySinh = (DateTime)reader["NgaySinh"];
                    QueQuan = reader["QueQuan"].ToString();
                    DienThoai = reader["DienThoai"].ToString();
                    NguoiBaoLanh = reader["NguoiBaoLanh"].ToString();
                    DonViNguoiBaoLanh = reader["DonViNguoiBaoLanh"].ToString();
                    MucDichVao = reader["MucDichVao"].ToString();

                    OnPropertyChanged(nameof(HoTen));
                    OnPropertyChanged(nameof(GioiTinh));
                    OnPropertyChanged(nameof(NgaySinh));
                    OnPropertyChanged(nameof(QueQuan));
                    OnPropertyChanged(nameof(DienThoai));
                    OnPropertyChanged(nameof(NguoiBaoLanh));
                    OnPropertyChanged(nameof(DonViNguoiBaoLanh));
                    OnPropertyChanged(nameof(MucDichVao));
                }
                reader.Close();
            }
        }
    }
}
