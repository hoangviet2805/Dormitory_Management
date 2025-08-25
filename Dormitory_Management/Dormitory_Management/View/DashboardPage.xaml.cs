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
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            using (var context = new Dormitory_ManagementContext())
            {
                // Tổng số phòng
                int totalRooms = context.Rooms.Count();

                // Phòng trống (Booked = "No")
                int availableRooms = context.Rooms.Count(r => r.Booked == "No");

                // Phòng đã ở (Booked != "No")
                int occupiedRooms = context.Rooms.Count(r => r.Booked != "No");

                // Tổng số sinh viên
                int totalStudents = context.Students.Count();

                // Gán vào UI
                TotalRoomsText.Text = totalRooms.ToString();
                AvailableRoomsText.Text = availableRooms.ToString();
                OccupiedRoomsText.Text = occupiedRooms.ToString();
                TotalStudentsText.Text = totalStudents.ToString();
            }
        }
    }
}
