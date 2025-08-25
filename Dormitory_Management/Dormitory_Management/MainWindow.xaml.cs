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
using Dormitory_Management.View;

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
        private void DashboardClick(object sender, RoutedEventArgs e)
        {
            // Khi nhấn Dashboard thì load DashboardPage vào Frame
            MainFrame.Content = new View.DashboardPage();
        }

        private void ManageRoomsClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageRoomsPage());
        }

        private void AddStudentClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddStudentPage());
        }

        private void UpdateDeleteStudentClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EditStudentPage());
        }

        private void RoomPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RoomPayment());
        }

        private void ManageStudentStatusClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StudentStatus());
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm logout.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                this.Hide();
                Login loginWindow = new Login();
                loginWindow.Show();
            }
          
        }

        
    }
}