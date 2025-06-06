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
    /// Логика взаимодействия для CoolingTypesPage.xaml
    /// </summary>
    public partial class CoolingTypesPage : Page
    {
        private CoolingTypes selectedCoolingType = null;
        private bool isNewRecord = false;
        public CoolingTypesPage()
        {
            InitializeComponent();
            LoadCoolingTypes();
        }
        private void LoadCoolingTypes()
        {
            DGCoolingTypes.ItemsSource = DatabaseEntities.GetContext().CoolingTypes.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCoolingType = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCoolingType = (sender as Button).DataContext as CoolingTypes;
            if (selectedCoolingType != null)
            {
                isNewRecord = false;
                tbName.Text = selectedCoolingType.CoolingType;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                CoolingTypes newCoolingType = new CoolingTypes
                {
                    CoolingType = tbName.Text
                };
                context.CoolingTypes.Add(newCoolingType);
            }
            else if (selectedCoolingType != null)
            {
                selectedCoolingType.CoolingType = tbName.Text;
            }
            context.SaveChanges();
            LoadCoolingTypes();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var coolingType = (sender as Button).DataContext as CoolingTypes;
            if (coolingType != null && MessageBox.Show("Удалить этот тип охлаждения?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().CoolingTypes.Remove(coolingType);
                DatabaseEntities.GetContext().SaveChanges();
                LoadCoolingTypes();
            }
        }
    }
}
