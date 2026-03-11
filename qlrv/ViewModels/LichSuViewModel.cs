using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using qlrv.Helpers;
using qlrv.Models;
using qlrv.Services;
using qlrv.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace qlrv.ViewModels
{
    public class LichSuViewModel : BaseViewModel
    {
        #region ===== DANH SÁCH =====
        public ObservableCollection<dynamic> LichSu { get; set; } = new();
        #endregion

        #region ===== TÌM KIẾM =====
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadLichSu();
            }
        }
        #endregion

        #region ===== HIỂN THỊ FORM =====
        private bool _isFormVisible;
        public bool FormVisible => _isFormVisible;
        public Visibility FormVisibility => _isFormVisible ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ListVisibility => _isFormVisible ? Visibility.Collapsed : Visibility.Visible;

        private object _formView;
        public object FormView
        {
            get => _formView;
            set { _formView = value; OnPropertyChanged(); }
        }

        private void DongForm()
        {
            _isFormVisible = false;
            FormView = null;
            OnPropertyChanged(nameof(FormVisibility));
            OnPropertyChanged(nameof(ListVisibility));
            LoadLichSu();
        }
        #endregion

        #region ===== COMMAND =====
        public RelayCommand XoaCommand { get; set; }
        public RelayCommand SuaCommand { get; set; }
        public RelayCommand XuatExcelCommand { get; set; }
        public RelayCommand TimKiemCommand { get; set; }
        #endregion

        public LichSuViewModel()
        {
            LoadLichSu();

            // XÓA
            XoaCommand = new RelayCommand(o =>
            {
                if (o == null) return;

                dynamic ls = o;
                if (MessageBox.Show("Bạn có chắc muốn xóa bản ghi này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    return;

                using var conn = DatabaseService.GetConnection();
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM LichSuRaVao WHERE ID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", (int)ls.ID);
                cmd.ExecuteNonQuery();

                LoadLichSu();
            });

            // SỬA
            SuaCommand = new RelayCommand(o =>
            {
                if (o == null) return;

                FormView = new ThemSuaLichSuView
                {
                    DataContext = new ThemSuaLichSuViewModel(o, DongForm)
                };

                _isFormVisible = true;
                OnPropertyChanged(nameof(FormVisibility));
                OnPropertyChanged(nameof(ListVisibility));
            });

            // XUẤT EXCEL
            XuatExcelCommand = new RelayCommand(o =>
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Excel (*.xlsx)|*.xlsx",
                    FileName = "BaoCaoLichSuRaVao.xlsx"
                };

                if (sfd.ShowDialog() != true) return;

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("LichSu");

                // Tiêu đề
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Số CCCD";
                ws.Cell(1, 3).Value = "Họ tên";
                ws.Cell(1, 4).Value = "Loại đối tượng";
                ws.Cell(1, 5).Value = "Loại ra/vào";
                ws.Cell(1, 6).Value = "Thời gian";
                ws.Cell(1, 7).Value = "Phương tiện";
                ws.Cell(1, 8).Value = "Biển số";
                ws.Cell(1, 9).Value = "Người trực gác";
                ws.Cell(1, 10).Value = "Đơn vị người trực";

                int row = 2;
                foreach (dynamic item in LichSu)
                {
                    ws.Cell(row, 1).Value = item.ID;
                    ws.Cell(row, 2).Value = item.CCCD;
                    ws.Cell(row, 3).Value = item.HoTen;
                    ws.Cell(row, 4).Value = item.LoaiDoiTuong;
                    ws.Cell(row, 5).Value = item.LoaiRaVao;
                    ws.Cell(row, 6).Value = item.ThoiGian;
                    ws.Cell(row, 7).Value = item.PhuongTien;
                    ws.Cell(row, 8).Value = item.BienSo;
                    ws.Cell(row, 9).Value = item.NguoiTrucGac;
                    ws.Cell(row, 10).Value = item.DonViNguoiTruc;
                    row++;
                }

                wb.SaveAs(sfd.FileName);
                MessageBox.Show("Xuất Excel thành công!");
            });

            // TÌM KIẾM
            TimKiemCommand = new RelayCommand(o => LoadLichSu());
        }

        #region ===== LOAD DATA =====
        private void LoadLichSu()
        {
            LichSu.Clear();
            using var conn = DatabaseService.GetConnection();
            conn.Open();

            string sql = @"
                SELECT ls.*, 
                    CASE 
                        WHEN ls.LoaiDoiTuong='Quân Nhân' THEN qn.HoTen
                        WHEN ls.LoaiDoiTuong='Khách' THEN kh.HoTen
                    END AS HoTen
                FROM LichSuRaVao ls
                LEFT JOIN QuanNhan qn ON ls.CCCD=qn.CCCD AND ls.LoaiDoiTuong='Quân Nhân'
                LEFT JOIN Khach kh ON ls.CCCD=kh.CCCD AND ls.LoaiDoiTuong='Khách'
                WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                sql += " AND (ls.CCCD LIKE @Search OR ls.LoaiDoiTuong LIKE @Search OR ls.LoaiRaVao LIKE @Search)";
            }

            sql += " ORDER BY ls.ThoiGian DESC";

            var cmd = new SqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                cmd.Parameters.AddWithValue("@Search", $"%{SearchText}%");
            }

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                LichSu.Add(new
                {
                    ID = (int)reader["ID"],
                    CCCD = reader["CCCD"].ToString(),
                    LoaiDoiTuong = reader["LoaiDoiTuong"].ToString(),
                    LoaiRaVao = reader["LoaiRaVao"].ToString(),
                    ThoiGian = (DateTime)reader["ThoiGian"],
                    PhuongTien = reader["PhuongTien"].ToString(),
                    BienSo = reader["BienSo"].ToString(),
                    NguoiTrucGac = reader["NguoiTrucGac"].ToString(),
                    DonViNguoiTruc = reader["DonViNguoiTruc"].ToString(),
                    HoTen = reader["HoTen"].ToString()
                });
            }
        }
        #endregion
    }
}