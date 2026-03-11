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
    public class KhachViewModel : BaseViewModel
    {
        #region ===== DANH SÁCH =====
        public ObservableCollection<dynamic> DanhSachKhach { get; } = new ObservableCollection<dynamic>();
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
                LoadDanhSach();
            }
        }
        #endregion

        #region ===== HIỂN THỊ FORM =====
        private bool _isFormVisible;

        public Visibility FormVisibility =>
            _isFormVisible ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ListVisibility =>
            _isFormVisible ? Visibility.Collapsed : Visibility.Visible;

        private object _formView;
        public object FormView
        {
            get => _formView;
            set
            {
                _formView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region ===== COMMAND =====
        public RelayCommand ThemCommand { get; }
        public RelayCommand SuaCommand { get; }
        public RelayCommand XoaCommand { get; }
        public RelayCommand XoaTimKiemCommand { get; }
        public RelayCommand XuatExcelCommand { get; }
        #endregion

        public KhachViewModel()
        {
            LoadDanhSach();

            ThemCommand = new RelayCommand(_ => MoFormThem());
            SuaCommand = new RelayCommand(MoFormSua);
            XoaCommand = new RelayCommand(XoaKhach);
            XoaTimKiemCommand = new RelayCommand(_ => SearchText = "");
            XuatExcelCommand = new RelayCommand(_ => XuatExcel());
        }

        #region ===== FORM =====
        private void MoFormThem()
        {
            FormView = new Views.ThemSuaKhachView
            {
                DataContext = new ThemSuaKhachViewModel(
                    isThemMoi: true,
                    khach: null,
                    onClose: DongForm)
            };

            _isFormVisible = true;
            RefreshVisibility();
        }

        private void MoFormSua(object obj)
        {
            if (obj == null) return;

            FormView = new Views.ThemSuaKhachView
            {
                DataContext = new ThemSuaKhachViewModel(
                    isThemMoi: false,
                    khach: obj,
                    onClose: DongForm)
            };

            _isFormVisible = true;
            RefreshVisibility();
        }

        private void DongForm()
        {
            _isFormVisible = false;
            FormView = null;
            RefreshVisibility();
            LoadDanhSach();
        }

        private void RefreshVisibility()
        {
            OnPropertyChanged(nameof(FormVisibility));
            OnPropertyChanged(nameof(ListVisibility));
        }
        #endregion

        #region ===== LOAD DATA =====
        private void LoadDanhSach()
        {
            DanhSachKhach.Clear();

            using var conn = DatabaseService.GetConnection();
            conn.Open();

            string sql = @"
                SELECT k.*,
                       ls.LoaiRaVao AS TrangThai,
                       ls.ThoiGian,
                       ls.PhuongTien,
                       ls.BienSo
                FROM Khach k
                LEFT JOIN (
                    SELECT *,
                           ROW_NUMBER() OVER (PARTITION BY CCCD ORDER BY ThoiGian DESC) rn
                    FROM LichSuRaVao
                    WHERE LoaiDoiTuong = N'Khách'
                ) ls ON k.CCCD = ls.CCCD AND ls.rn = 1
                WHERE (@Search IS NULL
                       OR k.CCCD LIKE @Search
                       OR k.HoTen LIKE @Search
                       OR k.DienThoai LIKE @Search
                       OR k.NguoiBaoLanh LIKE @Search)
                ORDER BY k.HoTen";

            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Search",
                string.IsNullOrWhiteSpace(SearchText) ? DBNull.Value : $"%{SearchText}%");

            var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                DanhSachKhach.Add(new
                {
                    CCCD = rd["CCCD"].ToString(),
                    HoTen = rd["HoTen"].ToString(),
                    GioiTinh = rd["GioiTinh"].ToString(),
                    NgaySinh = (DateTime)rd["NgaySinh"],
                    QueQuan = rd["QueQuan"].ToString(),
                    DienThoai = rd["DienThoai"].ToString(),
                    NguoiBaoLanh = rd["NguoiBaoLanh"].ToString(),
                    DonViNguoiBaoLanh = rd["DonViNguoiBaoLanh"].ToString(),
                    MucDichVao = rd["MucDichVao"].ToString(),
                    TrangThai = rd["TrangThai"]?.ToString(),
                    ThoiGian = rd["ThoiGian"] as DateTime?,
                    PhuongTien = rd["PhuongTien"]?.ToString(),
                    BienSo = rd["BienSo"]?.ToString()
                });
            }
        }
        #endregion

        #region ===== XÓA =====
        private void XoaKhach(object obj)
        {
            if (obj == null) return;

            dynamic k = obj;
            if (MessageBox.Show($"Xóa khách {k.HoTen}?",
                "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using var conn = DatabaseService.GetConnection();
            conn.Open();

            new SqlCommand("DELETE FROM Khach WHERE CCCD=@CCCD", conn)
            { Parameters = { new("@CCCD", k.CCCD) } }.ExecuteNonQuery();

            new SqlCommand("DELETE FROM LichSuRaVao WHERE CCCD=@CCCD", conn)
            { Parameters = { new("@CCCD", k.CCCD) } }.ExecuteNonQuery();

            LoadDanhSach();
        }
        #endregion

        #region ===== EXCEL =====
        private void XuatExcel()
        {
            SaveFileDialog sfd = new()
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = "Khach.xlsx"
            };

            if (sfd.ShowDialog() != true) return;

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Khách");

            ws.Cell(1, 1).Value = "CCCD";
            ws.Cell(1, 2).Value = "Họ tên";
            ws.Cell(1, 3).Value = "Giới tính";
            ws.Cell(1, 4).Value = "Ngày sinh";
            ws.Cell(1, 5).Value = "Quê quán";
            ws.Cell(1, 6).Value = "Điện thoại";
            ws.Cell(1, 7).Value = "Người bảo lãnh";
            ws.Cell(1, 8).Value = "Đơn vị";
            ws.Cell(1, 9).Value = "Mục đích vào";

            int r = 2;
            foreach (var k in DanhSachKhach)
            {
                ws.Cell(r, 1).Value = k.CCCD;
                ws.Cell(r, 2).Value = k.HoTen;
                ws.Cell(r, 3).Value = k.GioiTinh;
                ws.Cell(r, 4).Value = k.NgaySinh;
                ws.Cell(r, 5).Value = k.QueQuan;
                ws.Cell(r, 6).Value = k.DienThoai;
                ws.Cell(r, 7).Value = k.NguoiBaoLanh;
                ws.Cell(r, 8).Value = k.DonViNguoiBaoLanh;
                ws.Cell(r, 9).Value = k.MucDichVao;
                r++;
            }

            wb.SaveAs(sfd.FileName);
        }
        #endregion
    }
}