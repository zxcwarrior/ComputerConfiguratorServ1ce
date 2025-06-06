using ComputerConfiguratorService.Model;
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

namespace ComputerConfiguratorService.View
{
    /// <summary>
    /// Логика взаимодействия для VendorsPage.xaml
    /// </summary>
    public partial class VendorsPage : Page
    {
        private Vendors selectedVendor = null;
        private bool isNewRecord = false;
        public VendorsPage()
        {
            InitializeComponent();
            LoadVendors();
        }
        private void LoadVendors()
        {
            DGVendors.ItemsSource = DatabaseEntities.GetContext().Vendors.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedVendor = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedVendor = (sender as Button).DataContext as Vendors;
            if (selectedVendor != null)
            {
                isNewRecord = false;
                tbName.Text = selectedVendor.VendorName;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                Vendors newVendor = new Vendors
                {
                    VendorName = tbName.Text
                };
                context.Vendors.Add(newVendor);
            }
            else if (selectedVendor != null)
            {
                selectedVendor.VendorName = tbName.Text;
            }
            context.SaveChanges();
            LoadVendors();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var vendor = (sender as Button).DataContext as Vendors;
            if (vendor != null && MessageBox.Show("Удалить этого вендора?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().Vendors.Remove(vendor);
                DatabaseEntities.GetContext().SaveChanges();
                LoadVendors();
            }
        }
    }
}
