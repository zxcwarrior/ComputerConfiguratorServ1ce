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
    /// Логика взаимодействия для StorageTypesPage.xaml
    /// </summary>
    public partial class StorageTypesPage : Page
    {
        private StorageTypes selectedStorageType = null;
        private bool isNewRecord = false;
        public StorageTypesPage()
        {
            InitializeComponent();
            LoadStorageTypes();
        }
        private void LoadStorageTypes()
        {
            DGStorageTypes.ItemsSource = DatabaseEntities.GetContext().StorageTypes.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedStorageType = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedStorageType = (sender as Button).DataContext as StorageTypes;
            if (selectedStorageType != null)
            {
                isNewRecord = false;
                tbName.Text = selectedStorageType.StorageType;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                StorageTypes newStorageType = new StorageTypes
                {
                    StorageType = tbName.Text
                };
                context.StorageTypes.Add(newStorageType);
            }
            else if (selectedStorageType != null)
            {
                selectedStorageType.StorageType = tbName.Text;
            }
            context.SaveChanges();
            LoadStorageTypes();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var storageType = (sender as Button).DataContext as StorageTypes;
            if (storageType != null && MessageBox.Show("Удалить этот тип хранилища?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().StorageTypes.Remove(storageType);
                DatabaseEntities.GetContext().SaveChanges();
                LoadStorageTypes();
            }
        }
    }
}
