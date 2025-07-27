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
using System.Text.RegularExpressions;

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

        // Load các phòng chưa có sinh viên (Booked = "No")
        private void LoadAvailableRooms()
        {
            var availableRooms = _context.Rooms
                .Where(r => r.Booked == "No")  
                .Select(r => r.RoomNo)  
                .ToList();

            comboRoomNo.ItemsSource = availableRooms; 
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

                // Kiểm tra không để trống
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(father) || string.IsNullOrWhiteSpace(mother) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(cccd) ||
                    string.IsNullOrWhiteSpace(address) || selectedRoom == null)
                {
                    MessageBox.Show("Please fill in all information!");
                    return;
                }

                // Kiểm tra định dạng
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

                // Chuyển đổi số phòng
                long roomNo = Convert.ToInt64(selectedRoom);

                // Tạo sinh viên mới
                Student student = new Student
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

                // Cập nhật trạng thái phòng
                var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);
                if (room != null)
                {
                    room.Booked = "Yes";
                }

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
