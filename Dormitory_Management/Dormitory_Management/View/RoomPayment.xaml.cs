using System;
using System.Collections.Generic;
using System.IO;
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
using ClosedXML.Excel;
using Dormitory_Management.Models;

namespace Dormitory_Management.View
{
    /// <summary>
    /// Interaction logic for RoomPayment.xaml
    /// </summary>
    public partial class RoomPayment : Page
    {
        private readonly Dormitory_ManagementContext _context;
        private string _currentPhone;

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
                AmountTextBox.Text = "";
                _currentPhone = phone;
                LoadPaymentData(phone);
            }
            else
            {
                MessageBox.Show("Student not found.");
                LoadPaymentData();
            }
        }

        private void ViewPayment_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentListBox.Visibility == Visibility.Collapsed)
            {
                PaymentListBox.Visibility = Visibility.Visible;
            }
            else
            {
                PaymentListBox.Visibility = Visibility.Collapsed;
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

            LoadPaymentData();
        }

        private void LoadPaymentData(string phone = null)
        {
            var paymentDataQuery = _context.Fees.AsQueryable();

            if (!string.IsNullOrEmpty(phone))
            {
                paymentDataQuery = paymentDataQuery.Where(f => f.MobileNo == phone);
            }

            var paymentData = paymentDataQuery
                .Select(f => new
                {
                    f.MobileNo,
                    f.Fmonth,
                    f.Amount
                })
                .ToList();

            PaymentListBox.ItemsSource = paymentData;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to export the payment data?", "Confirm Export", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                if (saveFileDialog.ShowDialog() == true)
                {
                    var filePath = saveFileDialog.FileName;

                    string phone = PhoneTextBox.Text.Trim();

                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Room Payments");

                            worksheet.Cell(1, 1).Value = "Phone Number";
                            worksheet.Cell(1, 2).Value = "Month";
                            worksheet.Cell(1, 3).Value = "Amount";

                            var paymentDataQuery = _context.Fees.AsQueryable();

                            if (!string.IsNullOrEmpty(phone))
                            {
                                paymentDataQuery = paymentDataQuery.Where(f => f.MobileNo == phone);
                            }

                            var paymentData = paymentDataQuery.ToList();

                            int row = 2;
                            foreach (var payment in paymentData)
                            {
                                worksheet.Cell(row, 1).Value = payment.MobileNo;
                                worksheet.Cell(row, 2).Value = payment.Fmonth;
                                worksheet.Cell(row, 3).Value = payment.Amount;
                                row++;
                            }

                            workbook.SaveAs(filePath);

                            MessageBox.Show("Excel file exported successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error when exporting Excel file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }




    }


}
