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
    /// Логика взаимодействия для RAMTypesPage.xaml
    /// </summary>
    public partial class RAMTypesPage : Page
    {
        private RAMTypes selectedRAMType = null;
        private bool isNewRecord = false;
        public RAMTypesPage()
        {
            InitializeComponent();
            LoadRAMTypes();
        }
        private void LoadRAMTypes()
        {
            DGRAMTypes.ItemsSource = DatabaseEntities.GetContext().RAMTypes.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedRAMType = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedRAMType = (sender as Button).DataContext as RAMTypes;
            if (selectedRAMType != null)
            {
                isNewRecord = false;
                tbName.Text = selectedRAMType.RAMType;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                RAMTypes newRAMType = new RAMTypes
                {
                    RAMType = tbName.Text
                };
                context.RAMTypes.Add(newRAMType);
            }
            else if (selectedRAMType != null)
            {
                selectedRAMType.RAMType = tbName.Text;
            }
            context.SaveChanges();
            LoadRAMTypes();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var ramType = (sender as Button).DataContext as RAMTypes;
            if (ramType != null && MessageBox.Show("Удалить этот тип RAM?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().RAMTypes.Remove(ramType);
                DatabaseEntities.GetContext().SaveChanges();
                LoadRAMTypes();
            }
        }
    }
}
