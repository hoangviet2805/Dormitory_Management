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
            
            // Load all payment data initially
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
                _currentPhone = phone;

                // Load payment data for this specific student
                LoadPaymentData(phone);
            }
            else
            {
                MessageBox.Show("Student not found.");
                ClearFields();
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

            var student = _context.Students.FirstOrDefault(s => s.Mobile == phone);
            if (student == null)
            {
                MessageBox.Show("Student not found.");
                return;
            }

            // ✅ Chỉ cho phép thanh toán nếu sinh viên đang ở
            if (student.Living == "No")
            {
                MessageBox.Show("This student has checked out and is unable to pay!");
                return;
            }

            string fmonth = paymentDate.Value.ToString("MMMM yyyy");
            var paymentExists = _context.Fees.Any(f => f.MobileNo == phone && f.Fmonth == fmonth);
            if (paymentExists)
            {
                MessageBox.Show("This month has already been paid.");
                return;
            }

            if (!int.TryParse(amount, out int amountToPay))
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

            LoadPaymentData(phone);
            MessageBox.Show("Payment successful.");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            LoadPaymentData();
        }

        private void ClearFields()
        {
            PhoneTextBox.Clear();
            StudentNameTextBox.Clear();
            EmailTextBox.Clear();
            RoomNumberTextBox.Clear();
            AmountTextBox.Clear();
            PaymentDatePicker.SelectedDate = null;
            _currentPhone = null;
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

            PaymentDataGrid.ItemsSource = paymentData;
            
            // Update summary information
            UpdatePaymentSummary(paymentData);
        }
        
        private void UpdatePaymentSummary<T>(List<T> paymentData) where T : class
        {
            TotalPaymentsText.Text = paymentData.Count.ToString();
            
            // Use reflection to get Amount property value
            var totalAmount = paymentData.Sum(p => 
            {
                var amountProperty = typeof(T).GetProperty("Amount");
                if (amountProperty != null)
                {
                    var value = amountProperty.GetValue(p);
                    return Convert.ToInt32(value);
                }
                return 0;
            });
            
            TotalAmountText.Text = $"${totalAmount:N0}";
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to export the payment data?", "Confirm Export", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx"
                };

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
