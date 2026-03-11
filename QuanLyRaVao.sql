-- 1. Tạo cơ sở dữ liệu
CREATE DATABASE QuanLyRaVao;
GO
USE QuanLyRaVao;
GO

-- 2. Bảng Quân nhân
CREATE TABLE QuanNhan (
    CCCD VARCHAR(12) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    QueQuan NVARCHAR(100),
    CapBac NVARCHAR(50),
    ChucVu NVARCHAR(50),
    DonVi NVARCHAR(100)
);
GO

INSERT INTO QuanNhan
(CCCD, HoTen, GioiTinh, NgaySinh, QueQuan, CapBac, ChucVu, DonVi)
VALUES
('012345678901', N'Nguyễn Văn A', N'Nam', '1990-01-01', N'Hà Nội', N'Trung úy', N'Chỉ huy', N'Đơn vị 1'),
('012345678902', N'Trần Thị B', N'Nữ', '1992-02-15', N'Hải Phòng', N'Thiếu úy', N'Pháo binh', N'Đơn vị 2'),
('012345678903', N'Phạm Văn C', N'Nam', '1988-05-20', N'Nam Định', N'Đại úy', N'Tác chiến', N'Đơn vị 1'),
('012345678904', N'Lê Thị D', N'Nữ', '1995-03-10', N'Bắc Ninh', N'Thượng sĩ', N'Thủ kho', N'Đơn vị 3'),
('012345678905', N'Vũ Văn E', N'Nam', '1991-07-25', N'Hà Nam', N'Trung úy', N'Lính chiến', N'Đơn vị 2');
GO

-- 3. Bảng Khách
CREATE TABLE Khach (
    CCCD VARCHAR(12) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    QueQuan NVARCHAR(100),
    DienThoai VARCHAR(20),
    NguoiBaoLanh NVARCHAR(100),
    DonViNguoiBaoLanh NVARCHAR(100),
    MucDichVao NVARCHAR(100)
);
GO

INSERT INTO Khach
(CCCD, HoTen, GioiTinh, NgaySinh, QueQuan, DienThoai, NguoiBaoLanh, DonViNguoiBaoLanh, MucDichVao)
VALUES
('900000000001', N'Nguyễn Văn F', N'Nam', '1985-06-12', N'Hà Nội', '0912345678', N'Đại úy Nguyễn Văn A', N'Đơn vị 1', N'Thăm người thân'),
('900000000002', N'Trần Thị G', N'Nữ', '1990-08-20', N'Hải Phòng', '0987654321', N'Thượng sĩ Lê Thị D', N'Đơn vị 3', N'Công tác'),
('900000000003', N'Phạm Văn H', N'Nam', '1987-12-05', N'Nam Định', '0911222333', N'Đại úy Phạm Văn C', N'Đơn vị 1', N'Họp'),
('900000000004', N'Lê Thị I', N'Nữ', '1993-11-11', N'Bắc Ninh', '0922333444', N'Trung úy Vũ Văn E', N'Đơn vị 2', N'Giao hàng'),
('900000000005', N'Vũ Văn J', N'Nam', '1989-04-18', N'Hà Nam', '0933444555', N'Thiếu úy Trần Thị B', N'Đơn vị 2', N'Sửa chữa');
GO

-- 4. Bảng Lịch sử ra/vào
CREATE TABLE LichSuRaVao (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CCCD VARCHAR(12) NOT NULL,
    LoaiDoiTuong NVARCHAR(20) NOT NULL, -- 'Quân Nhân' hoặc 'Khách'
    LoaiRaVao NVARCHAR(10) NOT NULL, -- 'Ra' hoặc 'Vào'
    ThoiGian DATETIME NOT NULL DEFAULT GETDATE(),
    PhuongTien NVARCHAR(50),
    BienSo NVARCHAR(20),
    NguoiTrucGac NVARCHAR(100) NOT NULL,
    DonViNguoiTruc NVARCHAR(100) NOT NULL
);
GO

SET IDENTITY_INSERT LichSuRaVao ON;

INSERT INTO LichSuRaVao
(ID, CCCD, LoaiDoiTuong, LoaiRaVao, ThoiGian, PhuongTien, BienSo, NguoiTrucGac, DonViNguoiTruc)
VALUES
(1, '012345678901', N'Quân Nhân', N'Vào', '2026-01-07 08:00:00', N'Xe máy', '29A-12345', N'Người trực A', N'Đơn vị 1'),
(2, '012345678902', N'Quân Nhân', N'Vào', '2026-01-07 08:15:00', N'Đi bộ', NULL, N'Người trực B', N'Đơn vị 2'),
(3, '012345678903', N'Quân Nhân', N'Ra',  '2026-01-07 17:00:00', N'Ô tô', '30B-54321', N'Người trực C', N'Đơn vị 1'),
(4, '012345678904', N'Quân Nhân', N'Vào', '2026-01-07 09:30:00', N'Xe máy', '35C-67890', N'Người trực A', N'Đơn vị 3'),
(5, '012345678905', N'Quân Nhân', N'Ra',  '2026-01-07 18:00:00', N'Đi bộ', NULL, N'Người trực B', N'Đơn vị 2'),
(6, '900000000001', N'Khách', N'Vào', '2026-01-07 08:10:00', N'Xe máy', '29A-11111', N'Người trực A', N'Đơn vị 1'),
(7, '900000000002', N'Khách', N'Vào', '2026-01-07 08:20:00', N'Đi bộ', NULL, N'Người trực B', N'Đơn vị 3'),
(8, '900000000003', N'Khách', N'Ra',  '2026-01-07 12:00:00', N'Ô tô', '30B-22222', N'Người trực C', N'Đơn vị 1'),
(9, '900000000004', N'Khách', N'Vào', '2026-01-07 09:45:00', N'Xe máy', '35C-33333', N'Người trực A', N'Đơn vị 2'),
(10,'900000000005', N'Khách', N'Ra',  '2026-01-07 17:30:00', N'Đi bộ', NULL, N'Người trực B', N'Đơn vị 2'),
(11,'012345678904', N'Quân Nhân', N'Vào', '2026-01-07 18:14:08', N'Đi bộ', NULL, N'Người trực A', N'Đơn vị 1');

SET IDENTITY_INSERT LichSuRaVao OFF;
GO
