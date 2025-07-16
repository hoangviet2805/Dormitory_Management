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
using System.Windows.Shapes;
using Dormitory_Management.Models;
using System.Security.Cryptography;

namespace Dormitory_Management.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window  // Cập nhật tên lớp là Login
    {
        private readonly Dormitory_ManagementContext _context;

        public Login()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();  // Khởi tạo DbContext để kết nối cơ sở dữ liệu
        }

        // Sự kiện khi nhấn nút Đăng Nhập
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorText.Text = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                return;
            }

            // Mã hóa mật khẩu trước khi so sánh
            string hashedPassword = HashPassword(password);

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (user != null)
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow main = new MainWindow();  // Truyền thông tin người dùng vào MainWindow
                main.Show();
                this.Close();  // Đóng cửa sổ đăng nhập
            }
            else
            {
                ErrorText.Text = "Sai tên đăng nhập hoặc mật khẩu.";
            }
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Đóng cửa sổ MainWindow hiện tại
            this.Close();

            // Mở cửa sổ Login
            Login loginWindow = new Login();
            loginWindow.Show();  // Hiển thị cửa sổ Login
        }

        // Sự kiện khi nhấn nút Quên mật khẩu
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgotPasswordWindow = new ForgotPassword(); // Mở cửa sổ quên mật khẩu
            forgotPasswordWindow.ShowDialog();
        }

        // Hàm mã hóa SHA256 (Sử dụng SHA-256 để mã hóa mật khẩu)
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password); // Chuyển mật khẩu thành mảng byte
                byte[] hash = sha.ComputeHash(bytes);             // Mã hóa mật khẩu thành hash
                return BitConverter.ToString(hash).Replace("-", "").ToLower(); // Chuyển mảng byte thành chuỗi hex
            }
        }
    }
}
