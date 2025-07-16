using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dormitory_Management.Models;

namespace Dormitory_Management.View
{
    /// <summary>
    /// Interaction logic for ManageRoomsPage.xaml
    /// </summary>
    public partial class ManageRoomsPage : Page
    {
        private readonly Dormitory_ManagementContext _context;

        public ManageRoomsPage()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();
            LoadRooms();
        }

        // Hàm load dữ liệu phòng từ cơ sở dữ liệu vào DataGrid
        private void LoadRooms()
        {
            // Tải danh sách phòng từ cơ sở dữ liệu vào DataGrid
            RoomDataGrid.ItemsSource = _context.Rooms.ToList();
        }

        // Sự kiện khi nhấn nút Thêm Phòng
        // Sự kiện khi nhấn nút Thêm Phòng
        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            string roomStatus = ActivateRoomCheckBox.IsChecked == true ? "Yes" : "No";
            string bookedStatus = "No";  // Mặc định là chưa đặt phòng

            long roomNo;
            // Kiểm tra xem người dùng có nhập số phòng hợp lệ không
            if (string.IsNullOrWhiteSpace(RoomNumberTextBox.Text) || !long.TryParse(RoomNumberTextBox.Text, out roomNo))
            {
                MessageBox.Show("Số phòng không hợp lệ. Vui lòng nhập lại.");
                return;
            }

            // Kiểm tra nếu phòng đã tồn tại
            var existingRoom = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);
            if (existingRoom != null)
            {
                MessageBox.Show($"Phòng {roomNo} đã tồn tại trong hệ thống.");
                return;
            }

            // Tạo phòng mới
            Room newRoom = new Room
            {
                RoomNo = roomNo,
                RoomStatus = roomStatus,
                Booked = bookedStatus
            };

            _context.Rooms.Add(newRoom);
            _context.SaveChanges();

            LoadRooms();  // Tải lại danh sách phòng sau khi thêm
            MessageBox.Show($"Thêm phòng {roomNo} thành công.");
        }


        // Sự kiện khi nhấn nút Tìm Kiếm Phòng
        private void SearchRoom_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem người dùng đã nhập số phòng chưa
            if (string.IsNullOrEmpty(SearchRoomNumberTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập số phòng cần tìm.");
                return;
            }

            long roomNo = Convert.ToInt64(SearchRoomNumberTextBox.Text);
            var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);

            if (room != null)
            {
                // Hiển thị thông báo phòng đã tìm thấy
                MessageBox.Show($"Đã tìm thấy phòng {roomNo}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Cập nhật giao diện với thông tin phòng
                UpdateActivateRoomCheckBox.IsChecked = room.RoomStatus == "Yes";
                DeactivateRoomCheckBox.IsChecked = room.RoomStatus == "No";
            }
            else
            {
                // Hiển thị thông báo không tìm thấy phòng
                MessageBox.Show($"Phòng {roomNo} không có trong danh sách.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Sự kiện khi nhấn nút Cập nhật Phòng
        private void UpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            long roomNo = Convert.ToInt64(SearchRoomNumberTextBox.Text);
            var roomToUpdate = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);

            if (roomToUpdate != null)
            {
                roomToUpdate.RoomStatus = UpdateActivateRoomCheckBox.IsChecked == true ? "Yes" : "No";
                roomToUpdate.Booked = DeactivateRoomCheckBox.IsChecked == true ? "No" : roomToUpdate.Booked;
                _context.SaveChanges();

                LoadRooms();  // Tải lại danh sách phòng sau khi cập nhật
            }
            else
            {
                MessageBox.Show("Phòng không tồn tại.");
            }
        }

        
        // Sự kiện khi nhấn nút Xóa Phòng
        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu người dùng đã nhập số phòng
            if (string.IsNullOrEmpty(SearchRoomNumberTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập số phòng cần xóa.");
                return;
            }

            long roomNo = Convert.ToInt64(SearchRoomNumberTextBox.Text);
            var roomToDelete = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);

            if (roomToDelete != null)
            {
                // Hỏi người dùng có chắc chắn muốn xóa phòng này
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phòng {roomNo}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Kiểm tra nếu phòng có sinh viên hay không, không thể xóa nếu có sinh viên
                    var studentInRoom = _context.Students.FirstOrDefault(s => s.RoomNo == roomNo);
                    if (studentInRoom != null)
                    {
                        MessageBox.Show("Không thể xóa phòng này vì có sinh viên đang ở.");
                        return;
                    }

                    // Xóa phòng nếu không có sinh viên
                    _context.Rooms.Remove(roomToDelete);
                    _context.SaveChanges();

                    // Tải lại danh sách phòng sau khi xóa
                    LoadRooms();
                    MessageBox.Show("Xóa phòng thành công.");
                }
            }
            else
            {
                MessageBox.Show("Phòng không tồn tại.");
            }
        }


    }
}
