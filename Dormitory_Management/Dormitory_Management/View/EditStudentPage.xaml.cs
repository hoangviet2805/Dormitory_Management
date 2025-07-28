using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for EditStudentPage.xaml
    /// </summary>
    public partial class EditStudentPage : Page
    {
        private readonly Dormitory_ManagementContext _context;
        private Student _currentStudent;
        private bool isRecheckInMode = false;


        public EditStudentPage()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();
        }

        // Search
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string phone = txtMobile.Text.Trim();

            if (string.IsNullOrWhiteSpace(phone) || !phone.All(Char.IsDigit))
            {
                MessageBox.Show("Invalid phone number.");
                return;
            }

            _currentStudent = _context.Students.FirstOrDefault(s => s.Mobile == phone);

            if (_currentStudent != null)
            {
                txtStudentName.Text = _currentStudent.Name;
                txtFatherName.Text = _currentStudent.Fname;
                txtMotherName.Text = _currentStudent.Mname;
                txtEmail.Text = _currentStudent.Email;
                txtAddress.Text = _currentStudent.Paddress;
                txtIDProof.Text = _currentStudent.Idproof;

                // Hiển thị phòng hiện tại
                if (_currentStudent.RoomNo != 0 && _currentStudent.RoomNo != null)
                {
                    var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == _currentStudent.RoomNo);
                    txtCurrentRoom.Text = room != null
                        ? $"{room.RoomNo} (Booked: {room.Booked})"
                        : _currentStudent.RoomNo.ToString();
                }
                else
                {
                    txtCurrentRoom.Text = "Not Assigned";
                }

                // Xử lý hiển thị Change Room
                if (_currentStudent.Living == "No" && !isRecheckInMode)
                {
                    panelChangeRoom.Visibility = Visibility.Collapsed;
                }
                else
                {
                    panelChangeRoom.Visibility = Visibility.Visible;
                    comboRoomNo.ItemsSource = _context.Rooms
                        .Where(r => r.Booked == "No")
                        .Select(r => r.RoomNo)
                        .ToList();
                    comboRoomNo.IsEnabled = true;
                }

                SetActionButtonsEnabled(true); // bật lại các nút chức năng
            }
            else
            {
                MessageBox.Show("Student not found with this phone number.");
                ClearFields();
            }
        }






        // Save update
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = txtStudentName.Text.Trim();
            string father = txtFatherName.Text.Trim();
            string mother = txtMotherName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtMobile.Text.Trim();
            string cccd = txtIDProof.Text.Trim();
            string address = txtAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(father) ||
                string.IsNullOrWhiteSpace(mother) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(cccd) ||
                string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Please fill in all personal fields.");
                return;
            }

            if (!Regex.IsMatch(phone, @"^\d{10}$"))
            {
                MessageBox.Show("Phone number must be exactly 10 digits.");
                return;
            }

            if (!Regex.IsMatch(cccd, @"^\d{12}$"))
            {
                MessageBox.Show("ID Proof must be exactly 12 digits.");
                return;
            }

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            var student = _context.Students.FirstOrDefault(s => s.Mobile == phone);
            if (student == null)
            {
                MessageBox.Show("Student not found.");
                return;
            }

            // Nếu có chọn phòng mới thì xử lý đổi phòng
            if (comboRoomNo.SelectedItem != null)
            {
                long newRoomNo = (long)comboRoomNo.SelectedItem;
                long? oldRoomNo = student.RoomNo;

                if (oldRoomNo != newRoomNo)
                {
                    var confirm = MessageBox.Show($"Are you sure you want to change room from {oldRoomNo?.ToString() ?? "None"} to {newRoomNo}?", "Confirm Room Change", MessageBoxButton.YesNo);
                    if (confirm != MessageBoxResult.Yes)
                        return;

                    UpdateRoomStatus(oldRoomNo, newRoomNo);
                    student.RoomNo = newRoomNo;
                }
            }

            // Cập nhật thông tin cá nhân
            student.Name = name;
            student.Fname = father;
            student.Mname = mother;
            student.Email = email;
            student.Paddress = address;
            student.Idproof = cccd;

            _context.SaveChanges();
            MessageBox.Show("Student information updated successfully.");
        }


        // Checkout
        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStudent == null)
            {
                MessageBox.Show("No student found.");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to check out this student?", "Confirm Checkout", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _currentStudent.Living = "No";

                if (_currentStudent.RoomNo.HasValue)
                {
                    var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == _currentStudent.RoomNo.Value);
                    if (room != null)
                    {
                        room.Booked = "No";
                    }

                    _currentStudent.RoomNo = null;
                }

                _context.SaveChanges();
                MessageBox.Show("Student has been checked out and the room is now available.");
                ClearFields();
            }
        }

        // Re-check in
        private void btnRecheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStudent == null)
            {
                MessageBox.Show("No student found to re-check in.");
                return;
            }

            // Nếu vẫn đang ở thì không được re-check in
            if (_currentStudent.Living == "Yes")
            {
                MessageBox.Show("This student has not checked out yet. Please checkout before re-checking in.");
                return;
            }

            // Nhấn lần đầu: vào chế độ Re-check In
            if (!isRecheckInMode)
            {
                isRecheckInMode = true;
                SetActionButtonsEnabled(false);

                // Hiện panel change room
                panelChangeRoom.Visibility = Visibility.Visible;

                // Load phòng trống
                comboRoomNo.ItemsSource = _context.Rooms
                    .Where(r => r.Booked == "No")
                    .Select(r => r.RoomNo)
                    .ToList();

                comboRoomNo.IsEnabled = true;
                comboRoomNo.SelectedIndex = -1;

                MessageBox.Show("Please select a room and click Re-check In again to confirm.");
                return;
            }

            // Nhấn lần 2: xác nhận Re-check In
            if (comboRoomNo.SelectedItem == null)
            {
                MessageBox.Show("Please select a room before confirming.");
                return;
            }

            long selectedRoomNo = (long)comboRoomNo.SelectedItem;

            var confirm = MessageBox.Show($"Are you sure you want to re-check in student {_currentStudent.Name} to room {selectedRoomNo}?", "Confirm Re-check In", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes)
                return;

            var newRoom = _context.Rooms.FirstOrDefault(r => r.RoomNo == selectedRoomNo);
            if (newRoom == null || newRoom.Booked == "Yes")
            {
                MessageBox.Show("Selected room is not available.");
                return;
            }

            // Cập nhật
            UpdateRoomStatus(_currentStudent.RoomNo, selectedRoomNo);
            _currentStudent.Living = "Yes";
            _currentStudent.RoomNo = selectedRoomNo;
            _context.SaveChanges();

            MessageBox.Show("The student has been re-checked in successfully.");

            isRecheckInMode = false;
            SetActionButtonsEnabled(true);
            comboRoomNo.IsEnabled = false;

            btnSearch_Click(null, null); // reload lại thông tin
        }







        private void UpdateRoomStatus(long? oldRoomNo, long newRoomNo)
        {
            if (oldRoomNo.HasValue && oldRoomNo != newRoomNo)
            {
                var oldRoom = _context.Rooms.FirstOrDefault(r => r.RoomNo == oldRoomNo.Value);
                if (oldRoom != null)
                {
                    oldRoom.Booked = "No";
                }
            }

            var newRoom = _context.Rooms.FirstOrDefault(r => r.RoomNo == newRoomNo);
            if (newRoom != null)
            {
                newRoom.Booked = "Yes";
            }
        }


        // Delete
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStudent == null)
            {
                MessageBox.Show("No student found to delete.");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _currentStudent.Living = "No";

                var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == _currentStudent.RoomNo);
                if (room != null)
                {
                    room.Booked = "No";
                }

                var studentFees = _context.Fees.Where(f => f.MobileNo == _currentStudent.Mobile).ToList();
                foreach (var fee in studentFees)
                {
                    _context.Fees.Remove(fee);
                }

                _context.Students.Remove(_currentStudent);
                _context.SaveChanges();

                MessageBox.Show("Student has been deleted and room status updated.");
                ClearFields();
            }
        }

        private void SetActionButtonsEnabled(bool isEnabled)
        {
            btnSave.IsEnabled = isEnabled;
            btnDelete.IsEnabled = isEnabled;
            btnClear.IsEnabled = isEnabled;
            btnCheckout.IsEnabled = isEnabled;
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
            comboRoomNo.ItemsSource = null;
            txtCurrentRoom.Text = string.Empty;
            _currentStudent = null;
            isRecheckInMode = false;
            SetActionButtonsEnabled(true);
        }

    }

}

