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

namespace FUMiniHotel_ProjectPRN212.Invoice
{
    /// <summary>
    /// Interaction logic for InvoiceDetailWindow.xaml
    /// </summary>
    public partial class InvoiceDetailWindow : Window
    {
        public BusinessObjects.Invoice Invoice { get; private set; }
        public InvoiceDetailWindow(BusinessObjects.Invoice invoice)
        {
            InitializeComponent();
            Invoice = invoice;
            DataContext = Invoice;
            LoadInvoiceDetails();
        }
        private void LoadInvoiceDetails()
        {
            if (Invoice.Booking != null)
            {
                dgRoomDetails.ItemsSource = Invoice.Booking.BookingDetails;
                dgServiceDetails.ItemsSource = Invoice.Booking.BookingServices;
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            // Implement print functionality here
            MessageBox.Show("Xuất hóa đơn thành công!", "Thông báo",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            // You would typically call a print method here
            // PrintInvoice();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
