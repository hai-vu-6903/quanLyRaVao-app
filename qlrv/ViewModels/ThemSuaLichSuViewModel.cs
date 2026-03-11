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
    public class ThemSuaLichSuViewModel : BaseViewModel
    {
        public dynamic LichSu { get; set; } // nhận object từ danh sách
        private readonly Action _onClose;

        // các trường được phép sửa
        public string PhuongTien
        {
            get => LichSu.PhuongTien;
            set { LichSu.PhuongTien = value; OnPropertyChanged(); }
        }
        public string BienSo
        {
            get => LichSu.BienSo;
            set { LichSu.BienSo = value; OnPropertyChanged(); }
        }
        public string NguoiTrucGac
        {
            get => LichSu.NguoiTrucGac;
            set { LichSu.NguoiTrucGac = value; OnPropertyChanged(); }
        }
        public string DonViNguoiTruc
        {
            get => LichSu.DonViNguoiTruc;
            set { LichSu.DonViNguoiTruc = value; OnPropertyChanged(); }
        }

        public RelayCommand LuuCommand { get; }
        public RelayCommand HuyCommand { get; }

        public ThemSuaLichSuViewModel(dynamic lichSu, Action onClose)
        {
            LichSu = lichSu;
            _onClose = onClose;

            LuuCommand = new RelayCommand(_ => Luu());
            HuyCommand = new RelayCommand(_ => _onClose?.Invoke());
        }

        private void Luu()
        {
            try
            {
                using var conn = DatabaseService.GetConnection();
                conn.Open();

                string sql = @"
                    UPDATE LichSuRaVao
                    SET PhuongTien=@PhuongTien,
                        BienSo=@BienSo,
                        NguoiTrucGac=@NguoiTrucGac,
                        DonViNguoiTruc=@DonViNguoiTruc
                    WHERE ID=@ID";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PhuongTien", PhuongTien);
                cmd.Parameters.AddWithValue("@BienSo", BienSo);
                cmd.Parameters.AddWithValue("@NguoiTrucGac", NguoiTrucGac);
                cmd.Parameters.AddWithValue("@DonViNguoiTruc", DonViNguoiTruc);
                cmd.Parameters.AddWithValue("@ID", (int)LichSu.ID);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Cập nhật thành công!");
                _onClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }
    }
}
