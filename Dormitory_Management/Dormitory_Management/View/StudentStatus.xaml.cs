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
    /// Interaction logic for StudentStatus.xaml
    /// </summary>
    public partial class StudentStatus : Page
    {
        private readonly Dormitory_ManagementContext _context;

        public StudentStatus()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();
            LoadStudentData();
        }

        private void LoadStudentData(string phoneNumber = null)
        {
            var studentDataQuery = _context.Students.AsQueryable();
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                studentDataQuery = studentDataQuery.Where(s => s.Mobile.Contains(phoneNumber));
            }

            var studentData = studentDataQuery
                .Select(s => new
                {
                    s.Id,          
                    s.Mobile,      
                    s.Name,        
                    s.Fname,       
                    s.Mname,       
                    s.Email,       
                    s.Paddress,    
                    s.Idproof,     
                    s.RoomNo,      
                    s.Living      
                })
                .ToList();

            StudentDataGrid.ItemsSource = studentData;
        }

        private void SearchByPhone_Click(object sender, RoutedEventArgs e)
        {
            string phoneNumber = PhoneSearchTextBox.Text.Trim();
            LoadStudentData(phoneNumber); 
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadStudentData(); // Gọi lại hàm lấy dữ liệu
        }
    }
}
