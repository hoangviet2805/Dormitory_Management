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
    public partial class Login : Window 
    {
        private readonly Dormitory_ManagementContext _context;

        public Login()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();
        }

       
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorText.Text = "Please enter your username and password.";
                return;
            }

           
            string hashedPassword = HashPassword(password);

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (user != null)
            {
                MessageBox.Show("Login successful!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow main = new MainWindow();  
                main.Show();
                this.Close(); 
            }
            else
            {
                ErrorText.Text = "Incorrect username or password.";
            }
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Login loginWindow = new Login();
            loginWindow.Show(); 
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgotPasswordWindow = new ForgotPassword();
            forgotPasswordWindow.ShowDialog();
        }
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
