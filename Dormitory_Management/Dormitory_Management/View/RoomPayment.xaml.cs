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
    /// Interaction logic for RoomPayment.xaml
    /// </summary>
    public partial class RoomPayment : Page
    {
        private readonly Dormitory_ManagementContext _context;

        public RoomPayment()
        {
            InitializeComponent();
            _context = new Dormitory_ManagementContext();
            LoadPaymentData();
        }
        private void SearchStudent_Click(object sender, RoutedEventArgs e)
        {
            string phone = PhoneTextBox.Text.Trim();

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Please enter a phone number.");
                return;
            }

            var student = _context.Students.FirstOrDefault(s => s.Mobile == phone);
            if (student != null)
            {
                StudentNameTextBox.Text = student.Name;
                EmailTextBox.Text = student.Email;
                RoomNumberTextBox.Text = student.RoomNo.ToString();
                AmountTextBox.Text = "20000"; 
            }
            else
            {
                MessageBox.Show("Student not found.");
            }
        }
        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            string phone = PhoneTextBox.Text.Trim();
            DateTime? paymentDate = PaymentDatePicker.SelectedDate;
            string amount = AmountTextBox.Text.Trim();

            if (string.IsNullOrEmpty(phone) || !paymentDate.HasValue || string.IsNullOrEmpty(amount))
            {
                MessageBox.Show("Please fill in all the information.");
                return;
            }

            string fmonth = paymentDate.Value.ToString("MMMM yyyy");
            var paymentExists = _context.Fees.Any(f => f.MobileNo == phone && f.Fmonth == fmonth);
            if (paymentExists)
            {
                MessageBox.Show("This month has already been paid.");
                return;
            }

            int amountToPay;
            if (!int.TryParse(amount, out amountToPay))
            {
                MessageBox.Show("Invalid amount.");
                return;
            }

            var payment = new Fee
            {
                MobileNo = phone,
                Fmonth = fmonth, 
                Amount = amountToPay
            };

            _context.Fees.Add(payment);
            _context.SaveChanges();

            LoadPaymentData();

            MessageBox.Show("Payment successful.");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            PhoneTextBox.Clear();
            StudentNameTextBox.Clear();
            EmailTextBox.Clear();
            RoomNumberTextBox.Clear();
            AmountTextBox.Clear();
            PaymentDatePicker.SelectedDate = null;
        }

        private void LoadPaymentData()
        {
            var paymentData = _context.Fees
                .Select(f => new
                {
                    f.MobileNo,
                    f.Fmonth,
                    f.Amount
                })
                .ToList();

            PaymentListBox.ItemsSource = paymentData;
        }
    }
}
