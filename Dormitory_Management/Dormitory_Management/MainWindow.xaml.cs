using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dormitory_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Sự kiện khi nhấn nút Quản Lý Phòng
        private void ManageRoomsClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Quản lý phòng clicked.");
            // Logic điều hướng hoặc mở cửa sổ mới ở đây
        }

        // Sự kiện khi nhấn nút Thêm Sinh Viên
        private void AddStudentClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thêm sinh viên clicked.");
            // Logic điều hướng hoặc mở cửa sổ mới ở đây
        }

        // Sự kiện khi nhấn nút Cập Nhật - Xóa Sinh Viên
        private void UpdateDeleteStudentClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cập nhật - Xóa sinh viên clicked.");
            // Logic điều hướng hoặc mở cửa sổ mới ở đây
        }

        // Sự kiện khi nhấn nút Thanh Toán Phí Phòng
        private void ManageFeesClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thanh toán phí phòng clicked.");
            // Logic điều hướng hoặc mở cửa sổ mới ở đây
        }

        // Sự kiện khi nhấn nút Tất Cả Trạng Thái Sinh Viên
        private void ManageStudentStatusClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tất cả trạng thái sinh viên clicked.");
            // Logic điều hướng hoặc mở cửa sổ mới ở đây
        }

        // Sự kiện khi nhấn nút Sinh Viên Trả Phòng
        private void StudentCheckoutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sinh viên trả phòng clicked.");
            // Logic điều hướng hoặc mở cửa sổ mới ở đây
        }

        // Sự kiện khi nhấn nút Đăng xuất
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đăng xuất clicked.");
            // Logic đăng xuất hoặc điều hướng về trang đăng nhập
            this.Close(); // Đóng cửa sổ hiện tại (hoặc mở cửa sổ đăng nhập)
        }
    }
}