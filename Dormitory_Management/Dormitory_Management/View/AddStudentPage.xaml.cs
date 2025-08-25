using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Dormitory_Management.Models;

namespace Dormitory_Management.View
{
    /// <summary>
    /// Interaction logic for AddStudentPage.xaml
    /// </summary>
    public partial class AddStudentPage : Page
    {
        private readonly Dormitory_ManagementContext _context;

        public AddStudentPage()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();
            LoadAvailableRooms();
        }

        // Chỉ load các phòng còn trống và đang hoạt động (Booked = "No" & RoomStatus = "Yes")
        private void LoadAvailableRooms()
        {
            try
            {
                var availableRooms = _context.Rooms
                    .Where(r => r.Booked == "No" && r.RoomStatus == "Yes")
                    .Select(r => r.RoomNo)
                    .ToList();

                comboRoomNo.ItemsSource = availableRooms;
                comboRoomNo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rooms: " + ex.Message);
            }
        }

        // Nút Lưu
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = txtStudentName.Text.Trim();
                string father = txtFatherName.Text.Trim();
                string mother = txtMotherName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phone = txtMobile.Text.Trim();
                string cccd = txtIDProof.Text.Trim();
                string address = txtAddress.Text.Trim();
                var selectedRoom = comboRoomNo.SelectedItem;

                // Validate rỗng
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(father) ||
                    string.IsNullOrWhiteSpace(mother) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(cccd) ||
                    string.IsNullOrWhiteSpace(address) || selectedRoom == null)
                {
                    MessageBox.Show("Please fill in all information!");
                    return;
                }

                // Validate định dạng
                if (!Regex.IsMatch(phone, @"^\d{10}$"))
                {
                    MessageBox.Show("The phone number must consist of exactly 10 digits!");
                    return;
                }

                if (!Regex.IsMatch(cccd, @"^\d{12}$"))
                {
                    MessageBox.Show("The ID card must consist of exactly 12 digits!");
                    return;
                }

                if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    MessageBox.Show("The email is invalid!");
                    return;
                }

                // Kiểm tra trùng dữ liệu
                if (_context.Students.Any(s => s.Mobile == phone))
                {
                    MessageBox.Show("This phone number has already been used!");
                    return;
                }
                if (_context.Students.Any(s => s.Email == email))
                {
                    MessageBox.Show("This email has already been used!");
                    return;
                }
                if (_context.Students.Any(s => s.Idproof == cccd))
                {
                    MessageBox.Show("This ID Proof has already been used!");
                    return;
                }

                // Lấy thông tin phòng
                long roomNo = Convert.ToInt64(selectedRoom);
                var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);
                if (room == null)
                {
                    MessageBox.Show("Room does not exist.");
                    LoadAvailableRooms();
                    return;
                }

                // Không cho thêm nếu phòng đang bảo trì (inactive)
                if (room.RoomStatus == "No")
                {
                    MessageBox.Show($"Room {room.RoomNo} is under maintenance, cannot add student.");
                    return;
                }

                // Không cho thêm nếu phòng đã được đặt
                if (room.Booked == "Yes")
                {
                    MessageBox.Show($"Room {room.RoomNo} is already booked.");
                    LoadAvailableRooms();
                    return;
                }

                // Tạo sinh viên mới
                var student = new Student
                {
                    Mobile = phone,
                    Name = name,
                    Fname = father,
                    Mname = mother,
                    Email = email,
                    Paddress = address,
                    Idproof = cccd,
                    RoomNo = roomNo,
                    Living = "Yes"
                };

                // Cập nhật trạng thái phòng và lưu
                room.Booked = "Yes";
                _context.Students.Add(student);
                _context.SaveChanges();

                MessageBox.Show("Add student successfully!");
                ClearFields();
                LoadAvailableRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding student:\n" + ex.Message);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtMobile.Clear();
            txtStudentName.Clear();
            txtFatherName.Clear();
            txtMotherName.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtIDProof.Clear();
            comboRoomNo.SelectedIndex = -1;
        }
    }
}
