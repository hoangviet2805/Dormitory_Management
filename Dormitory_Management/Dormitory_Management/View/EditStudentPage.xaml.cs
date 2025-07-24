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

        public EditStudentPage()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();
        }

        // Search for student by phone number
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
                // Fill in student details
                txtStudentName.Text = _currentStudent.Name;
                txtFatherName.Text = _currentStudent.Fname;
                txtMotherName.Text = _currentStudent.Mname;
                txtEmail.Text = _currentStudent.Email;
                txtAddress.Text = _currentStudent.Paddress;
                txtIDProof.Text = _currentStudent.Idproof;

                // Display current room
                if (_currentStudent.RoomNo != 0)
                {
                    txtCurrentRoom.Text = _currentStudent.RoomNo.ToString();
                }
                else
                {
                    txtCurrentRoom.Text = "Not Assigned";
                }

                // Update ComboBox with available rooms
                var availableRooms = _context.Rooms.Where(r => r.Booked == "No").Select(r => r.RoomNo).ToList();
                comboRoomNo.ItemsSource = availableRooms;
            }
            else
            {
                MessageBox.Show("Student not found with this phone number.");
            }
        }

        // Save updated student information
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = txtStudentName.Text.Trim();
            string father = txtFatherName.Text.Trim();
            string mother = txtMotherName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtMobile.Text.Trim();
            string cccd = txtIDProof.Text.Trim();
            string address = txtAddress.Text.Trim();
            var selectedRoom = comboRoomNo.SelectedItem;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(father) || string.IsNullOrWhiteSpace(mother) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(cccd) ||
                string.IsNullOrWhiteSpace(address) || selectedRoom == null)
            {
                MessageBox.Show("Please fill in all fields.");
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

            long newRoomNo = (long)selectedRoom;

            var result = MessageBox.Show($"Are you sure you want to change room from {student.RoomNo} to {newRoomNo}?", "Confirm Room Change", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Update old room status
                var oldRoom = _context.Rooms.FirstOrDefault(r => r.RoomNo == student.RoomNo);
                if (oldRoom != null)
                {
                    oldRoom.Booked = "No";
                }

                // Update new room status
                var newRoom = _context.Rooms.FirstOrDefault(r => r.RoomNo == newRoomNo);
                if (newRoom != null)
                {
                    newRoom.Booked = "Yes"; 
                }

                // Update student details
                student.Name = name;
                student.Fname = father;
                student.Mname = mother;
                student.Email = email;
                student.Paddress = address;
                student.Idproof = cccd;
                student.RoomNo = newRoomNo;

                _context.SaveChanges();
                MessageBox.Show("Student information and room updated successfully!");
            }
        }

        // Checkout the student (mark as not living and free the room)
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
                // Update living status to "No"
                _currentStudent.Living = "No";

                // Update room status to available
                var roomNo = _currentStudent.RoomNo;
                var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);
                if (room != null)
                {
                    room.Booked = "No";
                }

                // Save the changes
                _context.SaveChanges();

                MessageBox.Show("Student has been checked out and the room is now available.");
                ClearFields();
            }
        }

        // Re-check in the student
        private void btnRecheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStudent == null)
            {
                MessageBox.Show("No student found to re-check in.");
                return;
            }

            // Ask user for confirmation to re-check in the student
            var result = MessageBox.Show($"Are you sure you want to re-check in student {_currentStudent.Name}?", "Confirm Re-check In", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // Set the student's living status to "Yes" (indicating they are living again)
                _currentStudent.Living = "Yes";

                // If the student has a room number assigned, mark the room as booked
                if (_currentStudent.RoomNo != 0)
                {
                    var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == _currentStudent.RoomNo);
                    if (room != null)
                    {
                        room.Booked = "Yes"; // Mark the room as booked again
                    }
                }

                // Save the changes
                _context.SaveChanges();

                MessageBox.Show("The student has been re-checked in successfully.");
            }
        }


        // Delete student and update room status (same as checkout)
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStudent == null)
            {
                MessageBox.Show("No student found.");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Update living status to "No"
                _currentStudent.Living = "No";

                // Update room status
                var roomNo = _currentStudent.RoomNo;
                var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);
                if (room != null)
                {
                    room.Booked = "No"; 
                }

                _context.SaveChanges();

                // Remove student
                _context.Students.Remove(_currentStudent);
                _context.SaveChanges();

                MessageBox.Show("Student deleted and room status updated.");
                ClearFields();
            }
        }

        // Clear fields on the form
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        // Clear fields
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
            txtCurrentRoom.Text = string.Empty;
        }

    }

}
