# PHẦN MỀM QUẢN LÝ RA VÀO CỔNG DOANH TRẠI

## 1.Giới thiệu

**Phần mềm quản lý ra vào cổng doanh trại** là ứng dụng desktop được phát triển nhằm hỗ trợ sĩ quan trực ban hoặc cán bộ quản lý trong việc **ghi nhận, theo dõi và quản lý việc ra vào của quân nhân và phương tiện tại cổng đơn vị**.

Ứng dụng giúp việc quản lý trở nên **nhanh chóng, chính xác và dễ tra cứu**, thay thế một phần các phương pháp ghi chép thủ công truyền thống.

Phần mềm được xây dựng bằng **WPF (.NET) kết hợp thư viện WPF-UI** để tạo giao diện hiện đại và trực quan, đồng thời sử dụng **SQL Server Express** để lưu trữ và quản lý dữ liệu.

## 2.Tính năng chính

* **Quản lý thông tin quân nhân**
* **Ghi nhận lượt ra vào cổng**
* **Lưu trữ lịch sử ra vào**
* **Tìm kiếm và tra cứu dữ liệu nhanh**
* **Hiển thị danh sách ra vào theo thời gian**
* **Giao diện hiện đại với WPF-UI**
* **Lưu trữ dữ liệu bằng SQL Server Express**
* 
## 3.Công nghệ sử dụng

* **C#**
* **WPF (.NET)**
* **WPF-UI** – thư viện giao diện hiện đại cho WPF
* **SQL Server Express** – hệ quản trị cơ sở dữ liệu
* **XAML** – thiết kế giao diện

  
## 4.Kiến trúc ứng dụng

Ứng dụng được phát triển theo mô hình tách biệt giữa:

* **UI (XAML + WPF-UI)**
* **Business Logic**
* **Database (SQL Server Express)**

Giúp hệ thống dễ dàng bảo trì và mở rộng trong tương lai.


## 5.Cài đặt và chạy project

### 5.1 Clone repository

git clone https://github.com/hai-vu-6903/quanLyRaVao-app.git

### 5.2️ Mở project bằng Visual Studio

Mở file: ProjectName.sln

### 5.3️ Cấu hình cơ sở dữ liệu

1. Cài đặt **SQL Server Express**
2. Tạo database mới
3. Import file database (nếu có) hoặc chạy script SQL
4. Cập nhật **connection string** trong file cấu hình:

### 5.4️ Chạy ứng dụng

Trong **Visual Studio**: Ctrl + F5

## 6.Mục đích dự án

Dự án được thực hiện nhằm:

* Hỗ trợ **quản lý ra vào tại cổng doanh trại**
* Giảm thiểu sai sót trong việc ghi chép thủ công
* Luyện tập và áp dụng kỹ năng phát triển ứng dụng **WPF**

---

## Screenshot

<p align="center">
  <img src="https://github.com/user-attachments/assets/1ae4a74e-0f02-4061-82db-be5592b9c84c" width="900"/>
</p>

<p align="center">
<img width="1918" height="1018" alt="image" src="https://github.com/user-attachments/assets/57c1efe0-32cf-487f-bd1c-e8f4ee8bc896" />
</p>

<p align="center">
<img width="1918" height="1017" alt="image" src="https://github.com/user-attachments/assets/0311cca2-2533-4695-9ec0-139fa17cd92a" />
</p>

<p align="center">
<img width="1918" height="1017" alt="image" src="https://github.com/user-attachments/assets/22179dc2-b6ad-49f6-8302-a7ce9aa7a345" />
</p>

## Download

Tải phần mềm tại đây:

[⬇️ Download File EXE](https://github.com/hai-vu-6903/quanLyRaVao-app/releases/tag/v1.0)
