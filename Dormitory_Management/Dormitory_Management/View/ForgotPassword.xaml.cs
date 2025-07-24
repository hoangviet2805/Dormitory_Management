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
using System.Net.Mail;
using System.Net;
namespace Dormitory_Management.View
{
    /// <summary>
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : Window
    {
        private readonly Dormitory_ManagementContext _context;
        private string generatedOtp;
        private User currentUser;

        public ForgotPassword()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();
        }

        // Sự kiện khi nhấn nút Gửi mã OTP
        private void SendOtpButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                ErrorText.Text = "Please enter your registered email.";
                return;
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                // Tạo mã OTP ngẫu nhiên
                generatedOtp = GenerateOtp();
                // Gửi OTP qua email
                SendOtpToEmail(user.Email, generatedOtp);
                currentUser = user;

                ResetSection.Visibility = Visibility.Visible; 
                ErrorText.Text = "";
            }
            else
            {
                ErrorText.Text = "No account found with this email.";
            }
        }

        // Tạo mã OTP ngẫu nhiên
        private string GenerateOtp()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }

        // Gửi OTP qua email
        private void SendOtpToEmail(string toEmail, string otpCode)
        {
            string fromEmail = "dormitorymanagement123@gmail.com";
            string appPassword = "jjao zwow pfeq qhua";

            MailMessage message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "OTP code to recover password",
                Body = $"Your OTP code is: {otpCode}",
                IsBodyHtml = false
            };

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, appPassword)
            };

            smtp.Send(message);
        }

        // Đổi mật khẩu
        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredOtp = OtpTextBox.Text.Trim();
            string newPassword = NewPasswordBox.Password.Trim();
            string confirmPassword = ConfirmPasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(enteredOtp) || currentUser == null)
            {
                ErrorText.Text = "Please resend OTP code.";
                return;
            }

            if (enteredOtp != generatedOtp)
            {
                ErrorText.Text = "OTP code is incorrect.";
                return;
            }

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ErrorText.Text = "Please enter your new password in full.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                ErrorText.Text = "Confirmation password does not match.";
                return;
            }

            // Mã hóa mật khẩu mới trước khi lưu vào cơ sở dữ liệu
            string hashedPassword = HashPassword(newPassword);

            using (var context = new Dormitory_ManagementContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == currentUser.UserId);
                if (user != null)
                {
                    user.Password = hashedPassword;
                    context.SaveChanges();

                    MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    ErrorText.Text = "No account found to update.";
                }
            }
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
