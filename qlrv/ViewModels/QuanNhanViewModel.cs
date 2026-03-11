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
using System.Windows.Controls;

namespace qlrv.ViewModels
{
    public class QuanNhanViewModel : BaseViewModel
    {
        #region ===== PROPERTIES =====

        private string _searchText;
        private bool _isLoaded;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                if (_isLoaded)
                    LoadDanhSach();
            }
        }

        public ObservableCollection<dynamic> DanhSachQuanNhan { get; set; }
            = new ObservableCollection<dynamic>();

        // Điều khiển hiển thị
        private Visibility _listVisibility = Visibility.Visible;
        public Visibility ListVisibility
        {
            get => _listVisibility;
            set
            {
                _listVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _formVisibility = Visibility.Collapsed;
        public Visibility FormVisibility
        {
            get => _formVisibility;
            set
            {
                _formVisibility = value;
                OnPropertyChanged();
            }
        }

        private UserControl _formView;
        public UserControl FormView
        {
            get => _formView;
            set
            {
                _formView = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ===== COMMANDS =====

        public RelayCommand ThemCommand { get; }
        public RelayCommand SuaCommand { get; }
        public RelayCommand XoaCommand { get; }
        public RelayCommand XuatExcelCommand { get; }
        public RelayCommand XoaTimKiemCommand { get; }

        #endregion

        public QuanNhanViewModel()
        {
            LoadDanhSach();
            _isLoaded = true;

            ThemCommand = new RelayCommand(_ => MoFormThem());
            SuaCommand = new RelayCommand(o => MoFormSua(o));
            XoaCommand = new RelayCommand(o => XoaQuanNhan(o));
            XuatExcelCommand = new RelayCommand(_ => XuatExcel());
            XoaTimKiemCommand = new RelayCommand(_ =>
            {
                SearchText = "";
                LoadDanhSach();
            });
        }

        #region ===== FORM HANDLING =====

        private void MoFormThem()
        {
            var vm = new ThemSuaQuanNhanViewModel(
                isThemMoi: true,
                quanNhan: null,
                onClose: DongForm);

            FormView = new ThemSuaQuanNhanView { DataContext = vm };
            ListVisibility = Visibility.Collapsed;
            FormVisibility = Visibility.Visible;
        }

        private void MoFormSua(object o)
        {
            if (o == null) return;

            var vm = new ThemSuaQuanNhanViewModel(
                isThemMoi: false,
                quanNhan: o,
                onClose: DongForm);

            FormView = new ThemSuaQuanNhanView { DataContext = vm };
            ListVisibility = Visibility.Collapsed;
            FormVisibility = Visibility.Visible;
        }

        private void DongForm()
        {
            FormView = null;
            FormVisibility = Visibility.Collapsed;
            ListVisibility = Visibility.Visible;
            LoadDanhSach();
        }

        #endregion

        #region ===== CRUD =====

        private void XoaQuanNhan(object o)
        {
            if (o == null) return;
            dynamic qn = o;

            if (MessageBox.Show(
                $"Bạn có chắc muốn xóa quân nhân {qn.HoTen}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            using (var conn = DatabaseService.GetConnection())
            {
                conn.Open();

                var cmd1 = new SqlCommand(
                    "DELETE FROM QuanNhan WHERE CCCD = @CCCD", conn);
                cmd1.Parameters.AddWithValue("@CCCD", qn.CCCD);
                cmd1.ExecuteNonQuery();

                var cmd2 = new SqlCommand(
                    "DELETE FROM LichSuRaVao WHERE CCCD = @CCCD", conn);
                cmd2.Parameters.AddWithValue("@CCCD", qn.CCCD);
                cmd2.ExecuteNonQuery();
            }

            MessageBox.Show("Đã xóa quân nhân thành công!");
            LoadDanhSach();
        }

        private void LoadDanhSach()
        {
            DanhSachQuanNhan.Clear();

            using (var conn = DatabaseService.GetConnection())
            {
                conn.Open();

                string query = @"
                SELECT qn.*,
                       ls.LoaiRaVao AS TrangThai,
                       ls.ThoiGian,
                       ls.PhuongTien,
                       ls.BienSo
                FROM QuanNhan qn
                LEFT JOIN (
                    SELECT CCCD, LoaiRaVao, ThoiGian, PhuongTien, BienSo,
                           ROW_NUMBER() OVER (PARTITION BY CCCD ORDER BY ThoiGian DESC) rn
                    FROM LichSuRaVao
                    WHERE LoaiDoiTuong = N'Quân Nhân'
                ) ls ON qn.CCCD = ls.CCCD AND ls.rn = 1
                WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(SearchText))
                    query += @" AND (qn.CCCD LIKE @S OR qn.HoTen LIKE @S 
                                  OR qn.DonVi LIKE @S OR qn.CapBac LIKE @S 
                                  OR qn.ChucVu LIKE @S)";

                query += " ORDER BY qn.HoTen";

                var cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrWhiteSpace(SearchText))
                    cmd.Parameters.AddWithValue("@S", $"%{SearchText}%");

                var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    DanhSachQuanNhan.Add(new
                    {
                        CCCD = rd["CCCD"].ToString(),
                        HoTen = rd["HoTen"].ToString(),
                        GioiTinh = rd["GioiTinh"].ToString(),
                        NgaySinh = (DateTime)rd["NgaySinh"],
                        QueQuan = rd["QueQuan"].ToString(),
                        CapBac = rd["CapBac"].ToString(),
                        ChucVu = rd["ChucVu"].ToString(),
                        DonVi = rd["DonVi"].ToString(),
                        TrangThai = rd["TrangThai"]?.ToString() ?? "Chưa có",
                        ThoiGian = rd["ThoiGian"] as DateTime?,
                        PhuongTien = rd["PhuongTien"]?.ToString(),
                        BienSo = rd["BienSo"]?.ToString()
                    });
                }
            }
        }

        private void XuatExcel()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "BaoCaoQuanNhan.xlsx"
            };

            if (sfd.ShowDialog() != true) return;

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("QuanNhan");

            ws.Cell(1, 1).Value = "STT";
            ws.Cell(1, 2).Value = "CCCD";
            ws.Cell(1, 3).Value = "Họ tên";
            ws.Cell(1, 4).Value = "Đơn vị";
            ws.Cell(1, 5).Value = "Cấp bậc";
            ws.Cell(1, 6).Value = "Chức vụ";

            int row = 2;
            int stt = 1;
            foreach (var qn in DanhSachQuanNhan)
            {
                ws.Cell(row, 1).Value = stt++;
                ws.Cell(row, 2).Value = qn.CCCD;
                ws.Cell(row, 3).Value = qn.HoTen;
                ws.Cell(row, 4).Value = qn.DonVi;
                ws.Cell(row, 5).Value = qn.CapBac;
                ws.Cell(row, 6).Value = qn.ChucVu;
                row++;
            }

            wb.SaveAs(sfd.FileName);
            MessageBox.Show("Xuất Excel thành công!");
        }

        #endregion
    }
}