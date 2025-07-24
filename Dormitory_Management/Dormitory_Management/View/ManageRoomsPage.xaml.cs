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

        
        private void LoadRooms()
        {
            RoomDataGrid.ItemsSource = _context.Rooms.ToList();
        }

        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            string roomStatus = ActivateRoomCheckBox.IsChecked == true ? "Yes" : "No";
            string bookedStatus = "No";

            long roomNo;
            if (string.IsNullOrWhiteSpace(RoomNumberTextBox.Text) || !long.TryParse(RoomNumberTextBox.Text, out roomNo))
            {
                MessageBox.Show("The room number is invalid. Please enter it again.");
                return;
            }

            var existingRoom = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);
            if (existingRoom != null)
            {
                MessageBox.Show($"Room {roomNo} already exists in the system.");
                return;
            }

            Room newRoom = new Room
            {
                RoomNo = roomNo,
                RoomStatus = roomStatus,
                Booked = bookedStatus
            };

            _context.Rooms.Add(newRoom);
            _context.SaveChanges();

            LoadRooms();
            MessageBox.Show($"Room {roomNo} added successfully.");
        }


        private void SearchRoom_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchRoomNumberTextBox.Text))
            {
                MessageBox.Show("Please enter the room number to search.");
                return;
            }

            long roomNo = Convert.ToInt64(SearchRoomNumberTextBox.Text);
            var room = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);

            if (room != null)
            {
                MessageBox.Show($"Room {roomNo} found.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateActivateRoomCheckBox.IsChecked = room.RoomStatus == "Yes";
                DeactivateRoomCheckBox.IsChecked = room.RoomStatus == "No";
            }
            else
            {
                MessageBox.Show($"Room {roomNo} is not in the list.", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            long roomNo = Convert.ToInt64(SearchRoomNumberTextBox.Text);
            var studentInRoom = _context.Students.FirstOrDefault(s => s.RoomNo == roomNo);
            if (studentInRoom != null)
            {
                MessageBox.Show($"Cannot update room {roomNo} because there are students currently staying in this room.", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var roomToUpdate = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);
            if (roomToUpdate != null)
            {
                roomToUpdate.RoomStatus = UpdateActivateRoomCheckBox.IsChecked == true ? "Yes" : "No";
                roomToUpdate.Booked = DeactivateRoomCheckBox.IsChecked == true ? "No" : roomToUpdate.Booked;
                _context.SaveChanges();

                LoadRooms();
                MessageBox.Show($"Room {roomNo} updated successfully.");
            }
            else
            {
                MessageBox.Show("The room does not exist.");
            }
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            string roomNoText = SearchRoomNumberTextBox.Text.Trim();

            // Check if the input is empty
            if (string.IsNullOrEmpty(roomNoText))
            {
                MessageBox.Show("Please enter a valid room number.");
                return;
            }

            // Try to parse the room number to ensure it's a valid number
            if (!long.TryParse(roomNoText, out long roomNo))
            {
                MessageBox.Show("Invalid room number format.");
                return;
            }

            // Check if the room has any students assigned
            var studentInRoom = _context.Students.FirstOrDefault(s => s.RoomNo == roomNo);
            if (studentInRoom != null)
            {
                MessageBox.Show($"Cannot delete the room {roomNo} because there are students currently staying in this room!", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ask for confirmation before deleting
            var result = MessageBox.Show($"Are you sure you want to delete room {roomNo}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var roomToDelete = _context.Rooms.FirstOrDefault(r => r.RoomNo == roomNo);
                if (roomToDelete != null)
                {
                    _context.Rooms.Remove(roomToDelete);
                    _context.SaveChanges();
                    LoadRooms();
                    MessageBox.Show($"Room {roomNo} deleted successfully.");
                }
                else
                {
                    MessageBox.Show("The room does not exist.");
                }
            }
            else
            {
                MessageBox.Show("Room deletion has been canceled.");
            }
        }





    }
}
